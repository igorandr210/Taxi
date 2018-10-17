using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models
{
    public class DriverLocationDto
    {
        public Guid DriverId { get; set; }
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
