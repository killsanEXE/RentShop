using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.DeliverymanControllerTests
{
    [Collection("Sequential")]
    public class AddDeliveryMan : DeliverymanControllerDependencyProvider
    {
        [Fact]
        public async void AddDeliveryManReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = true, Location = new() };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUserManager.AddToRoleAsync(user, "Deliveryman"))
                .Returns(Task.FromResult(new IdentityResult()));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage())).Returns(Task.FromResult(true));

            var actionResult = await _deliverymanController.AddDeliveryMan("USERNAME");

            var result = (actionResult.Result as OkObjectResult)?.Value as DeliverymanDTO;
            Assert.Equal(user.UserName, result?.Username);            
        }

        [Fact]
        public async void AddDeliveryManReturn404()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));

            
            var actionResult = await _deliverymanController.AddDeliveryMan("USERNAME");

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);    
        }
    }
}