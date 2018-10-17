using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class RefundRequest
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public bool Solved { get; set; }

        public Guid CustomerId { get; set; }

        public string IdentityId { get; set; }

        public Guid TripHistoryId { get; set; }

        public Guid AdminResponseId { get; set; }
    }
}
