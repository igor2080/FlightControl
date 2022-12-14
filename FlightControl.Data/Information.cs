using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Data
{
    public enum InfoCode
    {
        NotFound = 0,
        Invalid = 1,
        JustArrived = 2,
        Occupied = 3,
        //map(GUI) updating values
        LeftTheSystem = 4,
        Moved = 5,
        Open = 6,
        Closed = 7,
        //
        Success = 8,
        Error = 9,
        MovementForbidden = 10,
        Started = 11,
        Emergency = 12,
        Saved = 13
    }
    /// <summary>
    /// Used to provide information about specific actions; whether they were successful or not
    /// </summary>
    public class Information
    {
        [Key]
        public int InformationID { get; internal set; }

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

        public Information()
        {

        }
        /// <summary>
        /// Get a single log piece from the log queue
        /// </summary>
        /// <returns></returns>
        public static Information GetLogPiece()
        {
            if (logs.Count > nextLog)
            {
                Information log = logs[nextLog];
                if (log != null)
                {
                    nextLog++;
                    return log;
                }

            }
            return null;

        }

        //Save newly created logs to the database
        public static void SaveLogsToDB()
        {
            if (logs.Any(x=>x.Code!=InfoCode.Saved))
            {//prevent saving with just the saved message
                using (var context=new AirportContext())
                {
                    try
                    {
                        var templogs = logs;
                        context.Logs.AddRange(templogs);
                        context.SaveChanges();
                        logs.Clear();
                        nextLog = 0;
                        new Information(-1, "Saved logs to database!", InfoCode.Saved);
                    }
                    catch
                    {

                    }
                }
            }
            
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

        /// <summary>
        /// Initialize a new piece of log information, it is automatically stored upon creation in the log list
        /// </summary>
        /// <param name="stationid">A relevant station id for which the log is mean for</param>
        /// <param name="msg">The msesage to log</param>
        /// <param name="code">The appropriate code to go with the message</param>
        public Information(int stationid, string msg, InfoCode code)
        {
            StationID = stationid;
            Message = msg;
            Code = code;
            logs.Add(this);

        }
    }

}
