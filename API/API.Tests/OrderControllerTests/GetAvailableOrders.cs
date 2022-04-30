using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.OrderControllerTests
{
    [Collection("Sequential")]
    public class GetAvailableOrders : OrderControllerDependencyProvider
    {
        [Fact]
        public async void GetAvailableOrdersReturn200()
        {
            AppUser user = new();
            var fakeOrders = A.CollectionOfDummy<OrderDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetAvailableOrdersAsync(user))
                .Returns(Task.FromResult(fakeOrders));

            var actionResult = await _orderController.GetAvailableOrders();

            var result = actionResult.Result as OkObjectResult;
            var resultDTOs = result?.Value as IEnumerable<OrderDTO>;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(fakeOrders.Count(), resultDTOs?.Count());
        }

        [Fact]
        public async void GetAvailableOrdersReturn401()
        {
            AppUser? user = null;
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user)!);

            var actionResult = await _orderController.GetAvailableOrders();

            var result = actionResult.Result as UnauthorizedResult;
            Assert.Equal(401, result?.StatusCode);
        }
    }
}