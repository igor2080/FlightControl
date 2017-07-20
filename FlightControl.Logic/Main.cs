using FlightControl.Data;
using FlightControl.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FlightControl.Logic
{
    public static class Main
    {
        static Timer clock = new Timer(5000);
        static bool started = false;
        /// <summary>
        /// Perform an tick in the system when the timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Clock_Elapsed(object sender, ElapsedEventArgs e)
        {
            clock.Stop();//TODO: remove after testing
            Update();
        }


        /// <summary>
        /// Main loop of the airport
        /// </summary>
        /// <param name="planeToUpdate">Update only a specific airplane, not specifying this parameter will update all the planes</param>
        /// <returns>Information about each affected station</returns>
        private static List<Information> Update(Airplane planeToUpdate=null)
        {
            var information = new List<Information>();
            if (planeToUpdate==null)
            {
                for (int i = 1; i <= Chain.NumberOfStations; i++)
                {
                   information.Add(Chain.UpdateStation(i));
                }
            }
            else
            {
                var info = Chain.MovePlane(planeToUpdate.ID);
                information.Add(info);
                
            }
            return information;
        }


        /// <summary>
        /// Launches on program start
        /// </summary>
        public static void Start()
        {
            if (!started)
            {
                clock.Elapsed += Clock_Elapsed;
                Chain.InitializeChain();
                LoadState(); //TODO:implement state load
                var plane = Simulation.GeneratePlane(true);
                Chain.AcceptPlane(plane);
                //TODO:log start sequence
                clock.Start();
                started = true;
            }
        }

        /// <summary>
        /// Loads an existing state from the database
        /// </summary>
        private static void LoadState()
        {//temporary testing
            Chain.AcceptPlane(Simulation.GeneratePlane(true));
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
