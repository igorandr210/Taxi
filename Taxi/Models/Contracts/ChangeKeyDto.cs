using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Contracts
{
    public class ChangeKeyDto
    {
        [Required]
        [StringLength(64, MinimumLength = 64)]
        public string PrivateKey { get; set; }
    }
}
