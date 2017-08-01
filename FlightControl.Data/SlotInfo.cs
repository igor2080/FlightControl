using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Data
{
    /// <summary>
    /// A class used for reading station information on the UI, and for storing into the database
    /// </summary>
    public class SlotInfo
    {
        public int SlotInfoID { get; internal set; }
        public int Station { get; private set; }        
        public Airplane Plane { get; private set; }
        public bool Active { get; private set; }
        public double Arrival { get; private set; }//easier to use in javascript
        public SlotInfo()
        {

        }
        public SlotInfo(int stationNum, Airplane plane, bool isActive, DateTime arrivalToStation)
        {
            Station = stationNum;
            Plane = plane;
            Active = isActive;
            Arrival = arrivalToStation.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
