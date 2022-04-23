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
using DTOs;
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
        readonly string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
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
        public async Task<ActionResult> Register(RegisterDTO registerDTO)
        {
            registerDTO = TrimStrings<RegisterDTO>(registerDTO);
            if(await UserExists(registerDTO.Username!)) return BadRequest("User already exists");
            
            var user = _mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.Username;
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var role = await _userManager.AddToRoleAsync(user, "Client");
            if(!role.Succeeded) return BadRequest(role.Errors);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string message = $"{url()}/api/account/confirm-email/{user.UserName}/{token}";
            await _emailService.SendEmail(new EmailMessage(registerDTO.Email!, "Confirm email", message));
            return Ok();
        }

        [HttpGet("confirm-email/{username}/{**token}")]
        public async Task<ActionResult> ConfirmEmail(string username, string token)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user == null || user.EmailConfirmed) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded) return Content("Email was confirmed");
            return Content("Failed to confirm email");
        }

        [HttpPost("forgot-password/{email}")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null) return BadRequest("User with this email does not exists");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string url;
            if(env == "Development") url = "https://localhost:4200";
            else if(env == "Docker") url = "http://localhost:4200";
            else url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            string message = $"{url}/reset-password?token={token}";
            await _emailService.SendEmail(new EmailMessage(user.Email!, "Click the link to reset your password", message));
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null) return NotFound();
            var result = await _userManager.ResetPasswordAsync(user, dto.Token!.Replace(" ", "+"), dto.Password);
            if(result.Succeeded) return new EmptyResult();
            return BadRequest(result.Errors.FirstOrDefault()?.Description ?? "Failed to reset password");
        }

        [HttpPost("resend-email-confirmation/{email}")]
        public async Task<ActionResult> ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null) return BadRequest("User with this email does not exists");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string message = $"{url()}/api/account/confirm-email/{user.UserName}/{token}";
            await _emailService.SendEmail(new EmailMessage(email, "Confirm email", message));
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {

            // loginDTO = TrimStrings<LoginDTO>(loginDTO);
            AppUser? user = await _context.Users.Include(f => f.DeliveryLocations).SingleOrDefaultAsync(f => f.UserName == loginDTO.Username!.ToLower());
            if(user == null) return Unauthorized("Invalid username");

            if(!user.EmailConfirmed) return BadRequest("Email was not confirmed");

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


        string url()
        {
            return $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
        }

    }
}