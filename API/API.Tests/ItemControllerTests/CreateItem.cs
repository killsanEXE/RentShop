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
    public class CreateItem : ItemControllerDependencyProvider
    {
        [Fact]
        public async void CreateItemReturn200()
        {
            ItemDTO fakeItemDTO = new();
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.AddItem(new Item()))
                .DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _itemController.CreateItem(fakeItemDTO);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as ItemDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }        
        
        [Fact]
        public async void CreateItemReturn400()
        {
            ItemDTO fakeItemDTO = new();
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.AddItem(new Item()))
                .DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _itemController.CreateItem(fakeItemDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}