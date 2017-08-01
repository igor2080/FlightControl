using FlightControl.Data;
using FlightControl.Simulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FlightControl.Data
{
    public static class Main
    {
        static System.Timers.Timer clock = new System.Timers.Timer(5000);
        static System.Timers.Timer dbClock = new System.Timers.Timer(60000);

        static bool started = false;
        /// <summary>
        /// Perform an tick in the system when the timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Clock_Elapsed(object sender, ElapsedEventArgs e)
        {

            Update();
        }

        static object _state = new object();

        /// <summary>
        /// Main loop of the airport
        /// </summary>
        /// <param name="planeToUpdate">Update only a specific airplane, not specifying this parameter will update all the planes</param>
        /// <returns>Information about each affected station</returns>
        private static List<Information> Update(Airplane planeToUpdate = null)
        {
            clock.Stop();
            var information = new List<Information>();
            if (planeToUpdate == null)
            {
                Parallel.For(1, 10, (i) =>
                {
                    if (Chain.GetPlaneInfo(i) != null)
                        information.Add(Chain.UpdateStation(i));

                });
            }
            else
            {
                var info = Chain.MovePlane(planeToUpdate.ID);
                information.Add(info);

            }
            clock.Start();
            return information;
        }
        

        /// <summary>
        /// Launches on program start
        /// </summary>
        public static void Start()
        {
            if (!started)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                AppDomain.CurrentDomain.SetData("DataDirectory", path);
                clock.Elapsed += Clock_Elapsed;
                dbClock.Elapsed += DbClock_Elapsed;
                using (var context=new AirportContext())
                {//First time run without database data will create empty stations and save them into the database
                    if (context.Slots.ToList().Count() < 9)
                    {
                        context.Slots.AddRange(Enumerable.Range(1,9).Select(x=>new SlotInfo(x,null,true,DateTime.MinValue)).ToList());
                        context.SaveChanges();
                    }
                }
                LoadState();
                //TODO:log start sequence
                new Information(-1, "The system has started successfully!", InfoCode.Started);
                dbClock.Start();
                clock.Start();
                started = true;
            }
        }

        /// <summary>
        /// Save logs to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DbClock_Elapsed(object sender, ElapsedEventArgs e)
        {
            dbClock.Stop();
            Information.SaveLogsToDB();
            dbClock.Start();
        }

        /// <summary>
        /// Loads an existing state from the database
        /// </summary>
        private static void LoadState()
        {
            //TODO:implement state load 
            using (var context=new AirportContext())
            {
                var slots = context.Slots.Include("Plane").OrderBy(x=>x.Station).ToList();
                Chain.Restore(slots);
            }
        }

        /// <summary>
        /// Refresh the timer
        /// </summary>
        private static void ResetTimer()
        {
            clock.Stop();
            clock.Start();
        }
    }
}
