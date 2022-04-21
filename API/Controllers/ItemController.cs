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
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ItemController : BaseApiController
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        readonly IPhotoService _photoService;
        public ItemController(IUnitOfWork unitOfWork, ApplicationContext context, IMapper mapper,
            IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems([FromQuery] UserParams userParams)
        {
            int age = 16;
            bool admin = false;
            if(User.Identity!.IsAuthenticated)
            {
                if(User.IsInRole("Admin")) admin = true;
                age = _unitOfWork.UserRepository.GetUserAge(User.GetUsername());
            }
            var items = await _unitOfWork.ItemRepository.GetItemsAsync(userParams, age, admin);
            Response.AddPaginationHeader(items.CurrentPage, items.PageSize, items.TotalCount, items.TotalPages);
            return Ok(items);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemDTO>> GetItem(int id)
        {
            int age = 16;
            bool admin = false;
            if(User.Identity!.IsAuthenticated){
                if(User.IsInRole("Admin")) admin = true;
                age = _unitOfWork.UserRepository.GetUserAge(User.GetUsername());
            }
            var item = await _unitOfWork.ItemRepository.GetItemDTOByIdAsync(id, age, admin);
            if(item != null) return Ok(item);
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<ItemDTO>> CreateItem(ItemDTO itemDTO)
        {
            var item = _mapper.Map<Item>(itemDTO);
            _context.Items.Add(item);
            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<ItemDTO>(item));
            return BadRequest("Failed to add new item");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit-item/{id:int}")]
        public async Task<ActionResult<ItemDTO>> EditItem(int id, ItemDTO itemDTO)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(id);
            if(item == null) return NotFound();

            item.Name = itemDTO.Name;        
            item.Description = itemDTO.Description;        
            item.AgeRestriction = itemDTO.AgeRestriction;        
            item.PricePerDay = itemDTO.PricePerDay;   

            if(await _context.SaveChangesAsync() > 0) return Ok(await _unitOfWork.ItemRepository.GetItemDTOByIdAsync(item.Id, 1000, true));
            return BadRequest("Failed to update item");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("disable/{id:int}")]
        public async Task<ActionResult> DisableItem(int id)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(id);
            if(item == null) return NotFound();
            item.Disabled = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("enable/{id:int}")]
        public async Task<ActionResult> EnableItem(int id)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(id);
            if(item == null) return NotFound();
            item.Disabled = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // [Authorize(Roles = "Admin")]
        // [HttpDelete("{id:int}")]
        // public async Task<ActionResult> DeleteItem(int id)
        // {
        //     var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(id);
        //     if(item == null) return NotFound();
        //     var units = await _context.Units.Where(f => f.ItemUnitPoint!.Item!.Id == item.Id).ToListAsync();
        //     foreach(var photo in item.Photos!)
        //     {
        //         var result = await _photoService.DeletePhotoAsync(photo.PublicId!);
        //         if(result.Error != null) return BadRequest(result.Error.Message);
        //     }

        //     if(item.PreviewPhoto != null){
        //         var result = await _photoService.DeletePhotoAsync(item.PreviewPhoto.PublicId!);
        //         if(result.Error != null) return BadRequest(result.Error.Message);
        //     }

        //     _context.Units.RemoveRange(units);
        //     _context.Items.Remove(item);
            
        //     if(await _context.SaveChangesAsync() > 0) return Ok();
        //     return BadRequest("Failed to delete item");
        // }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-main-photo/{itemId:int}")]
        public async Task<ActionResult<PhotoDTO>> AddMainPhoto(int itemId, IFormFile file)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            if(item == null) return NotFound();

            if(item.PreviewPhoto != null){
                var deletionResult = await _photoService.DeletePhotoAsync(item.PreviewPhoto.PublicId!);
                if(deletionResult.Error != null) return BadRequest(deletionResult.Error.Message);
            }

            var result = await _photoService.AddMainPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo 
            { 
                Url = result.SecureUri.AbsoluteUri, 
                PublicId = result.PublicId 
            };

            item.PreviewPhoto = photo;

            if(await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Failed to add photo");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-photo/{itemId:int}")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(int itemId, IFormFile file)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            if(item == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo 
            { 
                Url = result.SecureUri.AbsoluteUri, 
                PublicId = result.PublicId 
            };

            item.Photos!.Add(photo);

            if(await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Failed to add photo");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-photo/{itemId:int}-{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int itemId, int photoId)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            var photo = item.Photos!.FirstOrDefault(f => f.Id == photoId);
            if(photo == null) return NotFound();

            // var result = await _photoService.DeletePhotoAsync(photo.PublicId!);
            // if(result.Error != null) return BadRequest(result.Error.Message);
            item.Photos!.Remove(photo);
            
            if(await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to delete a photo");
        }
    }
}