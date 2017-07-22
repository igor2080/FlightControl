using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        MovementForbidden = 128,
        Open = 256,
        Closed = 512,
        Started = 1024

    }
    /// <summary>
    /// Used to provide information about specific actions; whether they were successful or not
    /// </summary>
    public class Information
    {
        static List<Information> logs = new List<Information>();
        static int nextLog = 0;
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

        /// <summary>
        /// Get a single log piece from the log queue
        /// </summary>
        /// <returns></returns>
        public static Information GetLogPiece()
        {
            return logs[nextLog++];
        }


        public static bool SaveLogsToDB()
        {
            //TODO: dump all logs from list to DB(reset the next log back to 0)
            return false;
        }
        /// <summary>
        /// Gets a filtered list of logs based on the codes
        /// </summary>
        /// <param name="codes">The code(s) to filter by</param>
        /// <returns>All the logs matching the code(s)</returns>
        public static List<Information> GetLogs(params InfoCode[] codes)
        {
            var loglist = new List<Information>();

            foreach (var code in codes)
            {
                loglist.AddRange(logs.Where(x => x.Code == code));
            }
            return loglist;
        }

        public Information(int stationid, string msg, InfoCode code)
        {
            StationID = stationid;
            Message = msg;
            Code = code;
            logs.Add(this);
            
        }
    }

}
