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
        readonly ApplicationContext _context;
        readonly UserManager<AppUser> _userManager;
        public DeliverymanController(IUnitOfWork unitOfWork, IMapper mapper, 
            ApplicationContext context, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
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
            var query = _context.Users.Where(f => f.DeliverymanRequest).AsQueryable();
            return Ok(await query.ProjectTo<DeliverymanDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("requests/{username}")]
        public async Task<ActionResult<DeliverymanDTO>> AddDeliveryMan(string username)
        {
            // var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var user = await _context.Users.Include(f => f.Location).SingleOrDefaultAsync(f => f.UserName == username);
            if(user == null || !user.DeliverymanRequest) return NotFound();

            user.DeliverymanRequest = false;
            await _userManager.AddToRoleAsync(user, "Deliveryman");
            await _context.SaveChangesAsync(); 
            return Ok(_mapper.Map<DeliverymanDTO>(user));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("requests/{username}")]
        public async Task<ActionResult<DeliverymanDTO>> DenyDeliveryMan(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if(user == null || !user.DeliverymanRequest) return NotFound();

            user.DeliverymanRequest = false;
            await _context.SaveChangesAsync(); 
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
            
            if(await _context.SaveChangesAsync() > 0) return Ok(new UserDTO { DeliverymanRequest = true });
            return BadRequest("Failed to send request");
        }
    }
}