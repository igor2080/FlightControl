using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Data
{
    public class Airplane
    {
        /// <summary>
        /// Plane number
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Has the plane landed, or is taking off?
        /// </summary>
        public bool Landing { get; set; }

        /// <summary>
        /// How long the plane has existed within the system
        /// </summary>
        public DateTime LifeSpan { get; set; }

        /// <summary>
        /// Represents whether the plane is working or broken, broken airplanes are unable to move
        /// </summary>
        public bool IsWorking { get; set; }

        public Airplane(int Id, bool IsLanding, DateTime Lifespan)
        {
            this.ID = Id;
            this.Landing = IsLanding;
            this.LifeSpan = Lifespan;
            IsWorking = true;
        }

        
    }
}
