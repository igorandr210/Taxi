using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class AdminResponse
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid AdminId { get; set; }

        public string IdentityId { get; set; }
    }
}
