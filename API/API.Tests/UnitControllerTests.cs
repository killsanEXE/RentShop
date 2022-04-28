using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests
{
    public class UnitControllerTests : DependencyProvider
    {
        readonly UnitController _controller = null!;
        public UnitControllerTests()
        {
            _controller = new UnitController(_fakeUnitOfWork, _mapper);
        }

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

            var controller = new UnitController(_fakeUnitOfWork, _mapper);
            var actionResult = await controller.AddUnit(itemId, _fakeUnitDTO);

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

            var controller = new UnitController(_fakeUnitOfWork, _mapper);
            var actionResult = await controller.AddUnit(itemId, _fakeUnitDTO);

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

            var controller = new UnitController(_fakeUnitOfWork, _mapper);
            var actionResult = await controller.AddUnit(itemId, _fakeUnitDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }



        
        [Fact]
        public async void EditUnitReturn200()
        {
            Point fakePoint = new() { Id = 1 };
            ItemUnitPoint fakeItemUnitPoint = new()
            {
                Id = 1,
                UnitId = 1,
                Point = fakePoint,
            };
            Unit fakeUnit = new() 
            { 
                Id = 1, 
                Description = "Dodge challenger unit",
                IsAvailable = true,
                ItemUnitPoint = fakeItemUnitPoint,
            };
            UnitDTO fakeUnitDTO = new() { Description = "Another description", PointId = 2 };
            int unitId = 1;
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unitId))
                .Returns(Task.FromResult(fakeUnit));
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(fakeUnitDTO.PointId))
                .Returns(Task.FromResult(fakePoint));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _controller.EditUnit(unitId, fakeUnitDTO);
            var editedUnit = actionResult.Value as UnitDTO;
            Assert.NotEqual(fakeUnit.Description, editedUnit?.Description);
        }

        [Fact]
        public async void EditUnitReturn404()
        {
            int unitId = 1;
            Unit fakeUnit = new() { Id = -1 };
            UnitDTO fakeUnitDTO = new();
            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unitId))
                .Returns(Task.FromResult(fakeUnit));

            var actionResult = await _controller.EditUnit(unitId, fakeUnitDTO);
            
            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void EditUnitReturn400()
        {
            Point fakePoint = new() { Id = 1 };
            ItemUnitPoint fakeItemUnitPoint = new()
            {
                Id = 1,
                UnitId = 1,
                Point = fakePoint,
            };
            Unit fakeUnit = new() 
            { 
                Id = 1, 
                Description = "Dodge challenger unit",
                IsAvailable = true,
                ItemUnitPoint = fakeItemUnitPoint,
            };

            int unitId = 1;
            UnitDTO fakeUnitDTO = new() { Description = fakeUnit.Description, PointId = fakePoint.Id };

            A.CallTo(() => _fakeUnitOfWork.UnitRepository.GetUnitByIdAsync(unitId))
                .Returns(Task.FromResult(fakeUnit));
            A.CallTo(() => _fakeUnitOfWork.PointRepository.GetPointByIdAsync(fakeUnitDTO.PointId))
                .Returns(Task.FromResult(fakePoint));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));
            var actionResult = await _controller.EditUnit(unitId, fakeUnitDTO);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}