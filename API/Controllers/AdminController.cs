using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
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
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public AdminController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("dataset")]
        public async Task<ActionResult<DatasetDTO>> GetDataset()
        {
            var query = _context.Items.AsQueryable();
            var items = await query.ProjectTo<DatasetItemDTO>(_mapper.ConfigurationProvider).ToListAsync();
            if(items.Count > 0)
            {
                return Ok(new DatasetDTO() {
                    Items = items
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
                // var mappedItem = new Item() 
                // {
                //     Name = item.Name,
                //     Description = item.Description,
                //     PricePerDay = item.PricePerDay,
                //     AgeRestriction = item.AgeRestriction
                // };

                var mappedItem = _mapper.Map<Item>(item);
                var sameNameItems = await _context.Items.Include(f => f.Photos)
                    .Where(f => f.Name!.Trim().ToLower() == mappedItem.Name!.Trim().ToLower()).ToListAsync();

                System.Console.WriteLine(sameNameItems.Count);
                if(sameNameItems.Count == 1)
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
                    _context.Items.Add(mappedItem);
                }
            }
            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to add item(s) to database");
        }
    }
}