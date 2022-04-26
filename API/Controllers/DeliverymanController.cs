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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class DeliverymanController : BaseApiController
    {

        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;
        readonly UserManager<AppUser> _userManager;
        readonly IEmailService _emailService;
        public DeliverymanController(IUnitOfWork unitOfWork, IMapper mapper, 
            UserManager<AppUser> userManager, IEmailService emailService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliverymanDTO>>> GetAllDeliverymans([FromQuery] UserParams userParams)
        {
            var deliverymans = await _unitOfWork.UserRepository.GetDeliverymansAsync(userParams);
            Response.AddPaginationHeader(deliverymans.CurrentPage, deliverymans.PageSize, deliverymans.TotalCount, deliverymans.TotalPages);
            return Ok(deliverymans);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<DeliverymanDTO>>> GetAllJoinRequests()
        {
            return Ok(await _unitOfWork.UserRepository.GetDeliverymanRequestsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("requests/{username}")]
        public async Task<ActionResult<DeliverymanDTO>> AddDeliveryMan(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if(user == null || !user.DeliverymanRequest || user.Location == null) return NotFound();

            user.DeliverymanRequest = false;
            await _userManager.AddToRoleAsync(user, "Deliveryman");
            await _unitOfWork.Complete();
            
            await _emailService.SendEmail(new EmailMessage(user.Email, "Congratulations, you joined deliverymans", "Logout and log in to your account to see changes"));
            return Ok(_mapper.Map<DeliverymanDTO>(user));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("requests/{username}")]
        public async Task<ActionResult<DeliverymanDTO>> DenyDeliveryMan(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if(user == null || !user.DeliverymanRequest) return NotFound();

            user.DeliverymanRequest = false;
            await _unitOfWork.Complete(); 
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{username}")]
        public async Task<ActionResult> RemoveDeliveryman(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if(user == null) return NotFound();

            var orders = await _unitOfWork.OrderRepository.GetActiveDeliveriesForDeliverymanAsync(user.UserName);
            
            if(orders.Count() > 0) return BadRequest($"{user.UserName} did not finsh {orders.Count()} order(s)");

            await _userManager.RemoveFromRoleAsync(user, "Deliveryman");
            await _unitOfWork.Complete();
            return Ok();
        }

        [HttpPost("join")]
        public async Task<ActionResult<UserDTO>> CreateBecomeDeliverymanRequest(JoinDeliverymanDTO dto)
        {
            if(User.IsInRole("Deliveryman")) return BadRequest("You are already a deliveryman");
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();
            if(user.DeliverymanRequest) return BadRequest("You already sended join request");

            user.DeliverymanRequest = true;
            user.Location = new() 
            {
                Country = dto.Country
            };
            
            if(await _unitOfWork.Complete()) return Ok(new UserDTO { DeliverymanRequest = true });
            return BadRequest("Failed to send request");
        }
    }
}