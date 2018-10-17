using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models
{
    public class PaginationParameters
    {
        private int _pageSize = 10;
        private const int maxPageSize = 100;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value < 1)? 1: value;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize || value < 1) ? maxPageSize : value;
        }
    }
}
