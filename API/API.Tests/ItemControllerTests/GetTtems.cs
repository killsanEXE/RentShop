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
    public class GetTtems : ItemControllerDependencyProvider
    {
        [Fact]
        public async void GetTtemsReturn200()
        {
            var fakeItems = A.CollectionOfDummy<ItemDTO>(3).AsEnumerable();
            UserParams fakeUserParams = new();
            var fakePagedList = new PagedList<ItemDTO>(fakeItems, fakeItems.Count(), 1, 3) { TotalPages = 1 };

            A.CallTo(() => _fakeUser.Identity!.IsAuthenticated).Returns(true);
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserAge("USERNAME"))
                .Returns(2);
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemsAsync(fakeUserParams, 2, false))
                .Returns(Task.FromResult(fakePagedList));
            
            _itemController.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() 
                { 
                    User = _fakeUser 
                } };
            var actionResult = await _itemController.GetItems(fakeUserParams);

            var result = actionResult.Result as OkObjectResult;
            var resultItems = result?.Value as IEnumerable<ItemDTO>;
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(fakeItems.Count(), resultItems?.Count());
        }
    }
}