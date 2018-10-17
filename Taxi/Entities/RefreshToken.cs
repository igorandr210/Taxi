using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }

        public string IdentityId { get; set; }

        public long Expiration { get; set; }

        public string Ip { get; set; }

        public string Useragent { get; set; }

        public AppUser Identity { get; set; }
    }
}
