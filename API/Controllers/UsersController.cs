using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;
        readonly IPhotoService _photoService;
        readonly IWrapper _wrapper;
        public UsersController(IPhotoService photoService, IUnitOfWork unitOfWork, IMapper mapper, IWrapper wrapper)
        {
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _wrapper = wrapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetUsers([FromQuery] UserParams userParams){
            var users = await _unitOfWork.UserRepository.GetClientsAsync(userParams);
            _wrapper.AddPaginationHeaderViaWrapper(Response, users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));

            if(user.PhotoUrl != null && user.PublicPhotoId != null)
            {
                var deletionResult = await _photoService.DeletePhotoAsync(user.PublicPhotoId);
                if(deletionResult.Error != null) return BadRequest(deletionResult.Error.Message);
            }

            var result = await _photoService.AddUserPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            user.PhotoUrl = result.SecureUri.AbsoluteUri;
            user.PublicPhotoId = result.PublicId;
            if(await _unitOfWork.Complete()) return Content(user.PhotoUrl);
            return BadRequest("Error while adding a photo");
        }

        [HttpPost("locations")]
        public async Task<ActionResult<LocationDTO>> AddLocation(LocationDTO locationDTO)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            Location deliveryLocation = new()
            {
                Country = locationDTO.Country,
                City = locationDTO.City,
                Address = locationDTO.Address,
                Floor = locationDTO.Floor
            };
            if(locationDTO.Apartment != null) deliveryLocation.Apartment = locationDTO.Apartment;
            user.DeliveryLocations!.Add(deliveryLocation);
            if(await _unitOfWork.Complete()) return _mapper.Map<LocationDTO>(deliveryLocation);
            return BadRequest("Failed to add point"); 
        }

        [HttpPut("locations/{locationId:int}")]
        public async Task<ActionResult<LocationDTO>> EditLocation(int locationId, LocationDTO locationDTO)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var location = user.DeliveryLocations!.SingleOrDefault(f => f.Id == locationId);
            if(user == null || location == null) return NotFound();

            location.Address = locationDTO.Address;
            location.Country = locationDTO.Country;
            location.City = locationDTO.City;
            location.Floor = locationDTO.Floor;
            location.Apartment = locationDTO.Apartment;

            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<LocationDTO>(location));
            return BadRequest("Failed to update location");
        }

        [HttpDelete("locations/{locationId:int}")]
        public async Task<ActionResult> DeleteLocation(int locationId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(_wrapper.GetUsernameViaWrapper(User));
            var location = user.DeliveryLocations!.SingleOrDefault(f => f.Id == locationId);
            if(user == null || location == null) return NotFound();

            user.DeliveryLocations!.Remove(location);
            if(await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to delete location");
        }
    }
}