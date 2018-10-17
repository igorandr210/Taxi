using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;

namespace Taxi.Models.Trips
{
    public class TripUpdateDto
    {
        public PlaceDto From { get; set; }
       
        public PlaceDto To { get; set; }
    }
}
