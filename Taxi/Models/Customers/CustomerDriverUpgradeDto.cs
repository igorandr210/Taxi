using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Customers
{
    public class CustomerDriverUpgradeDto
    {
        [Required]
        public string City { get; set; }
    }
}
