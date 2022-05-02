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
    public class ReturnOrder : OrderControllerDependencyProvider
    {
        [Fact]
        public async void ReturnOrderReturn200_1()
        {
            AppUser user = new() 
            { 
                Id = 1, 
                UserRoles = A.CollectionOfDummy<AppUserRole>(1).ToList(),
                DeliveryLocations = A.CollectionOfDummy<Location>(3).ToList(),
            };
            user.DeliveryLocations.Add(new() { Id = 1, Country = "Belarus" });
            Order order = new() { Id = 1 };
            ReturnUnitDTO dto = new() 
            {
                Id = 1,
                ReturnFromLocation = "1",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(dto.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetDeliverymansFromCountryAsync("Belarus"))
                .Returns(A.CollectionOfDummy<AppUser>(0).AsEnumerable());

            var actionResult = await _orderController.ReturnOrder(dto);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(order.Id, resultDTO?.Id);
        }

        [Fact]
        public async void ReturnOrderReturn200_2()
        {
            AppUser user = new() { Id = 1, UserRoles = A.CollectionOfDummy<AppUserRole>(1).ToList() };
            Order order = new() { Id = 1 };
            ReturnUnitDTO dto = new() 
            {
                Id = 1,
                ReturnPoint = "1",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(dto.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(1))
                .Returns(Task.FromResult(new Point()));

            var actionResult = await _orderController.ReturnOrder(dto);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(order.Id, resultDTO?.Id);
        }

        [Fact]
        public async void ReturnOrderReturn404()
        {
            AppUser user = new() { Id = 1, UserRoles = A.CollectionOfDummy<AppUserRole>(1).ToList() };
            Order order = new() { Id = -1 };
            ReturnUnitDTO dto = new() 
            {
                Id = 1,
                ReturnPoint = "1",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id))
                .Returns(Task.FromResult(order));

            var actionResult = await _orderController.ReturnOrder(dto);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void ReturnOrderReturn400_1()
        {
            AppUser user = new() { Id = 1, UserRoles = A.CollectionOfDummy<AppUserRole>(1).ToList() };
            Order order = new() { Id = 1 };
            ReturnUnitDTO dto = new() { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(true);

            var actionResult = await _orderController.ReturnOrder(dto);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void ReturnOrderReturn400_2()
        {
            AppUser user = new() { Id = 1, UserRoles = A.CollectionOfDummy<AppUserRole>(1).ToList() };
            Order order = new() { Id = 1 };
            ReturnUnitDTO dto = new() 
            {
                Id = 1,
                ReturnPoint = "1",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id))
                .Returns(Task.FromResult(order));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetOrderDTOByIdAsync(dto.Id))
                .Returns(new OrderDTO() { Id = order.Id });
            A.CallTo(() => _fakeUser.IsInRole("Deliveryman")).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(1))
                .Returns(Task.FromResult(new Point()));

            var actionResult = await _orderController.ReturnOrder(dto);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}