using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models
{
    public class RestorePasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

      //  [Required(ErrorMessage = "You must provide a phone number")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
