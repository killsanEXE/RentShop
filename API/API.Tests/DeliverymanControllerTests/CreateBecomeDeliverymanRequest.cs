using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.DeliverymanControllerTests
{
    [Collection("Sequential")]
    public class CreateBecomeDeliverymanRequest : DeliverymanControllerDependencyProvider
    {
        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn200()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser user = new() { DeliverymanRequest = false };
            var userRoles = A.CollectionOfDummy<AppUserRole>(1).ToList();
            user.UserRoles = userRoles;

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            
            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as UserDTO;

            Assert.Equal(200, result?.StatusCode);
            Assert.True(resultDTO?.DeliverymanRequest);
        }

        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn404()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser? user = null;
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user)!);

            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn400()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser user = new() { DeliverymanRequest = true };
            var userRoles = A.CollectionOfDummy<AppUserRole>(2).ToList();
            user.UserRoles = userRoles;

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            
            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}