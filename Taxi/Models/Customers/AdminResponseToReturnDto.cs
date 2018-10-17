using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Taxi.Models.Customers
{
    public class AdminResponseToReturnDto
    {
        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public string Email { get; set; }
    }
}
