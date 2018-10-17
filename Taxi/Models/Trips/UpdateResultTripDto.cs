using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Trips
{
    public class UpdateResultTripDto
    {
        public PlaceDto From { get; set; }

        public PlaceDto To { get; set; }

        public Guid CustomerId { get; set; }

     //   public double TraveledDistance { get; set; }
        
        public PlaceDto LastUpdatePoint { get; set; }
    }
}
