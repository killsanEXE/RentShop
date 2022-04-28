using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IWrapper
    {
        string GetUsernameViaWrapper(ClaimsPrincipal user);
        void AddPaginationHeaderViaWrapper(HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages);
    }
}