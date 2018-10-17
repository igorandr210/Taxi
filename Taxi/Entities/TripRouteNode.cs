using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class TripRouteNode
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime UpdateTime { get; set; }

        public Guid Id { get; set; }

        public Guid TripId { get; set; }

        public Trip Trip { get; set; }
    }
}
