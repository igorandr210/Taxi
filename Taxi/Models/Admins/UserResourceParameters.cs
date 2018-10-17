using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class UserResourceParameters: PaginationParameters
    {
        public string Rol { get; set; }

        public string SearchQuery { get; set; }

        public bool? EmailConfirmed { get; set; } = null;
    }
}
