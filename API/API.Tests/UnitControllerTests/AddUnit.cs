using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.UnitControllerTests
{
    [Collection("Sequential")]
    public class AddUnit : UnitControllerDependencyProvider
    {
        [Fact]
        public async void AddUnitReturn200()
        {
            Item _fakeItem = new() { Id = 1 };
            UnitDTO _fakeUnitDTO = new()
            {
                PointId = 1,
                Description = "Dodge challenger unit"
            };
            Point _fakePoint = new() { Id = 1 };
            var itemId = 1;
            Unit fakeUnit = new() { Description = _fakeUnitDTO.Description };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(_fakeItem));
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(_fakeUnitDTO.PointId))
                .Returns(Task.FromResult(_fakePoint));
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.AddUnit(_fakeItem, _fakePoint, fakeUnit)).DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _unitController.AddUnit(itemId, _fakeUnitDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultUnit = result?.Value as UnitDTO;
            Assert.Equal(_fakeUnitDTO.Description, resultUnit?.Description);
        }

        [Fact]
        public async void AddUnitReturn404()
        {
            Item _fakeItem = new() { Id = 1 };
            UnitDTO _fakeUnitDTO = new()
            {
                PointId = 1,
                Description = "Dodge challenger unit"
            };
            Point _fakePoint = new() { Id = 1 };
            var itemId = 1;
            Item fakeItem = new() { Id = -1 };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(fakeItem));
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(_fakeUnitDTO.PointId))
                .Returns(Task.FromResult(_fakePoint));

            var actionResult = await _unitController.AddUnit(itemId, _fakeUnitDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void AddUnitReturn400()
        {
            Item _fakeItem = new() { Id = 1 };
            UnitDTO _fakeUnitDTO = new()
            {
                PointId = 1,
                Description = "Dodge challenger unit"
            };
            Point _fakePoint = new() { Id = 1 };
            var itemId = 1;
            Unit fakeUnit = new() { Description = _fakeUnitDTO.Description };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(_fakeItem));
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(_fakeUnitDTO.PointId))
                .Returns(Task.FromResult(_fakePoint));
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.AddUnit(_fakeItem, _fakePoint, fakeUnit)).DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _unitController.AddUnit(itemId, _fakeUnitDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}