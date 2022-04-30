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
    public class GetClientOrders : OrderControllerDependencyProvider
    {
        [Fact]
        public async void GetClientOrdersReturn200()
        {
            AppUser user = new() { UserName = "USERNAME" };
            var fakeOrders = A.CollectionOfDummy<OrderDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetClientOrdersAsync(user.UserName))
                .Returns(Task.FromResult(fakeOrders));

            var actionResult = await _orderController.GetClientOrders();

            var result = actionResult.Result as OkObjectResult;
            var resultDTOs = result?.Value as IEnumerable<OrderDTO>;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(fakeOrders.Count(), resultDTOs?.Count());
        }

        [Fact]
        public async void GetClientOrdersReturn404()
        {
            AppUser? user = null;
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user)!);

            var actionResult = await _orderController.GetClientOrders();

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }
    }
}