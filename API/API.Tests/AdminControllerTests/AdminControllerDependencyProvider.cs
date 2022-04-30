using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;

namespace API.Tests.AdminControllerTests
{
    public class AdminControllerDependencyProvider : DependencyProvider
    {
        protected readonly AdminController _adminController = null!;

        public AdminControllerDependencyProvider()
        {
            _adminController = new AdminController(_fakeUnitOfWork, _mapper);
        }
    }
}