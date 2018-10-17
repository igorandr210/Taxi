using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class ComissionDto
    {
        [Range(0, 99,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public ulong Value { get; set; }
    }
}
