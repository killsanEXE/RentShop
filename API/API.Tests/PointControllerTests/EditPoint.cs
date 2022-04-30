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
    public class EditPoint : PointControllerDpendencyProvider
    {
        [Fact]
        public async void EditPointReturn200()
        {
            int pointId = 1;
            Point point = new() { Country = "Belarus", City = "Minsk", Address = "some" };
            LocationDTO locationDTO = new() { Country = "USA", City = "LA", Address = "some" };

            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(pointId))
                .Returns(Task.FromResult(point));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _pointController.EditPoint(pointId, locationDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as PointDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }

        [Fact]
        public async void EditPointReturn404()
        {
            int pointId = 1;
            Point? point = null;
            LocationDTO locationDTO = new();

            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(pointId))
                .Returns(Task.FromResult(point)!);

            var actionResult = await _pointController.EditPoint(pointId, locationDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void EditPointReturn400()
        {
            int pointId = 1;
            LocationDTO locationDTO = new();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _pointController.EditPoint(pointId, locationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}