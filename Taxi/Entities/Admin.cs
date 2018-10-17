using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }

        public string IdentityId { get; set; }

        public AppUser Identity { get; set; }

        public bool IsApproved { get; set; }
        
    }
}
