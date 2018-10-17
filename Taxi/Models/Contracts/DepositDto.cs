using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Contracts
{
    
    public class DepositDto
    {
        [Required]
        public ulong Value { get; set; }
    }
}
