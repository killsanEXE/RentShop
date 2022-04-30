using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.PointControllerTests
{
    [Collection("Sequential")]
    public class GetPoints : PointControllerDpendencyProvider
    {
        [Fact]
        public async void GetPointsReturn200()
        {
            var fakePoints = A.CollectionOfDummy<PointDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetDTOPointsAsync())
                .Returns(Task.FromResult(fakePoints));

            var actionResult = await _pointController.GetPoints();

            var result = actionResult.Result as OkObjectResult;
            var resultDTOs = result?.Value as IEnumerable<PointDTO>;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(fakePoints.Count(), resultDTOs?.Count());
        }
    }
}