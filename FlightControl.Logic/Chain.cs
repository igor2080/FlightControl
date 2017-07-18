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

        static Chain()
        {
            InitializeChain();
        }
        /// <summary>
        /// Retrieves information about a specific station(leg)
        /// </summary>
        /// <param name="station"></param>
        public static (int Number, Airplane CurrentAirplane) GetInfo(int station)
        {

            return (_slots[station].Number, _slots[station].OccupyingPlane);
        }

        /// <summary>
        /// Move a specific plane onward
        /// </summary>
        /// <param name="planeNumber"></param>
        /// <returns></returns>
        public static bool MovePlane(int planeNumber)
        {
            int index = _slots.FindIndex(x =>x.OccupyingPlane.ID == planeNumber);
            if (index != -1)//plane found
            {
                int nextIndex = GetNextSlot(index + 1, _slots[index].OccupyingPlane.Landing);
                if (nextIndex == -1)//invalid slot number returned
                {
                    return false;
                }
                else if (nextIndex == 0)//the plane is out of the system(departed, entered the terminal)
                {
                    
                    _slots.RemoveAt(index);
                    return true;
                    //TODO:log plane removal
                }
                else
                {
                    if (_slots[nextIndex].OccupyingPlane == null)//there is no plane in the next slot
                    {
                        var item = _slots[index];
                        _slots.RemoveAt(index);
                        _slots[nextIndex] = item;
                        return true;
                        //TODO:log plane movement
                    }
                    else // there is a plane in the next slot
                    {
                        return false;
                    }
                }
            }
            else//plane not found
            {
                return false;
            }
        }
        /// <summary>
        /// Get the next slot that a plane should go to, depending on it's direction
        /// </summary>
        /// <param name="currentSlot">The current slot of the plane</param>
        /// <param name="isLanding">Whether the plane is landing or departing</param>
        /// <returns></returns>
        private static int GetNextSlot(int currentSlot, bool isLanding)
        {
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
        private static void InitializeChain()
        {
            _slots = new List<Slot>();

            for (int i = 1; i <= 9; i++)
            {
                _slots.Add(new Slot((byte)i));
            }
        }

        public static bool AcceptPlane(Airplane plane)
        {
            if (plane.Landing)
            {
                if (_slots[0].OccupyingPlane == null)//no plane in slot 1
                {
                    _slots[0].OccupyingPlane = plane;
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
                    //TODO:log plane acceptance
                    return true;
                }
                else if (_slots[6].OccupyingPlane == null)
                {
                    _slots[6].OccupyingPlane = plane;
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

            public Slot(byte num)
            {
                Number = num;
            }

        }
    }
}
