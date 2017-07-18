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
        public byte ID { get; set; }

        /// <summary>
        /// Has the plane landed, or is taking off?
        /// </summary>
        public bool Landing { get; set; }

        /// <summary>
        /// How long the plane has existed within the system
        /// </summary>
        public DateTime LifeSpan { get; set; }

        public Airplane(byte Id, bool IsLanding, DateTime Lifespan)
        {
            this.ID = Id;
            this.Landing = IsLanding;
            this.LifeSpan = Lifespan;
        }

        
    }
}
