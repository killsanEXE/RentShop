using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.DeliverymanControllerTests
{
    [Collection("Sequential")]
    public class GetAllDeliverymans : DeliverymanControllerDependencyProvider
    {
        [Fact]
        public async void GetAllDeliverymansReturn200()
        {
            var fakeDeliverymans = A.CollectionOfDummy<DeliverymanDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetDeliverymansAsync())
                .Returns(Task.FromResult(fakeDeliverymans));

            var actionResult = await _deliverymanController.GetAllDeliverymans();

            var result = (actionResult.Result as OkObjectResult)?.Value as IEnumerable<DeliverymanDTO>;
            Assert.Equal(fakeDeliverymans.Count(), result?.Count());
        }
    }
}