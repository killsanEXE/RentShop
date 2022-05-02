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
    public class ConfirmReturn : OrderControllerDependencyProvider
    {
        [Fact]
        public async void ConfirmReturnReturn200()
        {
            Unit unit = new()
            {
                Id = 1,
                ItemUnitPoint = new() { Point = new() }
            };
            Order order = new()
            { 
                Id = 1,
                ReturnDeliveryman = new(),
                ReturnPoint = new(),
                ClientGotDelivery = true,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unit.Id))
                .Returns(Task.FromResult(unit));

            var actionResult = await _orderController.ConfirmReturn(order.Id);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(order.Id, resultDTO?.Id);
        }

        [Fact]
        public async void ConfirmReturnReturn400_1()
        {
            Order order = new() { Id = -1 };

            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));

            var actionResult = await _orderController.ConfirmReturn(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void ConfirmReturnReturn400_2()
        {
            Order order = new() { Id = -1 };
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));

            var actionResult = await _orderController.ConfirmReturn(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void ConfirmReturnReturn400_3()
        {
            Unit unit = new()
            {
                Id = -1,
                ItemUnitPoint = new() { Point = new() }
            };
            Order order = new()
            { 
                Id = 1,
                ReturnDeliveryman = new(),
                ReturnPoint = new(),
                ClientGotDelivery = true,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unit.Id))
                .Returns(Task.FromResult(unit));

            var actionResult = await _orderController.ConfirmReturn(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void ConfirmReturnReturn400_4()
        {
            Unit unit = new()
            {
                Id = 1,
                ItemUnitPoint = new() { Point = new() }
            };
            Order order = new()
            { 
                Id = 1,
                ReturnDeliveryman = new(),
                ReturnPoint = new(),
                ClientGotDelivery = true,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unit.Id))
                .Returns(Task.FromResult(unit));

            var actionResult = await _orderController.ConfirmReturn(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}