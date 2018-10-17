using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Trips
{
    public class TripStatusDto
    {
        public DateTime CreationTime { get; set; }

        public DateTime? DriverTakeTripTime { get; set; }

        public DateTime? StartTime { get; set; }

        public Guid? DriverId { get; set; }

        public Guid CustomerId { get; set; }

        public PlaceDto From { get; set; }

        public PlaceDto To { get; set; }

        public double Distance { get; set; }

        public long Price { get; set; }

    }
}
