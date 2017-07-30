using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Data
{
    public class AirportContext : DbContext
    {
        public DbSet<Information> Logs { get; set; }

        public DbSet<SlotInfo> Slots { get; set; }
        //TODO: everything db related
    }
}
