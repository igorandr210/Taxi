using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Drivers
{
    public class DriverUpdateDto
    {
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        public string City { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}
