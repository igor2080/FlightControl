﻿using FlightControl.Data;
using FlightControl.Simulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Logic
{
    /// <summary>
    /// Represents the airport itself
    /// </summary>
    public static class Chain
    {
        private static List<Slot> _slots;

        public const int NumberOfStations = 9;

        private static bool IsInitialized = false;

        /// <summary>
        /// Retrieves information about a specific station(leg)
        /// </summary>
        /// <param name="station"></param>
        public static Airplane GetPlaneInfo(int station)
        {
            //treat the parameter as an actual station number, 
            //the array still starts at zero
            return _slots[station - 1]?.GetCurrentPlane();
        }

        /// <summary>
        /// Move a specific plane onward
        /// </summary>
        /// <param name="planeNumber"></param>
        /// <returns></returns>
        public static Information MovePlane(int planeNumber)
        {
            int index = _slots.FindIndex(x => x.GetCurrentPlane()?.ID == planeNumber);
            if (index != -1)//plane found
            {
                var plane = _slots[index].GetCurrentPlane();
                if ((DateTime.Now - _slots[index].PlaneArrivalToStation).TotalSeconds < 4)
                {//The plane has just arrived, do not move it
                    return new Information(index + 1, $"The plane #{plane.ID} cannot move yet, it has just arrived", InfoCode.JustArrived);
                }
                int nextIndex = GetNextSlot(index + 1, _slots[index].GetCurrentPlane().Landing) - 1;
                if (nextIndex == -2)//invalid slot number returned
                {
                    return new Information(-1, $"The plane #{plane.ID} is located in an invalid station", InfoCode.Invalid);
                }
                else if (nextIndex == -1)//the plane is out of the system(departed, entered the terminal)
                {
                    
                    _slots[index].RemovePlane();
                    
                    return new Information(0, $"Plane {plane.ID} Has left the system!", InfoCode.LeftTheSystem);
                    //TODO:log plane removal
                }
                else
                {
                    if (_slots[nextIndex].GetCurrentPlane() == null)//there is no plane in the next slot
                    {
                        if (nextIndex != 3)
                        {
                            _slots[index].RemovePlane();
                            _slots[nextIndex].AcceptPlane(plane);
                            //TODO:log plane movement
                            
                            return new Information(index + 1, $"Plane moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);
                        }
                        else
                        {//special conditions for station 4(interaction with 8 and 3)

                            if (_slots[2].GetCurrentPlane() != null && _slots[7].GetCurrentPlane() != null)
                            {//there are planes in both 3 and 8, compare timers

                                bool Station3Priority = _slots[2].PlaneArrivalToStation <= _slots[7].PlaneArrivalToStation.AddSeconds(15.0);
                                //station 3 has priority of 15 seconds over 8 
                                if (index == 2)
                                {
                                    if (Station3Priority)
                                    {//current plane is from station 3, and has priority
                                        _slots[index].RemovePlane();
                                        _slots[nextIndex].AcceptPlane(plane);
                                        //TODO:log plane movement
                                        return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);

                                    }
                                    else return new Information(index + 1, $"The plane #{plane.ID} isn't allowed to move due to priority", InfoCode.MovementForbidden);
                                }
                                else //index == 7
                                {
                                    if (!Station3Priority)
                                    {//current plane is from station 8, and has priority
                                        _slots[index].RemovePlane();
                                        _slots[nextIndex].AcceptPlane(plane);
                                        //TODO:log plane movement
                                        
                                        return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);

                                    }
                                    else return new Information(index + 1, $"The plane #{plane.ID} isn't allowed to move due to priority", InfoCode.MovementForbidden);
                                }
                            }
                            else
                            {//only one plane in either station, proceed as normal
                                _slots[index].RemovePlane();
                                _slots[nextIndex].AcceptPlane(plane);
                                //TODO:log plane movement
                                
                                return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);

                            }
                        }

                    }
                    else // there is a plane in the next slot
                    {
                        if (nextIndex == 5)
                        {//there is a plane in station 6, there might be room in 7

                            if (_slots[6].GetCurrentPlane() == null)
                            {//there is!
                                nextIndex = 6;
                                _slots[index].RemovePlane();
                                _slots[nextIndex].AcceptPlane(plane);
                                //TODO:log plane movement
                                
                                return new Information(index + 1, $"Plane moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);
                            }
                        }
                        
                        return new Information(index + 1, $"The plane at station {index + 1} cannot move, the next station is occupied", InfoCode.Occupied);

                    }
                }
            }
            else//plane not found
            {
                return new Information(-1, "The plane was not found", InfoCode.NotFound);
            }
        }

        /// <summary>
        /// Update a specific station by moving the plane that is inside forward, if possible
        /// </summary>
        /// <param name="stationNumber">The station number to update</param>
        /// <returns>Information whether the movement happened or not</returns>
        public static Information UpdateStation(int stationNumber)
        {
            var plane = GetPlaneInfo(stationNumber);//get the plane
            if (plane == null)
                return new Information(stationNumber, "There is no plane", InfoCode.NotFound);
            var planeMovement = MovePlane(plane.ID);//try moving the plane
            return planeMovement;
        }



        /// <summary>
        /// Get the next slot that a plane should go to, depending on it's direction
        /// </summary>
        /// <param name="currentSlot">The current slot of the plane</param>
        /// <param name="isLanding">Whether the plane is landing or departing</param>
        /// <returns></returns>
        private static int GetNextSlot(int currentSlot, bool isLanding)
        {//the slot paramter is treated as an actual station, not as the index value
            switch (currentSlot)
            {
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                    return isLanding ? 5 : 9;
                case 5:
                    return 6;
                case 6:
                    return isLanding ? 0 : 8;
                case 7:
                    return isLanding ? 0 : 8;
                case 8:
                    return 4;
                case 9:
                    return 0;
            }
            return -1;
        }

        /// <summary>
        /// Fill up the chain with slots
        /// </summary>
        public static void InitializeChain()
        {
            if (!IsInitialized)
            {
                _slots = new List<Slot>();

                for (int i = 1; i <= 9; i++)
                {
                    _slots.Add(new Slot((byte)i));
                }
                IsInitialized = true;
                //TODO: Log start
            }

        }
        /// <summary>
        /// Add a plane into the system
        /// </summary>
        /// <param name="isLanding">Is the plane landing or taking off?</param>
        /// <returns>Information about the action's success</returns>
        public static Information AcceptPlane(bool isLanding)
        {
            var plane = Simulation.GeneratePlane(isLanding);
            if (plane.Landing)
            {
                if (_slots[0].GetCurrentPlane() == null)//no plane in slot 1
                {
                    _slots[0].AcceptPlane(plane);
                    //TODO:log plane acceptance
                    return new Information(1, "Plane accepted successfully into station 1", InfoCode.Success);
                }
                else//there is a plane in slot 1, plane is rejected
                {
                    //TODO:log plane rejection
                    return new Information(-1, "There is no room for a plane", InfoCode.Error);
                }
            }
            else//plane is departing
            {
                if (_slots[5].GetCurrentPlane() == null)//slot 6 is empty, can accept
                {
                    if (_slots[6].GetCurrentPlane() != null && !_slots[6].GetCurrentPlane().Landing)
                    {//There is a plane in station 7 that is also departing, unable to add another departing plane!
                        return new Information(6, "Cannot have two departing planes at docking stations!", InfoCode.Occupied);
                    }
                    else
                    {
                        _slots[5].AcceptPlane(plane);
                        //TODO:log plane acceptance
                        return new Information(6, "Plane accepted successfully into station 6", InfoCode.Success); ;
                    }
                }
                else if (_slots[6].GetCurrentPlane() == null)//slot 7 is empty instead of 6, can accept
                {
                    if (_slots[5].GetCurrentPlane() != null && !_slots[5].GetCurrentPlane().Landing)
                    {//There is a plane in station 6 that is also departing, unable to add another departing plane!
                        return new Information(7, "Cannot have two departing planes at docking stations!", InfoCode.Occupied);
                    }
                    _slots[6].AcceptPlane(plane);
                    //TODO:log plane acceptance
                    return new Information(7, "Plane accepted successfully into station 7", InfoCode.Success); ;
                }
                else//both slots are taken, plane is rejected
                {
                    return new Information(-1, "There is no room for a plane", InfoCode.Error);
                    //TODO:log plane rejection

                }

            }
        }

        /// <summary>
        /// Represents an entity in the chain, a leg
        /// </summary>
        private class Slot
        {
            /// <summary>
            /// Slot number in the chain
            /// </summary>
            public byte Number { get; set; }

            /// <summary>
            /// The plane that is in this slot
            /// </summary>
            Airplane OccupyingPlane;

            public Airplane GetCurrentPlane() => OccupyingPlane;


            /// <summary>
            /// When a plane has arrived in the current station
            /// </summary>
            public DateTime PlaneArrivalToStation { get; set; }

            /// <summary>
            /// Indicates whether the current station is operating
            /// </summary>
            public bool IsActive { get; set; }

            /// <summary>
            /// Accept a plane into a station
            /// </summary>
            /// <param name="plane">The plane to accept</param>
            public void AcceptPlane(Airplane plane)
            {
                if (OccupyingPlane == null)
                {
                    OccupyingPlane = plane;
                    PlaneArrivalToStation = DateTime.Now;
                }
                else
                {
                    //TODO: log error plane collision!!
                }
            }

            /// <summary>
            /// Remove the plane from a leg
            /// </summary>
            public void RemovePlane()
            {
                OccupyingPlane = null;
                PlaneArrivalToStation = DateTime.MinValue;

            }
            public Slot(byte num)
            {
                Number = num;
                PlaneArrivalToStation = DateTime.MinValue;

            }

        }
    }
}
