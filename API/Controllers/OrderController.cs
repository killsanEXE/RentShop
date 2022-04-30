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
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        readonly IEmailService _emailService;
        readonly IWrapper _wrapper;
        public OrderController(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService,
            IWrapper wrapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _wrapper = wrapper;
        }   

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders([FromQuery] UserParams userParams)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(userParams, userParams.showAll);
            _wrapper.AddPaginationHeaderViaWrapper(Response, orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);
            return Ok(orders);
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAvailableOrders()
        {
            var deliveryman = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            if(deliveryman == null) return Unauthorized();
            return Ok(await _unitOfWork.OrderRepository.GetAvailableOrdersAsync(deliveryman));
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpGet("taken")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAcceptedOrders()
        {
            var deliveryman = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            if(deliveryman == null) return Unauthorized();
            return Ok(await _unitOfWork.OrderRepository.GetTakenOrdersAsync(deliveryman));
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders()
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            if(user == null) return NotFound();
            return Ok(await _unitOfWork.OrderRepository.GetClientOrdersAsync(user.UserName));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            if(User.IsInRole("Admin")) return BadRequest("Create another account to order something");

            var unit = await _unitOfWork.UnitRepository.GetUnitByIdAsync(dto.UnitId);
            var client = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));

            if(unit.Id == -1 || client == null || !unit.IsAvailable || unit.Disabled) return NotFound();
            if(dto.DeliveryLocation != null && client.UserRoles!.Count > 1) return BadRequest("You can only choose selfpick");

            AppUser? deliveryman = null;
            var deliveryLocation = client.DeliveryLocations?.SingleOrDefault(f => f.Id == dto.DeliveryLocation);
            if(deliveryLocation == null && dto.DeliveryLocation == null)
            {
                deliveryman = client;
            }
            else if(deliveryLocation == null && dto.DeliveryLocation != null) return NotFound(); 

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
            _unitOfWork.OrderRepository.AddOrder(order);

            if(dto.DeliveryLocation != null)
            {
                var availableDeliverymans = await _unitOfWork.UserRepository.GetDeliverymansFromCountryAsync(order!.DeliveryLocation?.Country ?? "");
                foreach(var availableDeliveryman in availableDeliverymans)
                {
                    await _emailService.SendEmail(new EmailMessage(availableDeliveryman.Email, "New available order", $"{unit.Description}"));
                }
            }

            await _unitOfWork.Complete();
            return Ok(_mapper.Map<OrderDTO>(order));
        }



        [Authorize(Roles = "Deliveryman")]
        [HttpPost("{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> AcceptOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            if(user == null || order.Id == -1) return NotFound();
            if(order.DeliveryMan == null) order.DeliveryMan = user;
            else if(order.ReturnDeliveryman == null && order.ReturnFromLocation != null) order.ReturnDeliveryman = user;
            else return BadRequest("This order is already taken");

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "Deliveryman accepted your order", ""));

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to accept order");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("start-delivery/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> StartDelivery(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if(user == null || order.Id == -1 || order.DeliveryCompleted || order.Cancelled) return NotFound();

            if(order.DeliveryMan == null || order.DeliveryMan.Id != user.Id) 
                return BadRequest("You did not accept this order");
            
            order.DeliveryInProcess = true;

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "The delivery of your order has begun", ""));

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to confirm delivery");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("delivered/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> DeliveredOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            if(user == null || order.Id == -1) return NotFound();
            if(order.DeliveryMan == null || order.DeliveryMan!.Id != user!.Id) 
                return BadRequest("You did not accept this order");

            if(!order.DeliveryInProcess) return BadRequest("You did not even start the delivery");

            order.DeliveryCompleted = true;

            await _emailService.SendEmail(new EmailMessage(order.Client!.Email, "Collect the item", "The delivery of your order was finished"));
            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to confirm delivery");
        }

        [HttpPut("received/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ReceivedOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            if(user == null || order.Id == -1 || order.DeliveryMan == null || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();

            if(!order.DeliveryCompleted && order.DeliveryMan.UserName != user.UserName) return BadRequest("Deliveryman did not confirm delivery");

            if(User.IsInRole("Deliveryman")) order.ReturnDeliveryman = user;
            order.ClientGotDelivery = true;
            order.InUsage = true;

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to confirm delivery");
        }


        [HttpPut("cancel/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> CancelOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            if(user == null || order.Id == -1 || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();
            if(order.DeliveryInProcess) return BadRequest("Delivery is already in progress");

            order.Cancelled = true;
            order.Unit!.IsAvailable = true;

            if(order.DeliveryMan != null)
                await _emailService.SendEmail(new EmailMessage(order.DeliveryMan.Email, "Client cancelled the order", ""));
            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to cancel delivery");
        }

        [HttpPut("selfpick/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> SelfpickOrder(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            if(user == null || order.Id == -1 || order.Client!.Id != user.Id || order.ClientGotDelivery) return NotFound();
            if(order.DeliveryMan != null) return BadRequest("Deliveryman already accepted this order");
            
            order.DeliveryMan = user;
            order.DeliveryLocation = null;
            
            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to cancel order delivery");
        }



        [HttpPut("return")]
        public async Task<ActionResult<OrderDTO>> ReturnOrder(ReturnUnitDTO dto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(dto.Id);
            if(order.Id == -1 || user == null) return NotFound();

            if(User.IsInRole("Deliveryman") || user.UserRoles!.Count > 1)
            {
                if(order.ReturnPoint != null || dto.ReturnPoint == null) return BadRequest("You are deliveryman");
                order.ReturnPoint = await _unitOfWork.PointRepository.GetPointByIdAsync(Convert.ToInt32(dto.ReturnPoint));
            }
            else
            {
                if(dto.ReturnPoint == null && dto.ReturnFromLocation != null)
                {
                    order.ReturnFromLocation = user.DeliveryLocations!.SingleOrDefault(f => f.Id == Convert.ToInt32(dto.ReturnFromLocation));
                    var availableDeliverymans = await _unitOfWork.UserRepository.GetDeliverymansFromCountryAsync(order.ReturnFromLocation!.Country ?? "");
                    foreach(var availableDeliveryman in availableDeliverymans)
                    {
                        await _emailService.SendEmail(new EmailMessage(availableDeliveryman.Email, "New available order", $"{order.ReturnFromLocation!.City}, {order.ReturnFromLocation!.Address}"));
                    }
                }
                else if(dto.ReturnPoint != null && dto.ReturnFromLocation == null)
                {
                    order.ReturnPoint = await _unitOfWork.PointRepository.GetPointByIdAsync(Convert.ToInt32(dto.ReturnPoint));
                    order.ReturnDeliveryman = user;
                }
            }

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to set create return process");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("confirm-return/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ConfirmReturn(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if(order.Id == -1 || user == null) return NotFound();

            if(!order.ClientGotDelivery || order.UnitReturned || order.Cancelled || order.ReturnDeliveryman == null || order.ReturnPoint == null) return BadRequest("You cannot confirm the return of this order");

            var unit = await _unitOfWork.UnitRepository.GetUnitByIdAsync(order.Unit!.Id);
            if(unit.Id == -1) return NotFound();

            order.UnitReturned = true;
            unit.WhenWillBeAvailable = null;
            unit.IsAvailable = true;
            unit.ItemUnitPoint!.Point = order.ReturnPoint;

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to confirm return");
        }

        [Authorize(Roles = "Deliveryman")]
        [HttpPut("confirm-receive/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> ConfirmReceive(int orderId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);
            if(order.Id == -1 || user == null) return NotFound();

            if(order.ClientGotDelivery 
            || order.UnitReturned 
            || order.Cancelled 
            || order.DeliveryMan == null 
            || order.DeliveryCompleted 
            || order.DeliveryMan.UserName != order.Client!.UserName) return BadRequest("You cannot confirm the receive of this order");

            order.DeliveryCompleted = true;

            if(await _unitOfWork.Complete()) return Ok(await _unitOfWork.OrderRepository.GetOrderDTOByIdAsync(order.Id));
            return BadRequest("Failed to confirm return");
        }
    }
}