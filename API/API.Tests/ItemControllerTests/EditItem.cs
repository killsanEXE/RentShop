using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.ItemControllerTests
{
    [Collection("Sequential")]
    public class EditItem : ItemControllerDependencyProvider
    {
        [Fact]
        public async void EditItemReturn200()
        {
            int itemId = 1;
            ItemDTO itemDTO = new() 
            { 
                Name = "New name", 
                Description = "New Description",
                AgeRestriction = 20,
                PricePerDay = 20,
            };

            Item item = new() { Name = "Original name", Description = "Original Descriptino" };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(item));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _itemController.EditItem(itemId, itemDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as ItemDTO;

            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }

        [Fact]
        public async void EditItemReturn400()
        {
            int itemId = 1;
            ItemDTO itemDTO = new() 
            { 
                Name = "New name", 
                Description = "New Description",
                AgeRestriction = 20,
                PricePerDay = 20,
            };

            Item item = new() { Name = "Original name", Description = "Original Descriptino" };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(item));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _itemController.EditItem(itemId, itemDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void EditItemReturn404()
        {
            int itemId = 1;
            ItemDTO itemDTO = new() 
            { 
                Name = "New name", 
                Description = "New Description",
                AgeRestriction = 20,
                PricePerDay = 20,
            };

            Item? item = null;

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemByIdAsync(itemId))
                .Returns(Task.FromResult(item)!);

            var actionResult = await _itemController.EditItem(itemId, itemDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }
    }
}