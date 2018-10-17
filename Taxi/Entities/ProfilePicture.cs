using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class ProfilePicture
    {
        public string Id { get; set; }

        public string IdentityId { get; set; }

        public AppUser Identity { get; set; }

    }
}
