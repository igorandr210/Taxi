using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class TripHistoryRouteNode
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime UpdateTime { get; set; }

        public Guid Id { get; set; }

        public Guid TripHistoryId { get; set; }

        public TripHistory TripHistory { get; set; }
    }
}
