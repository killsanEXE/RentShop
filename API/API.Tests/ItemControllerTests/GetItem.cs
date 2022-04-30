using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.ItemControllerTests
{
    [Collection("Sequential")]
    public class GetItem : ItemControllerDependencyProvider
    {
        [Fact]
        public async void GetItemReturn200()
        {
            int itemId = 1;
            ItemDTO fakeItem = new() { Id = itemId, Name = "ITEM" };
            A.CallTo(() => _fakeUser.Identity!.IsAuthenticated).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemDTOByIdAsync(itemId, 2, false))
                .Returns(Task.FromResult(fakeItem));

            _itemController.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() 
                { 
                    User = _fakeUser 
                } };
            var actionResult = await _itemController.GetItem(itemId);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as ItemDTO;
            Assert.Equal(200, result?.StatusCode);
            Assert.NotNull(resultDTO);
        }

        [Fact]
        public async void GetItemReturn404()
        {
            int itemId = 1;
            ItemDTO? fakeItem = null;
            A.CallTo(() => _fakeUser.Identity!.IsAuthenticated).Returns(false);
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemDTOByIdAsync(itemId, 16, false))
                .Returns(Task.FromResult(fakeItem)!);

            _itemController.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() 
                { 
                    User = _fakeUser 
                } };
            var actionResult = await _itemController.GetItem(itemId);
        
            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }
    }
}