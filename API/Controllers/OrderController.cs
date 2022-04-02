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
        public OrderController(ApplicationContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }   

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders([FromQuery] UserParams userParams)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(userParams);
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
                .Include(f => f.DeliveryMan)
                .AsQueryable();
            query = query.Where(f => f.DeliveryMan == null);
            query = query.Where(f => f.DeliveryLocation!.Country!.Trim().ToLower() == deliveryman.Location!.Country!.Trim().ToLower());
            query = query.Where(f => !f.Cancelled);

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
                    .AsQueryable()
                    .Where(f => f.Client!.Id != user.Id)
                    .Where(f => f.DeliveryMan != null && f.DeliveryMan.Id == user.Id)
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
                    .AsQueryable()
                    .Where(f => f.Client!.UserName == user.UserName)
                    .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync()
            );
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            var unit = await _context.Units.SingleOrDefaultAsync(f => f.Id == dto.UnitId);
            var client = await _context.Users
                .Include(f => f.DeliveryLocations!.Where(s => s.Id == dto.DeliveryLocation))
                .SingleOrDefaultAsync(f => f.UserName == User.GetUsername());

            if(unit == null || client == null || !unit.IsAvailable || unit.Disabled) return NotFound();

            AppUser? deliveryman = null;
            var deliveryLocation = client?.DeliveryLocations!.FirstOrDefault();
            if(deliveryLocation == null && dto.DeliveryLocation == null)
            {
                deliveryman = client;
            }else if(deliveryLocation == null && dto.DeliveryLocation != null)
            { 
                return NotFound(); 
            } 

            System.Console.WriteLine(deliveryLocation?.Country);

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

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<OrderDTO>(order));
        }



        [Authorize(Roles = "Deliveryman")]
        [HttpPost("{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> AcceptOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var order = await _context.Orders
                .Include(f => f.DeliveryMan)
                .Include(f => f.Client)
                .Include(f => f.Unit)
                .Include(f => f.DeliveryLocation)
                .Where(f => f.DeliveryMan == null && !f.Cancelled)
                .SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null) return NotFound();
            if(order.DeliveryMan != null) return BadRequest("This order is already taken");

            order.DeliveryMan = user;
            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<OrderDTO>(order));
            return BadRequest("Failed to accept order");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("start-delivery/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> StartDelivery(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var order = await _context.Orders
                .Include(f => f.DeliveryMan)
                .SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null || order.DeliveryCompleted || order.Cancelled) return NotFound();

            if(order.DeliveryMan == null || order.DeliveryMan.Id != user.Id) 
                return BadRequest("You did not accept this order");
            
            order.DeliveryInProcess = true;

            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<OrderDTO>(order));
            return BadRequest("Failed to confirm delivery");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("delivered/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> DeliveredOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var order = await _context.Orders
                .Include(f => f.Client)
                .Include(f => f.DeliveryMan)
                .Include(f => f.DeliveryLocation)
                .SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null) return NotFound();
            if(order.DeliveryMan == null || order.DeliveryMan!.Id != user.Id) 
                return BadRequest("You did not accept this order");

            if(!order.DeliveryInProcess) return BadRequest("You did not even start the delivery");

            order.DeliveryCompleted = true;
            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<OrderDTO>(order));
            return BadRequest("Failed to confirm delivery");
        }

        [HttpPut("received/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ReceivedOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var order = await _context.Orders
                .Include(f => f.Client)
                .Include(f => f.DeliveryLocation)
                .Include(f => f.DeliveryMan)
                .SingleOrDefaultAsync(f => f.Id == orderId);
            if(user == null || order == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();

            if(!order.DeliveryCompleted) return BadRequest("Deliveryman did not confirm delivery");

            order.ClientGotDelivery = true;
            order.InUsage = true;

            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<OrderDTO>(order));
            return BadRequest("Failed to confirm delivery");
        }


        [HttpPut("cancel/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> CancelOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var order = await _context.Orders
                .Include(f => f.Client)
                .Include(f => f.Unit)
                .SingleOrDefaultAsync(f => f.Id == orderId && !f.Cancelled);
            if(user == null || order == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();

            if(order.DeliveryInProcess) return BadRequest("Delivery is already in progress");

            order.Cancelled = true;
            order.Unit!.IsAvailable = true;

            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<OrderDTO>(order));
            return BadRequest("Failed to cancel delivery");
        }
    }
}