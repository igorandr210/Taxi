using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Trips
{
    public class RouteNodeDto
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
