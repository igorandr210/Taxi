using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class RefundSolutionDto
    {
        [Required]
        public bool ToRefund { get; set; } = false;
        [Required]
        public string  Message { get; set; }
    }
}
