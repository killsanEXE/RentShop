using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.Tests.AdminControllerTests
{
    [Collection("Sequential")]
    public class SetDataset : AdminControllerDependencyProvider
    {
        [Fact]
        public async void SetDatasetReturn200()
        {
            var fakeDatasetItems = A.CollectionOfDummy<DatasetItemDTO>(10).ToList();
            DatasetItemDTO singleDatasetItem = new()
            {
                AgeRestriction = 1,
                Description = "new description",
                PricePerDay = 1
            };
            Item fakeItem = new()
            {
                AgeRestriction = 0,
                Description = "original description",
                PricePerDay = 0
            };
            fakeDatasetItems.Add(singleDatasetItem);
            var fakeSameNameItems = A.CollectionOfDummy<Item>(0).AsEnumerable().Append(fakeItem);
            DatasetDTO fakeDatasetDTO = new() { Items = fakeDatasetItems };

            A.CallTo(() => _fakeUnitOfWork.ItemRepository.GetItemsByNameAsync("SAME NAEM ITEM"))
                .Returns(Task.FromResult(fakeSameNameItems));
            A.CallTo(() => _fakeUnitOfWork.ItemRepository.AddItem(fakeItem)).DoesNothing();
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _adminController.SetDataset(fakeDatasetDTO);

            var result = actionResult as OkResult;
            Assert.Equal(200, result?.StatusCode);
        }

        [Fact]
        public async void SetDatasetReturn400()
        {
            DatasetDTO fakeDatasetDTO = new();
            var actionResult = await _adminController.SetDataset(fakeDatasetDTO);

            var result = actionResult as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }
    }
}