using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;

namespace API.Tests.PointControllerTests
{
    public class PointControllerDpendencyProvider : DependencyProvider
    {
        protected readonly PointController _pointController = null!;

        public PointControllerDpendencyProvider()
        {
            _pointController = new PointController(_fakeUnitOfWork, _mapper, _fakePhotoService);
        }
    }
}