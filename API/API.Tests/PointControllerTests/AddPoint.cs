using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace API.Tests.PointControllerTests
{
    [Collection("Sequential")]
    public class AddPoint : PointControllerDpendencyProvider
    {
        [Fact]
        public async void AddPointReturn200()
        {
            LocationDTO locationDTO = new();
            A.CallTo(() => _fakeUnitOfWork.PointRepository.AddPoint(new Point())).DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _pointController.AddPoint(locationDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as PointDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }

        [Fact]
        public async void AddPointReturn400()
        {
            LocationDTO locationDTO = new();
            A.CallTo(() => _fakeUnitOfWork.PointRepository.AddPoint(new Point())).DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _pointController.AddPoint(locationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}