using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController  
    {
        readonly ApplicationContext _context;
        readonly ITokenService _tokenService;
        readonly IMapper _mapper;
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly IEmailService _emailService;
        public AccountController(ApplicationContext context, ITokenService tokenService, IMapper mapper, 
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            registerDTO = TrimStrings<RegisterDTO>(registerDTO);
            if(await UserExists(registerDTO.Username!)) return BadRequest("User already exists");
            
            var user = _mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.Username;
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var role = await _userManager.AddToRoleAsync(user, "Client");
            if(!role.Succeeded) return BadRequest(role.Errors);

            await _emailService.SendEmail(new EmailMessage(registerDTO.Email!, "Confirm email", "Go to link"));

            return new UserDTO
            {
                Username = registerDTO.Username,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.PhotoUrl
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {

            // loginDTO = TrimStrings<LoginDTO>(loginDTO);
            AppUser? user = await _context.Users.Include(f => f.DeliveryLocations).SingleOrDefaultAsync(f => f.UserName == loginDTO.Username!.ToLower());
            if(user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if(!result.Succeeded) return Unauthorized("Invalid password");


            var query = user.DeliveryLocations!.AsQueryable();
            var locations = query.ProjectTo<LocationDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToList();


            return new UserDTO
            {
                Username = user.UserName!,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.PhotoUrl,
                Locations = locations,
            };
        }

        async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(f => f.UserName == username.ToLower());
        }

        T TrimStrings<T>(T obj) where T: class
        {
            var stringProperties = obj.GetType().GetProperties()
                          .Where(p => p.PropertyType == typeof (string));
            foreach (var stringProperty in stringProperties)
            {
                if(stringProperty.Name != "Password"){
                    string currentValue = (string) stringProperty.GetValue(obj, null)!;
                    stringProperty.SetValue(obj, currentValue.Trim(), null);
                }
            }
            return obj;
        }
    }
}