using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public List<string> Roles{ get; set; } = new List<string>();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePictureId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber{ get; set; }

        public bool EmailConfirmed { get; set; }
        
        public Dictionary<string, string> Ids { get; set; } = new Dictionary<string, string>();
    }
}
