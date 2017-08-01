using FlightControl.Data;
using FlightControl.Simulator;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Data
{
    /// <summary>
    /// Represents the airport itself
    /// </summary>
    public static class Chain
    {
        private static List<Slot> _slots;

        public const int NumberOfStations = 9;
        
        static InfoCode[] EmergencyClosedStations = new InfoCode[4];

        //Station which needs an emergency landing
        static int EmergencyStation = -1;
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
            //Backup();

            //find what station the plane is in. station = index + 1
            int index = _slots.FindIndex(x => x.GetCurrentPlane()?.ID == planeNumber);
            if (index != -1)//plane found
            {
                if (EmergencyStation != -1)
                {//Emergency landing!!
                    if (index == EmergencyStation - 1)
                    {
                        if (_slots[3].IsActive && _slots[3].GetCurrentPlane() == null)
                        {//station 4 is finally empty and operational, land the emergency plane
                            _slots[3].AcceptPlane(_slots[index].GetCurrentPlane());
                            _slots[index].RemovePlane();
                            StopEmergency();
                            return new Information(index + 1, "The emergency plane has landed!", InfoCode.Success);
                        }
                    }
                }
                if (_slots[index].IsActive == false)
                {//station closed, no plane can get in or out
                    return new Information(index + 1, $"Station #{index + 1} is closed! cannot move planes in or to it", InfoCode.Closed);
                }

                var plane = _slots[index].GetCurrentPlane();
                if ((DateTime.Now - _slots[index].PlaneArrivalToStation).TotalSeconds < 4)
                {//The plane has just arrived, do not move it
                    return new Information(index + 1, $"The plane #{plane.ID} cannot move yet, it has just arrived", InfoCode.JustArrived);
                }
                int nextIndex = GetNextSlot(index + 1, _slots[index].GetCurrentPlane().Landing) - 1;
                if (nextIndex > 0 && _slots[nextIndex].IsActive == false)
                {//next station is closed, cannot move planes to it
                    //check if next station is 6 and 7 is open
                    if (nextIndex == 5 && _slots[6].IsActive)
                    {//next station happens to be 6, checking if station 7 is open AND has no planes
                        if (_slots[6].GetCurrentPlane() == null)
                        {
                            _slots[index].RemovePlane();
                            _slots[6].AcceptPlane(plane);
                            return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station 7", InfoCode.Moved);
                        }
                        else return new Information(index + 1, "The next station is occupied, cannot move planes to it", InfoCode.Occupied);
                    }
                    else return new Information(index + 1, "The next station is closed! cannot move planes to it", InfoCode.Closed);
                }
                else
                {//the next station is open

                    if (nextIndex == -2)//invalid slot number returned
                    {
                        return new Information(-1, $"The plane #{plane.ID} is located in an invalid station", InfoCode.Invalid);
                    }
                    else if (nextIndex == -1)//the plane is out of the system(departed, entered the terminal)
                    {
                        _slots[index].RemovePlane();
                        return new Information(0, $"Plane #{plane.ID} Has left the system!", InfoCode.LeftTheSystem);
                        //TODO:log plane removal
                    }
                    else
                    {
                        if (_slots[nextIndex].GetCurrentPlane() == null)
                        {//there is no plane in the next slot
                            if (nextIndex != 3)
                            {
                                _slots[index].RemovePlane();
                                _slots[nextIndex].AcceptPlane(plane);
                                //TODO:log plane movement

                                return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);
                            }
                            else
                            {//special conditions for station 4(interaction with 8 and 3)
                                if (_slots[2].GetCurrentPlane() != null && _slots[7].GetCurrentPlane() != null)
                                {//there are planes in both 3 and 8, compare timers for priority
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
                                    return new Information(index + 1, $"Plane #{plane.ID} moved from station {index + 1} to station {nextIndex + 1}", InfoCode.Moved);
                                }
                            }
                            return new Information(index + 1, $"The plane #{plane.ID} at station {index + 1} cannot move, the next station is occupied", InfoCode.Occupied);
                        }
                    }
                }
            }
            else//plane not found
            {
                return new Information(-1, "The plane was not found", InfoCode.NotFound);
            }
        }
        /// <summary>
        /// Load the saved state of the airport
        /// </summary>
        /// <param name="slots">The saved data</param>
        internal static void Restore(List<SlotInfo> slots)
        {
            _slots = new List<Slot>();
            _slots.AddRange(slots.Select(x => new Slot((byte)x.Station, x.Plane, x.Active, DateTime.MinValue.AddMilliseconds(x.Arrival<0?0:x.Arrival))));
        }

        public static Information Backup()
        {
            if (EmergencyStation != -1)
            {
                return new Information(-1, "Cannot back up during an emergency landing!!", InfoCode.Error);
                
            }
            using (var context=new AirportContext())
            {
                var back = context.Slots.Include("Plane").ToList();                
                context.Slots.RemoveRange(context.Slots);
                context.SaveChanges();
                context.Slots.AddRange(_slots.Select(x => new SlotInfo(x.Number, x.GetCurrentPlane(), x.IsActive, x.PlaneArrivalToStation)).ToList());                
                context.SaveChanges();
            }
            return new Information(-1, "System state has been saved!", InfoCode.Success);
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
        /// Perform an emergency landing for a specific station
        /// </summary>
        /// <param name="station">The station to land immediately</param>
        /// <returns>Information about whether the operation succeded or failed</returns>
        public static Information EmergencyLanding(int station)
        {
            if(_slots[station - 1].GetCurrentPlane() == null)
            {
                return new Information(station, "There is no plane in this station", InfoCode.Error);
            }
            if (EmergencyStation != -1)
                return new Information(EmergencyStation, "There is an emergency landing already happening!", InfoCode.Error);

            if (station > 0 && station < 4)
            {
                EmergencyStation = station;
                EmergencyClosedStations[0] = CloseStation(1).Code;
                EmergencyClosedStations[1] = CloseStation(2).Code;
                EmergencyClosedStations[2] = CloseStation(3).Code;
                EmergencyClosedStations[3] = CloseStation(8).Code;
                return new Information(station, "An emergency landing is happening!!", InfoCode.Emergency);
            }
            else return new Information(-1, $"The entered station ({station}) is invalid for landing.", InfoCode.Invalid);
        }

        /// <summary>
        /// End an emergency landing
        /// </summary>
        /// <returns></returns>
        public static Information StopEmergency()
        {
            EmergencyStation = -1;
            if (EmergencyClosedStations[0] == InfoCode.Closed)
                OpenStation(1);
            if (EmergencyClosedStations[1] == InfoCode.Closed)
                OpenStation(2);
            if (EmergencyClosedStations[2] == InfoCode.Closed)
                OpenStation(3);
            if (EmergencyClosedStations[3] == InfoCode.Closed)
                OpenStation(8);

            return new Information(-1, "Airport is now operating normally", InfoCode.Success);

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

        ///// <summary>
        ///// Fill up the chain with slots
        ///// </summary>
        //public static Information InitializeChain()
        //{
        //    if (!IsInitialized)
        //    {
        //        _slots = new List<Slot>();

        //        for (int i = 1; i <= 9; i++)
        //        {
        //            _slots.Add(new Slot((byte)i));
        //        }
        //        IsInitialized = true;
        //        //TODO: Log start
        //        return new Information(-1, "The system has started successfully!", InfoCode.Started);
        //    }
        //    return new Information(-1, "The system is already running.", InfoCode.Error);

        //}
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
                if (_slots[0].GetCurrentPlane() == null && _slots[0].IsActive)//no plane in slot 1 and the slot is operational
                {
                    _slots[0].AcceptPlane(plane);
                    //TODO:log plane acceptance
                    return new Information(1, $"Plane #{plane.ID} accepted successfully into station 1", InfoCode.Success);
                }
                else//there is a plane in slot 1, plane is rejected
                {
                    //TODO:log plane rejection
                    return new Information(-1, "There is no room for a plane, or the station is not operational", InfoCode.Error);
                }
            }
            else//plane is departing
            {
                if (_slots[5].GetCurrentPlane() == null && _slots[5].IsActive)//slot 6 is empty and operational, can accept
                {
                    if (_slots[6].GetCurrentPlane() != null && !_slots[6].GetCurrentPlane().Landing)
                    {//There is a plane in station 7 that is also departing, unable to add another departing plane!
                        return new Information(6, "Cannot have two departing planes at docking stations!", InfoCode.Occupied);
                    }
                    else
                    {
                        _slots[5].AcceptPlane(plane);
                        //TODO:log plane acceptance
                        return new Information(6, $"Plane #{plane.ID} accepted successfully into station 6", InfoCode.Success); ;
                    }
                }
                else if (_slots[6].GetCurrentPlane() == null && _slots[6].IsActive)//slot 7 is empty (and opertaional) instead of 6, can accept
                {
                    if (_slots[5].GetCurrentPlane() != null && !_slots[5].GetCurrentPlane().Landing)
                    {//There is a plane in station 6 that is also departing, unable to add another departing plane!
                        return new Information(7, "Cannot have two departing planes at docking stations!", InfoCode.Occupied);
                    }
                    _slots[6].AcceptPlane(plane);
                    //TODO:log plane acceptance
                    return new Information(7, $"Plane #{plane.ID} accepted successfully into station 7", InfoCode.Success); ;
                }
                else//both slots are taken, plane is rejected
                {
                    return new Information(-1, "There is no room for a plane, the stations are either closed or occupied", InfoCode.Error);
                    //TODO:log plane rejection

                }

            }
        }
        /// <summary>
        /// Closes a station
        /// </summary>
        /// <param name="station">Station number</param>
        /// <returns>Returns information on whether the operation was successful</returns>
        public static Information CloseStation(int station)
        {
            if (station > 0 && station < 10)
            {
                if (_slots[station - 1].IsActive)
                {
                    _slots[station - 1].IsActive = false;
                    return new Information(station, $"Station #{station} is now closed!", InfoCode.Closed);
                }
                return new Information(station, $"Station #{station} is already closed!", InfoCode.Error);
            }
            else return new Information(-1, "Invalid station number!", InfoCode.Invalid);
        }

        /// <summary>
        /// Opens a station
        /// </summary>
        /// <param name="station">Station number</param>
        /// <returns>Returns information on whether the operation was successful</returns>
        public static Information OpenStation(int station)
        {
            if (station > 0 && station < 10)
            {
                if (!_slots[station - 1].IsActive)
                {
                    _slots[station - 1].IsActive = true;
                    return new Information(station, $"Station #{station} is now open!", InfoCode.Open);
                }
                return new Information(station, $"Station #{station} is already open!", InfoCode.Error);
            }
            else return new Information(-1, "Invalid station number!", InfoCode.Invalid);

        }

        /// <summary>
        /// Retrieves information about a station
        /// </summary>
        /// <param name="station">The station number</param>
        /// <returns>Information about a station</returns>
        public static SlotInfo GetStationInfo(int station)
        {
            return _slots[station - 1].ToInfo();
        }

        /// <summary>
        /// Retrieves information on all stations
        /// </summary>
        /// <returns>Information about all stations</returns>
        public static List<SlotInfo> GetStations()
        {
            return _slots.Select(x => x.ToInfo()).ToList();
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

            /// <summary>
            /// Get the current plane in the station
            /// </summary>
            /// <returns></returns>
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
                if (OccupyingPlane == null && IsActive)
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
            /// Remove the plane from a station
            /// </summary>
            public void RemovePlane()
            {
                OccupyingPlane = null;
                PlaneArrivalToStation = DateTime.MinValue;

            }
            /// <summary>
            /// Convert a station to a UI publicly useable format
            /// </summary>
            /// <returns></returns>
            public SlotInfo ToInfo()
            {
                return new SlotInfo(Number, OccupyingPlane, IsActive, PlaneArrivalToStation);
            }

            public Slot(byte num)
            {
                Number = num;
                PlaneArrivalToStation = DateTime.MinValue;
                IsActive = true;

            }
            public Slot(byte num,Airplane plane,bool isactive,DateTime time)
            {
                Number = num;
                OccupyingPlane = plane;
                IsActive = isactive;
                PlaneArrivalToStation = time;
            }

        }

        

    }
}
