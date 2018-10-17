using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models
{
    public class LatLonDto
    {
        [Required]
        [Range(-90.0, 90.0,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Longitude { get; set; }
    }
}
