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
    public class EditLocation : UsersControllerDependencyProvider
    {
        [Fact]
        public async void EditLocationReturn400()
        {
            var locationId = 1;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new()
            {
                Id = 1,
                Country = "Belarus",
                City = "Minsk"
            });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _usersController.EditLocation(locationId, fakeLocationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);           
        }

        [Fact]
        public async void EditLocationReturn404()
        {
            var locationId = 1;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));

            var actionResult = await _usersController.EditLocation(locationId, fakeLocationDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);   
        }
    }
}