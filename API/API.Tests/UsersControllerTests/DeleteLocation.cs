using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.UsersControllerTests
{
    [Collection("Sequential")]
    public class DeleteLocation : UsersControllerDependencyProvider
    {
        [Fact]
        public async void DeleteLocationReturn200()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations.Add(new() { Id = 1 });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as OkResult;
            Assert.Equal(200, result?.StatusCode);
        }

        [Fact]
        public async void DeleteLocationReturn400()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations.Add(new() { Id = 1 });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void DeleteLocationReturn404()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }
    }
}