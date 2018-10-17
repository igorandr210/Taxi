using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;

namespace Taxi.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string ConnectionId { get; set; }

        public string  IdentityId { get; set; }

        public AppUser Identity { get; set; }

        public Trip CurrentTrip { get; set; }

        public List<TripHistory> TripHistories { get; set; } = new List<TripHistory>();

        public List<RefundRequest> RefundRequests { get; set; } = new List<RefundRequest>();
    }
}
