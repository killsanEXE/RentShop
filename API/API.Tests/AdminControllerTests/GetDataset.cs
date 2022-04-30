using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.AdminControllerTests
{
    [Collection("Sequential")]
    public class GetDataset : AdminControllerDependencyProvider
    {
        [Fact]
        public async void GetDatasetReturn200()
        {
            var fakeItems = A.CollectionOfDummy<DatasetItemDTO>(6).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetDatasetItemsAsync())
                .Returns(Task.FromResult(fakeItems));

            var actionResult = await _adminController.GetDataset();

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as DatasetDTO;
            Assert.Equal(fakeItems.Count(), resultDTO?.Items?.Count());
        }

        [Fact]
        public async void GetDatasetReturn400()
        {
            var fakeItems = A.CollectionOfDummy<DatasetItemDTO>(0).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetDatasetItemsAsync())
                .Returns(Task.FromResult(fakeItems));

            var actionResult = await _adminController.GetDataset();

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}