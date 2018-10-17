using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public List<Picture> Pictures { get; set; } = new List<Picture>();

        public string Number { get; set; }

        public string Model { get; set; }

        public string Brand { get; set; }

        public string Color { get; set; }

        public Guid DriverId { get; set; }

        public Driver Driver { get; set; }
    }
}
