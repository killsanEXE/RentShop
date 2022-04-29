using System.Security.Claims;
using System.Security.Principal;
using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace API.Tests
{
    public class Tests : DependencyProvider
    {
        readonly UsersController _usersController = null!;
        readonly UnitController _unitController = null!;
        readonly AdminController _adminController = null!;
        readonly DeliverymanController _deliverymanController = null!;
        readonly ItemController _itemController = null!;
        protected readonly ITestOutputHelper _output;
        protected ClaimsPrincipal _fakeUser = null!;
        public Tests(ITestOutputHelper output)
        {
            _usersController = new UsersController(_fakePhotoService, _fakeUnitOfWork, _mapper, _wrapper);
            _unitController = new UnitController(_fakeUnitOfWork, _mapper);
            _adminController = new AdminController(_fakeUnitOfWork, _mapper);
            _deliverymanController = new DeliverymanController(_fakeUnitOfWork, 
                _mapper, _fakeUserManager, _fakeEmailService, _wrapper);
            _itemController = new ItemController(_fakeUnitOfWork, _mapper, _fakePhotoService, _wrapper);
            // _itemController.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() 
            //     { 
            //         User = _fakeUser 
            //     } };

            _fakeUser = A.Fake<ClaimsPrincipal>(f => f.WithArgumentsForConstructor(() => 
                    new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Username"),
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim("custom-claim", "example claim value"),
                    }, "mock"))
                ));

            _output = output;
        }


        //USERS CONTROLLER
        [Fact]
        public async void GetUsersReturn200()
        {
            var fakeUsers = A.CollectionOfDummy<ClientDTO>(3).AsEnumerable();
            UserParams fakeUserParams = new();
            var fakePagedList = new PagedList<ClientDTO>(fakeUsers, fakeUsers.Count(), 1, 3) { TotalPages = 1 };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetClientsAsync(fakeUserParams))
                .Returns(Task.FromResult(fakePagedList));

            var actionResult = await _usersController.GetUsers(fakeUserParams);

            var result = actionResult.Result as OkObjectResult;
            var resultUsers = result?.Value as IEnumerable<ClientDTO>;
            Assert.Equal(fakeUsers.Count(), resultUsers?.Count());
        }

        [Fact]
        public async void AddLocationReturn200()
        {
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new());
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            _usersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fakeUser }
            };

            var actionResult = await _usersController.AddLocation(fakeLocationDTO);

            var result = actionResult.Value;
            Assert.Equal(fakeLocationDTO.Country, result?.Country);
        }        

        [Fact]
        public async void AddLocationReturn400()
        {
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            LocationDTO fakeLocationDTO = new()
            {
                Country = "Belarus",
                City = "Minsk",
                Address = "something",
            };

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            _usersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fakeUser }
            };

            var actionResult = await _usersController.AddLocation(fakeLocationDTO);

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
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new()
            {
                Id = 1,
                Country = "Belarus",
                City = "Minsk"
            });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _usersController.EditLocation(locationId, fakeLocationDTO);

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
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations?.Add(new()
            {
                Id = 1,
                Country = "Belarus",
                City = "Minsk"
            });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _usersController.EditLocation(locationId, fakeLocationDTO);

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
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));

            var actionResult = await _usersController.EditLocation(locationId, fakeLocationDTO);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);   
        }

        [Fact]
        public async void DeleteLocationReturn200()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations.Add(new() { Id = 1 });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as OkResult;
            Assert.Equal(200, result?.StatusCode);
        }

        [Fact]
        public async void DeleteLocationReturn400()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;
            fakeUser.DeliveryLocations.Add(new() { Id = 1 });

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(false));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public async void DeleteLocationReturn404()
        {
            int locationId = 1;
            AppUser fakeUser = new() { Name = "user" };
            var fakeDeliveryLocations = A.CollectionOfDummy<Location>(4);
            fakeUser.DeliveryLocations = fakeDeliveryLocations;

            A.CallTo(() => _fakeUnitOfWork.UserRepository
                .GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(_fakeUser)))
                .Returns(Task.FromResult(fakeUser));

            var actionResult = await _usersController.DeleteLocation(locationId);

            var result = actionResult as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }











        //UNIT CONTROLLER
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










        //ADMIN CONTROLLER
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














        //DELIVERYMAN CONTROLLERS
        [Fact]
        public async void GetAllDeliverymansReturn200()
        {
            var fakeDeliverymans = A.CollectionOfDummy<DeliverymanDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetDeliverymansAsync())
                .Returns(Task.FromResult(fakeDeliverymans));

            var actionResult = await _deliverymanController.GetAllDeliverymans();

            var result = (actionResult.Result as OkObjectResult)?.Value as IEnumerable<DeliverymanDTO>;
            Assert.Equal(fakeDeliverymans.Count(), result?.Count());
        }

        [Fact]
        public async void GetAllJoinRequestsReturn200()
        {
            var fakeDeliverymans = A.CollectionOfDummy<DeliverymanDTO>(5).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetDeliverymanRequestsAsync())
                .Returns(Task.FromResult(fakeDeliverymans));

            var actionResult = await _deliverymanController.GetAllJoinRequests();

            _output.WriteLine(actionResult.ToString());
            var result = (actionResult.Result as OkObjectResult)?.Value as IEnumerable<DeliverymanDTO>;
            Assert.Equal(fakeDeliverymans.Count(), result?.Count());
        }

        [Fact]
        public async void AddDeliveryManReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = true, Location = new() };

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUserManager.AddToRoleAsync(user, "Deliveryman"))
                .Returns(Task.FromResult(new IdentityResult()));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage())).Returns(Task.FromResult(true));

            var actionResult = await _deliverymanController.AddDeliveryMan("USERNAME");

            var result = (actionResult.Result as OkObjectResult)?.Value as DeliverymanDTO;
            Assert.Equal(user.UserName, result?.Username);            
        }

        [Fact]
        public async void AddDeliveryManReturn404()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));

            
            var actionResult = await _deliverymanController.AddDeliveryMan("USERNAME");

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);    
        }

        [Fact]
        public async void DenyDeliverymanReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = true, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeEmailService.SendEmail(new EmailMessage())).Returns(Task.FromResult(true));
            
            var actionResult = await _deliverymanController.DenyDeliveryMan("USERNAME");

            var result = actionResult.Result as OkResult;
            Assert.Equal(200, result?.StatusCode);
        }

        [Fact]
        public async void DenyDeliverymanReturn404()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            
            var actionResult = await _deliverymanController.DenyDeliveryMan("USERNAME");

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void RemoveDeliverymanReturn200()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            var fakeActiveOrders = A.CollectionOfDummy<Order>(0).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetActiveDeliveriesForDeliverymanAsync("USERNAME"))
                .Returns(Task.FromResult(fakeActiveOrders));
            A.CallTo(() => _fakeUserManager.RemoveFromRoleAsync(user, "Deliveryman"))
                .Returns(Task.FromResult(new IdentityResult()));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));

            var actionResult = await _deliverymanController.RemoveDeliveryman("USERNAME");

            var result = actionResult as OkResult;
            Assert.Equal(200, result?.StatusCode); 
        }

        [Fact]
        public async void RemoveDeliverymanReturn400()
        {
            AppUser user = new() { UserName = "USERNAME", DeliverymanRequest = false, Location = new() };
            var fakeActiveOrders = A.CollectionOfDummy<Order>(1).AsEnumerable();
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.OrderRepository.GetActiveDeliveriesForDeliverymanAsync("USERNAME"))
                .Returns(Task.FromResult(fakeActiveOrders));

            var actionResult = await _deliverymanController.RemoveDeliveryman("USERNAME");

            var result = actionResult as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode); 
        }

        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn200()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser user = new() { DeliverymanRequest = false };
            var userRoles = A.CollectionOfDummy<AppUserRole>(1).ToList();
            user.UserRoles = userRoles;

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => _fakeUnitOfWork.Complete()).Returns(Task.FromResult(true));
            
            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as OkObjectResult;
            var resultDTO = result?.Value as UserDTO;

            Assert.Equal(200, result?.StatusCode);
            Assert.True(resultDTO?.DeliverymanRequest);
        }

        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn404()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser? user = null;
            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user)!);

            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as NotFoundResult;
            Assert.Equal(404, result?.StatusCode);
        }

        [Fact]
        public async void CreateBecomeDeliverymanRequestReturn400()
        {
            JoinDeliverymanDTO dto = new() { Country = "Belarus" };
            AppUser user = new() { DeliverymanRequest = true };
            var userRoles = A.CollectionOfDummy<AppUserRole>(2).ToList();
            user.UserRoles = userRoles;

            A.CallTo(() => _fakeUnitOfWork.UserRepository.GetUserByUsernameAsync("USERNAME"))
                .Returns(Task.FromResult(user));
            
            var actionResult = await _deliverymanController.CreateBecomeDeliverymanRequest(dto);

            var result = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, result?.StatusCode);
        }





















        //ITEM CONTROLLER
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