using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Drivers
{
    public class DriverLicenseResourceParameters : PaginationParameters
    {
        public bool? IsApproved { get; set; }
    }
}
