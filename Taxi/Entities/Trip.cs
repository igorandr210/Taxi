using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace Taxi.Entities
{
    public class Trip
    {
        public long ContractId { get; set; }

        public Guid Id { set; get; }

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid? DriverId { get; set; }

        public Driver Driver{ get; set; }
        
        public Point From { get; set; }

        public Point To { get; set; }

        public List<TripRouteNode> RouteNodes { get; set; } = new List<TripRouteNode>();

        public double LastLat { get; set; }

        public double LastLon { get; set; }

        public double Distance { get; set; }

        public long Price { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime DriverTakeTripTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }
    }
}
 