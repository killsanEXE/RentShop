using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("dataset")]
        public async Task<ActionResult<DatasetDTO>> GetDataset()
        {
            var items = await _unitOfWork.ItemRepository.GetDatasetItemsAsync();
            if(items.Count() > 0)
            {
                return Ok(new DatasetDTO() {
                    Items = items.ToList()
                });
            }else
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("dataset")]
        public async Task<ActionResult> SetDataset([FromBody] DatasetDTO dto)
        {
            if(dto.Items == null) return BadRequest("Invalid json file");
            foreach(var item in dto.Items)
            {
                var mappedItem = _mapper.Map<Item>(item);
                var sameNameItems = await _unitOfWork.ItemRepository.GetItemsByNameAsync(mappedItem.Name!);
                if(sameNameItems.Count() == 1)
                {
                    var editedItem = sameNameItems.First();
                    editedItem.Description = mappedItem.Description;
                    editedItem.AgeRestriction = mappedItem.AgeRestriction;
                    editedItem.PricePerDay = mappedItem.PricePerDay;

                    foreach(var photo in mappedItem.Photos!)
                    {
                        if(!editedItem.Photos!.Any(f => f.PublicId == photo.PublicId))
                        {
                            editedItem.Photos!.Add(photo);
                        }
                    }
                }
                else{
                    _unitOfWork.ItemRepository.AddItem(mappedItem);
                }
            }
            if(await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to add item(s) to database");
        }
    }
}