using FlightControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Simulator
{
    public static class Simulation
    {
        /// <summary>
        /// Stores a value for random airplane generation
        /// </summary>
        private static Random _seed = new Random();

        /// <summary>
        /// Creates a plane
        /// </summary>
        /// <param name="planes">A relevant list of existing planes</param>
        /// <param name="IsLanding">Whether the generated plane is landing or departing</param>
        public static Airplane GeneratePlane( bool IsLanding)
        {
            return new Airplane(
                _seed.Next(100000, 999999)
                , IsLanding,
                DateTime.Now);            
        }

    }
}
