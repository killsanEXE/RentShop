using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;

namespace API.Wrappers
{
    public class Wrapper : IWrapper
    {
        public string GetUsernameViaWrapper(ClaimsPrincipal user)
        {
            return user.GetUsername();
        }

        public void AddPaginationHeaderViaWrapper(HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            response.AddPaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
        }
    }
}