using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Trips
{
    public class TripDto
    {
        public PlaceDto From  { get; set; }

        public PlaceDto To { get; set; }
        
        public Guid  CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
    }
}
