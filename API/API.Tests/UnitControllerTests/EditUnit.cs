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
    public class EditUnit : UnitControllerDependencyProvider
    {
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

            var actionResult = await _unitController.EditUnit(unitId, fakeUnitDTO);
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

            var actionResult = await _unitController.EditUnit(unitId, fakeUnitDTO);
            
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
            var actionResult = await _unitController.EditUnit(unitId, fakeUnitDTO);
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}