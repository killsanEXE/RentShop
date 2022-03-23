using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParams
    {
        const int MaxPageSize = 20;
        public int PageNumber { get; set; } = 1;
        int _pageSize = 5;
        public int PageSize { get => _pageSize; set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }
}