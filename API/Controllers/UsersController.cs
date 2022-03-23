using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;
        readonly IPhotoService _photoService;
        public UsersController(IPhotoService photoService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetUsers([FromQuery] UserParams userParams){
            var users = await _unitOfWork.UserRepository.GetClientsAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user.PhotoUrl != null && user.PublicPhotoId != null)
            {
                var deletionResult = await _photoService.DeletePhotoAsync(user.PublicPhotoId);
                if(deletionResult.Error != null) return BadRequest(deletionResult.Error.Message);
            }

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            user.PhotoUrl = result.SecureUri.AbsoluteUri;
            user.PublicPhotoId = result.PublicId;
            if(await _unitOfWork.Complete()) return Content(user.PhotoUrl);
            return BadRequest("Error while adding a photo");
        }
    }
}