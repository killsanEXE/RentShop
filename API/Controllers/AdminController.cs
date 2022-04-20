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
                System.Console.WriteLine(mappedItem.Name);

                _context.Items.Add(mappedItem);
            }
            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to add item(s) to database");
        }
    }
}