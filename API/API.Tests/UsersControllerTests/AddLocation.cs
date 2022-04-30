using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.UsersControllerTests
{
    [Collection("Sequential")]
    public class AddLocation : UsersControllerDependencyProvider
    {
        [Fact]
        public async void AddLocationReturn200()
        {
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new());
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            _usersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fakeUser }
            };

            var actionResult = await _usersController.AddLocation(fakeLocationDTO);

            var result = actionResult.Value;
            Assert.Equal(fakeLocationDTO.Country, result?.Country);
        }        

        [Fact]
        public async void AddLocationReturn400()
        {
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            _usersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fakeUser }
            };

            var actionResult = await _usersController.AddLocation(fakeLocationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}