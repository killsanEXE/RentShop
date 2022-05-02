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
    public class CancelOrder : OrderControllerDependencyProvider
    {
        [Fact]
        public async void CancelledReturn200()
        {
            AppUser user = new() { Id = 1 };
            Unit unit = new();
            Order order = new() 
            { 
                Id = 1, 
                DeliveryInProcess = false,
                Cancelled = false,
                Client = user,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            
            var actionResult = await _orderController.CancelOrder(order.Id);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;

            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(order.Id, resultDTO?.Id);
        }

        [Fact]
        public async void CancelledReturn404()
        {
            AppUser user = new() { Id = 1 };
            Unit unit = new();
            Order order = new() 
            { 
                Id = 1, 
                DeliveryInProcess = false,
                ClientGotDelivery = true,
                Cancelled = false,
                Client = user,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            
            var actionResult = await _orderController.CancelOrder(order.Id);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void CancelledReturn400_1()
        {
            AppUser user = new() { Id = 1 };
            Unit unit = new();
            Order order = new() 
            { 
                Id = 1, 
                DeliveryInProcess = true,
                Cancelled = false,
                Client = user,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            
            var actionResult = await _orderController.CancelOrder(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void CancelledReturn400_2()
        {
            AppUser user = new() { Id = 1 };
            Unit unit = new();
            Order order = new() 
            { 
                Id = 1, 
                DeliveryInProcess = false,
                Cancelled = false,
                Client = user,
                Unit = unit
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            
            var actionResult = await _orderController.CancelOrder(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}