using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Drivers
{
    public class VehicleToReturnDto
    {
        public Guid Id { get; set; }
        
        public string Number { get; set; }

        public string Model { get; set; }

        public string Brand { get; set; }

        public string Color { get; set; }

        public List<string> Pictures { get; set; }

        public Guid DriverId { get; set; }
    }
}
