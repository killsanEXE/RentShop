using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using FakeItEasy;

namespace API.Tests.OrderControllerTests
{
    public class OrderControllerDependencyProvider : DependencyProvider
    {
        protected ClaimsPrincipal _fakeUser = null!;
        protected readonly OrderController _orderController = null!;
        public OrderControllerDependencyProvider()
        {
            _orderController = new OrderController(_fakeUnitOfWork, _mapper, _fakeEmailService, _wrapper);
            _fakeUser = A.Fake<ClaimsPrincipal>(f => f.WithArgumentsForConstructor(() => 
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Username"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"))
            ));
        }
    }
}