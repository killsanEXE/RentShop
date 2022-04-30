using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using FakeItEasy;

namespace API.Tests.UsersControllerTests
{
    public class UsersControllerDependencyProvider : DependencyProvider
    {
        protected readonly UsersController _usersController = null!;
        protected ClaimsPrincipal _fakeUser = null!;

        public UsersControllerDependencyProvider()
        {
            _fakeUser = A.Fake<ClaimsPrincipal>(f => f.WithArgumentsForConstructor(() => 
                    new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Username"),
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim("custom-claim", "example claim value"),
                    }, "mock"))
                ));
            _usersController = new UsersController(_fakePhotoService, _fakeUnitOfWork, _mapper, _wrapper);
        }
    }
}