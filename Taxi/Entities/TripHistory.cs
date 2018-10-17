using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace Taxi.Entities
{
    public class TripHistory
    {
        public long ContractId { get; set; }
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid DriverId { get; set; }

        public Driver Driver { get; set; }

        public Point From { get; set; }

        public Point To { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime DriverTakeTripTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public long Price { get; set; }

        public double Distance { get; set; }

        public List<TripHistoryRouteNode> TripHistoryRouteNodes { get; set; } = new List<TripHistoryRouteNode>();
    }
}
