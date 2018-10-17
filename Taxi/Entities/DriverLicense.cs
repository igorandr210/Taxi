using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class DriverLicense
    {
        public Guid Id { get; set; }
        
        public DateTime LicensedFrom { get; set; }

        public DateTime LicensedTo { get; set; }

        public string ImageId { get; set; }

        public DateTime UpdateTime { get; set; }
        
        public Driver Driver { get; set; }

        public Guid DriverId { get; set; }

        public bool IsApproved { get; set; } 
    }
}
