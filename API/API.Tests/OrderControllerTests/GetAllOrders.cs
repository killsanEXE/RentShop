using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.OrderControllerTests
{
    [Collection("Sequential")]
    public class GetAllOrders : OrderControllerDependencyProvider
    {
        [Fact]
        public async void GetAllOrdersReturn200()
        {
            UserParams userParams = new() { showAll = true };
            var fakeOrders = A.CollectionOfDummy<OrderDTO>(5).AsEnumerable();
            var fakePagedList = new PagedList<OrderDTO>(fakeOrders, fakeOrders.Count(), 1, 3) { TotalPages = 1 };
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrdersAsync(userParams, userParams.showAll))
                .Returns(Task.FromResult(fakePagedList));

            var actionResult = await _orderController.GetAllOrders(userParams);

            var result = actionResult.Result as OkObjectResult;
            var resultDTOs = result?.Value as IEnumerable<OrderDTO>;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(fakeOrders.Count(), resultDTOs?.Count());
        }
    }
}