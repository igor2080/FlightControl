using FlightControl.Data;
using System;
using System.Collections.Generic;
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
        public static (int Number, Airplane CurrentAirplane) GetInfo(int station)
        {
            //treat the parameter as an actual station number, 
            //the array still starts at zero
            if (station < 1 || station > 9)
            {
                return (0, null);
            }
            return (_slots[station - 1].Number, _slots[station - 1].OccupyingPlane);
        }

        /// <summary>
        /// Move a specific plane onward
        /// </summary>
        /// <param name="planeNumber"></param>
        /// <returns></returns>
        public static Information MovePlane(int planeNumber)
        {
            int index = _slots.FindIndex(x => x.OccupyingPlane?.ID == planeNumber);
            if (index != -1)//plane found
            {
                if((_slots[index].PlaneArrivalToStation - DateTime.Now).TotalSeconds < 5)
                {//The plane has just arrived, do not move it
                    return new Information(index + 1, "The plane has just arrived", InfoCode.JustArrived);
                }
                int nextIndex = GetNextSlot(index + 1, _slots[index].OccupyingPlane.Landing) - 1;
                if (nextIndex == -1)//invalid slot number returned
                {
                    return new Information(-1, "The plane is located in an invalid station", InfoCode.Invalid);
                }
                else if (nextIndex == 0)//the plane is out of the system(departed, entered the terminal)
                {
                    var item = _slots[index].OccupyingPlane;
                    _slots[index].OccupyingPlane = null;
                    _slots[index].PlaneArrivalToStation = DateTime.MinValue;
                    return new Information(0, "The plane has left the system", InfoCode.LeftTheSystem);
                    //TODO:log plane removal
                }
                else
                {
                    if (_slots[nextIndex].OccupyingPlane == null)//there is no plane in the next slot
                    {
                        var item = _slots[index].OccupyingPlane;
                        _slots[index].OccupyingPlane = null;
                        _slots[index].PlaneArrivalToStation = DateTime.MinValue;
                        _slots[nextIndex].OccupyingPlane = item;
                        _slots[nextIndex].PlaneArrivalToStation = DateTime.Now;
                        return new Information(index + 1, $"The plane has moved to station {nextIndex + 1} successfully", InfoCode.Moved);
                        //TODO:log plane movement
                    }
                    else // there is a plane in the next slot
                    {
                        return new Information(index + 1, "The plane cannot move, the next station is occupied", InfoCode.Occupied);
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
            var station = GetInfo(stationNumber);//get the station
            var planeMovement = MovePlane(station.CurrentAirplane.ID);//try moving the plane
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

        public static bool AcceptPlane(Airplane plane)
        {
            if (plane.Landing)
            {
                if (_slots[0].OccupyingPlane == null)//no plane in slot 1
                {
                    _slots[0].OccupyingPlane = plane;
                    _slots[0].PlaneArrivalToStation = DateTime.Now;
                    //TODO:log plane acceptance
                    return true;
                }
                else//there is a plane in slot 1, plane is rejected
                {
                    //TODO:log plane rejection
                    return false;
                }
            }
            else//plane is departing
            {
                if (_slots[5].OccupyingPlane == null)//slot 6 is empty, can accept
                {
                    _slots[5].OccupyingPlane = plane;
                    _slots[5].PlaneArrivalToStation = DateTime.Now;
                    //TODO:log plane acceptance
                    return true;
                }
                else if (_slots[6].OccupyingPlane == null)
                {
                    _slots[6].OccupyingPlane = plane;
                    _slots[6].PlaneArrivalToStation = DateTime.Now;
                    //TODO:log plane acceptance
                    return true;
                }
                else//both slots are taken, plane is rejected
                {
                    return false;
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
            public Airplane OccupyingPlane { get; set; }

            /// <summary>
            /// When a plane has arrived in the current station
            /// </summary>
            public DateTime PlaneArrivalToStation { get; set; }

            public Slot(byte num)
            {
                Number = num;
                PlaneArrivalToStation = DateTime.MinValue;
            }

        }
    }
}
