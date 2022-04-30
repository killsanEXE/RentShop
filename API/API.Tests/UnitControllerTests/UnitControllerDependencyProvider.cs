using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;

namespace API.Tests.UnitControllerTests
{
    public class UnitControllerDependencyProvider : DependencyProvider
    {
        protected readonly UnitController _unitController = null!;

        public UnitControllerDependencyProvider()
        {
            _unitController = new UnitController(_fakeUnitOfWork, _mapper);
        }
    }
}