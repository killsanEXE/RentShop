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
    public class GetAllJoinRequests : DeliverymanControllerDependencyProvider
    {
        [Fact]
        public async void GetAllJoinRequestsReturn200()
        {
            var fakeDeliverymans = A.CollectionOfDummy<DeliverymanDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetDeliverymanRequestsAsync())
                .Returns(Task.FromResult(fakeDeliverymans));

            var actionResult = await _deliverymanController.GetAllJoinRequests();

            var result = (actionResult.Result as OkObjectResult)?.Value as IEnumerable<DeliverymanDTO>;
            Assert.Equal(fakeDeliverymans.Count(), result?.Count());
        }
    }
}