using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class RefundRequestDto
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public bool Solved { get; set; }

        public Guid CustomerId { get; set; }

        public Guid IdentityId { get; set; }

        public Guid TripHistoryId { get; set; }

        public string Response { get; set; }
    }
}
