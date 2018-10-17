using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models
{
    public class TokensDto
    {
        public string auth_token { get; set; }

        public string refresh_token { get; set; }

        public int expires_in { get; set; }
    }
}
