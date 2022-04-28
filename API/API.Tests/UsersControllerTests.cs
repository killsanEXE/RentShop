using System.Security.Claims;
using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace API.Tests
{
    public class UsersControllerTests : DependencyProvider
    {
        readonly UsersController _UsersController = null!;
        readonly UnitController _unitController = null!;
        protected readonly ITestOutputHelper _output;
        public UsersControllerTests(ITestOutputHelper output)
        {
            _UsersController = new UsersController(_fakePhotoService, _fakeUnitOfWork, _mapper, _wrapper);
            _unitController = new UnitController(_fakeUnitOfWork, _mapper);
            _output = output;
        }

        [Fact]
        public async void GetUsersReturn200()
        {
            var fakeUsers = A.CollectionOfDummy<ClientDTO>(3).AsEnumerable();
            UserParams fakeUserParams = new();
            var fakePagedList = new PagedList<ClientDTO>(fakeUsers, fakeUsers.Count(), 1, 3) { TotalPages = 1 };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetClientsAsync(fakeUserParams))
                .Returns(Task.FromResult(fakePagedList));

            var response = A.Fake<HttpResponse>();
            var actionResult = await _UsersController.GetUsers(fakeUserParams);

            var result = actionResult.Result as OkObjectResult;
            var resultUsers = result?.Value as IEnumerable<ClientDTO>;
            Assert.Equal(fakeUsers.Count(), resultUsers?.Count());
        }

        [Fact]
        public async void AddLocationReturn200()
        {
            AppUser fakeUser = new()
            {
                Name = "user"
            };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new());
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Username"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(user)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            _UsersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var actionResult = await _UsersController.AddLocation(fakeLocationDTO);

            var result = actionResult.Value;
            Assert.Equal(fakeLocationDTO.Country, result?.Country);
        }        

        [Fact]
        public async void AddLocationReturn400()
        {
            AppUser fakeUser = new()
            {
                Name = "user"
            };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Username"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(user)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            _UsersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var actionResult = await _UsersController.AddLocation(fakeLocationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }


        [Fact]
        public async void EditLocationReturn200()
        {
            var locationId = 1;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "USA",
                City = "LA",
                Address = "something",
            };
            AppUser fakeUser = new()
            {
                Name = "user"
            };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new()
            {
                Id = 1,
                Country = "Belarus",
                City = "Minsk"
            });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Username"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(user)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _UsersController.EditLocation(locationId, fakeLocationDTO);

            var result = (actionResult.Result as OkObjectResult)?.Value as LocationDTO;
            Assert.Equal(fakeLocationDTO.Country, result?.Country);
        }

        [Fact]
        public async void EditLocationReturn400()
        {
            var locationId = 1;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };
            AppUser fakeUser = new()
            {
                Name = "user"
            };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new()
            {
                Id = 1,
                Country = "Belarus",
                City = "Minsk"
            });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Username"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(user)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _UsersController.EditLocation(locationId, fakeLocationDTO);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);           
        }

        [Fact]
        public async void EditLocationReturn404()
        {
            var locationId = 1;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };
            AppUser fakeUser = new()
            {
                Name = "user"
            };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Username"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(user)))
                .Returns(Task.FromResult(fakeUser));

            var actionResult = await _UsersController.EditLocation(locationId, fakeLocationDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);   
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