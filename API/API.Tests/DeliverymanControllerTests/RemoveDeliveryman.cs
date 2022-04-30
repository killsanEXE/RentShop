using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.DeliverymanControllerTests
{
    [Collection("Sequential")]
    public class RemoveDeliveryman : DeliverymanControllerDependencyProvider
    {
        [Fact]
        public async void RemoveDeliverymanReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            var fakeActiveOrders = A.CollectionOfDummy<Order>(0).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetActiveDeliveriesForDeliverymanAsync("USERNAME"))
                .Returns(Task.FromResult(fakeActiveOrders));
            A.CallTo(() => _fakeUserManager.RemoveFromRoleAsync(user, "Deliveryman"))
                .Returns(Task.FromResult(new IdentityResult()));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _deliverymanController.RemoveDeliveryman("USERNAME");

            var result = actionResult as OkResult;
            Assert.Equal(200, result?.StatusCode); 
        }

        [Fact]
        public async void RemoveDeliverymanReturn400()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            var fakeActiveOrders = A.CollectionOfDummy<Order>(1).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetActiveDeliveriesForDeliverymanAsync("USERNAME"))
                .Returns(Task.FromResult(fakeActiveOrders));

            var actionResult = await _deliverymanController.RemoveDeliveryman("USERNAME");

            var result = actionResult as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode); 
        }
    }
}