using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Logic
{
    public enum InfoCode
    {
        NotFound = 0,
        Invalid = 1,
        JustArrived = 2,
        LeftTheSystem = 4,
        Moved = 8,
        Occupied = 16,
        Success = 32,
        Error = 64,

    }
    /// <summary>
    /// Used to provide information about specific actions; whether they were successful or not
    /// </summary>
    public class Information
    {
        /// <summary>
        /// ID of the station
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// Report on the situation
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The requested action has succeeded of failed
        /// </summary>
        public InfoCode Code { get; set; }


        public Information(int stationid, string msg, InfoCode code)
        {
            StationID = stationid;
            Message = msg;
            Code = code;
        }
    }

}
