using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Drivers
{
    public class LicenseCreationDto
    {
        [Required]
        public int DayFrom { get; set; }
        [Required]
        public int YearFrom { get; set; }
        [Required]
        public int MonthFrom { get; set; }
        [Required]
        public int DayTo { get; set; }
        [Required]
        public int YearTo { get; set; }
        [Required]
        public int MonthTo { get; set; }

    }
}
