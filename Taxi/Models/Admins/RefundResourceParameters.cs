using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Admins
{
    public class RefundResourceParameters: PaginationParameters
    {
        public bool? IsSolved { get; set; }
    }
}
