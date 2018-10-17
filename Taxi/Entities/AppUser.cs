using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ProfilePicture ProfilePicture { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }
        
        public string  PrivateKey { get; set; }

        public List<AdminResponse> AdminResponces { get; set; } = new List<AdminResponse>();
    }
}
