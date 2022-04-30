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
    public class CreateOrder : OrderControllerDependencyProvider
    {
        [Fact]
        public async void CreateOrderReturn200()
        {
            CreateOrderDTO createOrderDTO = new() 
            { 
                UnitId = 1, 
                DeliveryLocation = 1,
                DeliveryDate = new DateTime(),
                ReturnDate = new DateTime()
            };
            Unit unit = new() { Id = 1, IsAvailable = true };

            var fakeClientDeliveryLocations = A.CollectionOfDummy<Location>(1).ToList();
            fakeClientDeliveryLocations.Add(new() { Id = 1, Country = "Belarus" });
            var fakeClientRoles = A.CollectionOfDummy<AppUserRole>(1).ToList();
            AppUser client = new() { DeliveryLocations = fakeClientDeliveryLocations, UserRoles = fakeClientRoles};


            A.CallTo(() => _fakeUser.IsInRole("Admin")).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(client));
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(createOrderDTO.UnitId))
                .Returns(Task.FromResult(unit));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.AddOrder(new Order())).DoesNothing();

            _orderController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fakeUser }
            };

            var actionResult = await _orderController.CreateOrder(createOrderDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as OrderDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal
            (
                client.DeliveryLocations.SingleOrDefault(f => f.Id == 1)!.Country, 
                resultDTO?.DeliveryLocation?.Country
            );
        }
    }
}