using API.DTOs;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.UsersControllerTests
{
    [Collection("Sequential")]
    public class GetUsers : UsersControllerDependencyProvider
    {

        [Fact]
        public async void GetUsersReturn200()
        {
            var fakeUsers = A.CollectionOfDummy<ClientDTO>(3).AsEnumerable();
            UserParams fakeUserParams = new();
            var fakePagedList = new PagedList<ClientDTO>(fakeUsers, fakeUsers.Count(), 1, 3) { TotalPages = 1 };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetClientsAsync(fakeUserParams))
                .Returns(Task.FromResult(fakePagedList));

            var actionResult = await _usersController.GetUsers(fakeUserParams);

            var result = actionResult.Result as OkObjectResult;
            var resultUsers = result?.Value as IEnumerable<ClientDTO>;
            Assert.Equal(fakeUsers.Count(), resultUsers?.Count());
        }
        
    }
}