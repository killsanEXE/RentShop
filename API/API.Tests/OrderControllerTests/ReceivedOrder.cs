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
    public class ReceivedOrder : OrderControllerDependencyProvider
    {
        [Fact]
        public async void ReceivedOrderReturn200()
        {
            AppUser user = new() { Id = 1 };
            Order order = new() 
            { 
                Id = 1, 
                DeliveryCompleted = true,
                DeliveryInProcess = true,
                Cancelled = false,
                Client = user,
                DeliveryMan = user
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage(order.Client.Email, "", "")))
                .Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(false);

            var actionResult = await _orderController.ReceivedOrder(order.Id);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(order.Id, resultDTO?.Id);
        }

        [Fact]
        public async void ReceivedOrderReturn404()
        {
            AppUser user = new() { Id = 1 };
            Order order = new() 
            { 
                Id = -1, 
                DeliveryCompleted = true,
                DeliveryInProcess = true,
                Cancelled = false,
                Client = user,
                DeliveryMan = user
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));

            var actionResult = await _orderController.ReceivedOrder(order.Id);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void ReceivedOrderReturn400_1()
        {
            AppUser user = new() { Id = 1 };
            Order order = new() 
            { 
                Id = 1, 
                DeliveryCompleted = false,
                DeliveryInProcess = true,
                Cancelled = false,
                Client = user,
                DeliveryMan = user
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));

            var actionResult = await _orderController.ReceivedOrder(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void ReceivedOrderReturn400_2()
        {
            AppUser user = new() { Id = 1 };
            Order order = new() 
            { 
                Id = 1, 
                DeliveryCompleted = true,
                DeliveryInProcess = true,
                Cancelled = false,
                Client = user,
                DeliveryMan = user
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(order.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage(order.Client.Email, "", "")))
                .Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(false);

            var actionResult = await _orderController.ReceivedOrder(order.Id);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

    }
}