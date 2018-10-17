using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Drivers
{
    public class DriverLicenseDto
    {
        public DateTime LicensedFrom { get; set; }

        public DateTime LicensedTo { get; set; }

        public bool IsApproved { get; set; }

        public Guid DriverId { get; set; }
    }
}
