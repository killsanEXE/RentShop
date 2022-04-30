using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using FakeItEasy;

namespace API.Tests.ItemControllerTests
{
    public class ItemControllerDependencyProvider : DependencyProvider
    {
        protected ClaimsPrincipal _fakeUser = null!;
        protected readonly ItemController _itemController = null!;

        public ItemControllerDependencyProvider()
        {
            _itemController = new ItemController(_fakeUnitOfWork, _mapper, _fakePhotoService, _wrapper);
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