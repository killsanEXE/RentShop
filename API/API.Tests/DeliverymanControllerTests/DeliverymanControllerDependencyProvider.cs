using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;

namespace API.Tests.DeliverymanControllerTests
{
    public class DeliverymanControllerDependencyProvider : DependencyProvider
    {
        protected readonly DeliverymanController _deliverymanController = null!;

        public DeliverymanControllerDependencyProvider()
        {
            _deliverymanController = new DeliverymanController(_fakeUnitOfWork, 
                _mapper, _fakeUserManager, _fakeEmailService, _wrapper);
        }
    }
}