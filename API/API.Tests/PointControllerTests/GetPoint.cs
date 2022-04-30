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
    public class GetPoint : PointControllerDpendencyProvider
    {
        [Fact]
        public async void GetPointRetrun200()
        {
            int pointId = 1;
            PointDTO point = new();
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointDTO(pointId))
                .Returns(Task.FromResult(point));
            
            var actionResult = await _pointController.GetPoint(pointId);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as PointDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }

        [Fact]
        public async void GetPointReturn404()
        {
            int pointId = 1;
            PointDTO? point = null;
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointDTO(pointId))
                .Returns(Task.FromResult(point)!);
            
            var actionResult = await _pointController.GetPoint(pointId);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        
    }
}