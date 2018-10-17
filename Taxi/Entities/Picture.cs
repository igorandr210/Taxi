using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class Picture
    {
        public string Id { get; set; } 

        public Guid VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}
