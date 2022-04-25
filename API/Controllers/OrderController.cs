using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        readonly ApplicationContext _context;
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        readonly IEmailService _emailService;
        public OrderController(ApplicationContext context, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }   

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders([FromQuery] UserParams userParams)
        {
            var query = _context.Orders.AsQueryable();
            if(!userParams.showAll) query = query.Where(f => !f.Cancelled && !f.UnitReturned);
            var orders = await PagedList<OrderDTO>.CreateAsync(
                query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, userParams.PageSize
            );
            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);
            return Ok(orders);
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAvailableOrders()
        {
            var deliveryman = await _context.Users
                .Include(f => f.Location)
                .SingleOrDefaultAsync(f => f.UserName == User.GetUsername());
            if(deliveryman == null) return Unauthorized();

            var query = _context.Orders
                .Include(f => f.DeliveryLocation)
                .Include(f => f.ReturnFromLocation)
                .Include(f => f.ReturnDeliveryman)
                .Include(f => f.DeliveryMan)
                .AsQueryable();
                
            query = query.Where(f => f.DeliveryMan == null || (f.ReturnFromLocation != null && f.ReturnDeliveryman == null));
            query = query.Where(f => (f.DeliveryLocation!.Country!.Trim().ToLower() == deliveryman.Location!.Country!.Trim().ToLower()) 
            || (f.ReturnFromLocation!.Country!.Trim().ToLower() == deliveryman.Location!.Country!.Trim().ToLower()));
            query = query.Where(f => !f.Cancelled && !f.UnitReturned);

            var orders = await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpGet("taken")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAcceptedOrders()
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();

            return Ok(
                await _context.Orders
                    .Include(f => f.DeliveryMan)
                    .Include(f => f.ReturnDeliveryman)
                    .AsQueryable()
                    .Where(f => f.Client!.Id != user.Id)
                    .Where(f => (f.DeliveryMan != null && f.DeliveryMan.Id == user.Id && !f.ClientGotDelivery) 
                    || (f.ReturnDeliveryman != null && f.ReturnDeliveryman.Id == user.Id && !f.UnitReturned))
                    .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            );
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders()
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();

            return Ok(
                await _context.Orders
                    .Include(f => f.Client)
                    .OrderBy(f => f.Id)
                    .AsQueryable()
                    .Where(f => f.Client!.UserName == user.UserName)
                    .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            );
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            if(User.IsInRole("Admin")) return BadRequest("Create another account to order something");

            var unit = await _context.Units
                .Include(f => f.ItemUnitPoint)
                .ThenInclude(f => f!.Point)
                .SingleOrDefaultAsync(f => f.Id == dto.UnitId);
            var client = await _context.Users
                .Include(f => f.UserRoles)
                .Include(f => f.DeliveryLocations!.Where(s => s.Id == dto.DeliveryLocation))
                .SingleOrDefaultAsync(f => f.UserName == User.GetUsername());

            if(unit == null || client == null || !unit.IsAvailable || unit.Disabled) return NotFound();
            var userRolesCount = client.UserRoles?.Count;
            if(dto.DeliveryLocation != null && client.UserRoles!.Count > 1) return BadRequest("You can only choose selfpick");

            AppUser? deliveryman = null;
            var deliveryLocation = client!.DeliveryLocations!.FirstOrDefault();
            if(deliveryLocation == null && dto.DeliveryLocation == null)
            {
                deliveryman = client;
            }else if(deliveryLocation == null && dto.DeliveryLocation != null)
            { 
                return NotFound(); 
            }

            Order order = new() 
            {
                Unit = unit,
                Client = client,
                DeliveryMan = deliveryman,
                DeliveryLocation = deliveryLocation,
                DeliveryDate = dto.DeliveryDate,
                ReturnDate = dto.ReturnDate,
            };

            unit.IsAvailable = false;
            _context.Orders.Add(order);

            if(dto.DeliveryLocation != null)
            {
                var availableDeliverymans = await _context.Users
                    .Include(f => f.UserRoles!.Where(f => f.Role!.Name == "Deliveryman"))
                    .Where(f => f.UserRoles!.Count > 1)
                    .Include(f => f.Location)
                    .Where(f => f.Location!.Country == unit.ItemUnitPoint!.Point!.Country)
                    .ToListAsync();

                foreach(var availableDeliveryman in availableDeliverymans)
                {
                    await _emailService.SendEmail(new EmailMessage(availableDeliveryman.Email, "New available order", $"{unit.Description}"));
                }
            }

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<OrderDTO>(order));
        }



        [Authorize(Roles = "Deliveryman")]
        [HttpPost("{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> AcceptOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var query = _context.Orders
                .Include(f => f.DeliveryMan)
                .Include(f => f.ReturnDeliveryman)
                .Include(f => f.ReturnFromLocation)
                .Include(f => f.Client)
                .AsQueryable();

            var order = await query.Where(f => !f.Cancelled).SingleOrDefaultAsync(f => f.Id == orderId);

            if(user == null || order == null) return NotFound();
            if(order.DeliveryMan == null) order.DeliveryMan = user;
            else if(order.ReturnDeliveryman == null && order.ReturnFromLocation != null) order.ReturnDeliveryman = user;
            else return BadRequest("This order is already taken");

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "Deliveryman accepted your order", ""));

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.Where(f => f.Id == orderId).ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to accept order");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("start-delivery/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> StartDelivery(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var query = _context.Orders
                .Include(f => f.DeliveryMan)
                .Where(f => f.Id == orderId)
                .Include(f => f.Client)
                .AsQueryable();

            var order = await query.SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null || order.DeliveryCompleted || order.Cancelled) return NotFound();

            if(order.DeliveryMan == null || order.DeliveryMan.Id != user.Id) 
                return BadRequest("You did not accept this order");
            
            order.DeliveryInProcess = true;

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "The delivery of your order has begun", ""));

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to confirm delivery");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("delivered/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> DeliveredOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var query = _context.Orders
                .Include(f => f.DeliveryMan)
                .Where(f => f.Id == orderId)
                .Include(f => f.Client)
                .AsQueryable();

            var order = await query.SingleOrDefaultAsync(f => f.Id == orderId);

            if(user == null || order == null) return NotFound();
            if(order.DeliveryMan == null || order.DeliveryMan!.Id != user!.Id) 
                return BadRequest("You did not accept this order");

            if(!order.DeliveryInProcess) return BadRequest("You did not even start the delivery");

            order.DeliveryCompleted = true;

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "Collect the item", "The delivery of your order was finished"));
            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to confirm delivery");
        }

        [HttpPut("received/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ReceivedOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var query = _context.Orders
                .Include(f => f.Client)
                .Include(f => f.DeliveryMan)
                .Where(f =>f.Id == orderId)
                .AsQueryable();
            var order = await query.SingleOrDefaultAsync(f => f.Id == orderId);

            if(user == null || order == null || order.DeliveryMan == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();

            if(!order.DeliveryCompleted && order.DeliveryMan.UserName != user.UserName) return BadRequest("Deliveryman did not confirm delivery");

            if(User.IsInRole("Deliveryman")) order.ReturnDeliveryman = user;
            order.ClientGotDelivery = true;
            order.InUsage = true;

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to confirm delivery");
        }


        [HttpPut("cancel/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> CancelOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var query = _context.Orders
                .Include(f => f.Unit)
                .Include(f => f.Client)
                .Where(f => f.Id == orderId)
                .Include(f => f.DeliveryMan)
                .AsQueryable();

            var order = await query.FirstOrDefaultAsync(f => !f.Cancelled);
            if(user == null || order == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();

            if(order.DeliveryInProcess) return BadRequest("Delivery is already in progress");

            order.Cancelled = true;
            order.Unit!.IsAvailable = true;

            if(order.DeliveryMan != null)
                await _emailService.SendEmail(new EmailMessage(order.DeliveryMan.Email, "Client cancelled the order", ""));
            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to cancel delivery");
        }

        [HttpPut("selfpick/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> SelfpickOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var query = _context.Orders
                .Include(f => f.Client)
                .Include(f => f.DeliveryMan)
                .AsQueryable();

            var order = await query.SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();
            if(order.DeliveryMan != null) return BadRequest("Deliveryman already accepted this order");
            
            order.DeliveryMan = user;
            order.DeliveryLocation = null;
            
            if(await _context.SaveChangesAsync() > 0) return Ok(await query.Where(f => f.Id == orderId).ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to cancel order delivery");
        }



        [HttpPut("return")]
        public async Task<ActionResult<OrderDTO>> ReturnOrder(ReturnUnitDTO dto)
        {
            var user = await _context.Users
                .Include(f => f.DeliveryLocations)
                .Include(f => f.UserRoles)
                .SingleOrDefaultAsync(f => f.UserName == User.GetUsername());
            var query = _context.Orders.Include(f => f.Client).Include(f => f.ReturnDeliveryman).Where(f => f.Id == dto.Id && f.InUsage).AsQueryable();
            var order = await query.SingleOrDefaultAsync();
            if(order == null || user == null) return NotFound();

            if(User.IsInRole("Deliveryman") || user.UserRoles!.Count > 1)
            {
                if(order.ReturnPoint != null || dto.ReturnPoint == null) return BadRequest("You are deliveryman");
                order.ReturnPoint = await _context.Points.SingleOrDefaultAsync(f => f.Id == Convert.ToInt32(dto.ReturnPoint));
            }
            else
            {
                if(dto.ReturnPoint == null && dto.ReturnFromLocation != null)
                {
                    order.ReturnFromLocation = user.DeliveryLocations!.SingleOrDefault(f => f.Id == Convert.ToInt32(dto.ReturnFromLocation));
                    var availableDeliverymans = await _context.Users
                        .Include(f => f.UserRoles!.Where(f => f.Role!.Name == "Deliveryman"))
                        .Where(f => f.UserRoles!.Count > 1)
                        .Include(f => f.Location)
                        .Where(f => f.Location!.Country == order.ReturnFromLocation!.Country)
                        .ToListAsync();

                    foreach(var availableDeliveryman in availableDeliverymans)
                    {
                        await _emailService.SendEmail(new EmailMessage(availableDeliveryman.Email, "New available order", $"{order.ReturnFromLocation!.City}, {order.ReturnFromLocation!.Address}"));
                    }
                }
                else if(dto.ReturnPoint != null && dto.ReturnFromLocation == null)
                {
                    order.ReturnPoint = await _context.Points.SingleOrDefaultAsync(f => f.Id == Convert.ToInt32(dto.ReturnPoint));
                    order.ReturnDeliveryman = user;
                }
            }

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to set create return process");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("confirm-return/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ConfirmReturn(int orderId)
        {
            var user = await _context.Users.Include(f => f.DeliveryLocations).SingleOrDefaultAsync(f => f.UserName == User.GetUsername());
            var query = _context.Orders.Include(f => f.ReturnPoint).Include(f => f.ReturnDeliveryman).Include(f => f.Unit).Where(f => f.Id == orderId && f.InUsage).AsQueryable();
            var order = await query.SingleOrDefaultAsync();
            if(order == null || user == null) return NotFound();

            if(!order.ClientGotDelivery || order.UnitReturned || order.Cancelled || order.ReturnDeliveryman == null || order.ReturnPoint == null) return BadRequest("You cannot confirm the return of this order");

            var unit = await _context.Units.Include(f => f.ItemUnitPoint).SingleOrDefaultAsync(f => f.Id == order.Unit!.Id);
            if(unit == null) return NotFound();

            order.UnitReturned = true;
            unit.WhenWillBeAvailable = null;
            unit.IsAvailable = true;
            unit.ItemUnitPoint!.Point = order.ReturnPoint;

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to confirm return");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("confirm-receive/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ConfirmReceive(int orderId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(f => f.UserName == User.GetUsername());
            var query = _context.Orders.Include(f => f.DeliveryMan).Include(f => f.Client).Where(f => f.Id == orderId).AsQueryable();
            var order = await query.SingleOrDefaultAsync();
            if(order == null || user == null) return NotFound();

            if(order.ClientGotDelivery 
            || order.UnitReturned 
            || order.Cancelled 
            || order.DeliveryMan == null 
            || order.DeliveryCompleted 
            || order.DeliveryMan.UserName != order.Client!.UserName) return BadRequest("You cannot confirm the receive of this order");

            order.DeliveryCompleted = true;

            if(await _context.SaveChangesAsync() > 0) return Ok(await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().FirstOrDefaultAsync());
            return BadRequest("Failed to confirm return");
        }
    }
}