using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.DeliverymanControllerTests
{
    [Collection("Sequential")]
    public class DenyDeliveryman : DeliverymanControllerDependencyProvider  
    {
        [Fact]
        public async void DenyDeliverymanReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = true, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage())).Returns(Task.FromResult(true));
            
            var actionResult = await _deliverymanController.DenyDeliveryMan("USERNAME");

            var result = actionResult.Result as OkResult;
            Assert.Equal(200, result?.StatusCode);
        }

        [Fact]
        public async void DenyDeliverymanReturn404()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            
            var actionResult = await _deliverymanController.DenyDeliveryMan("USERNAME");

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }
    }
}