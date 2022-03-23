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
    public class PointController : BaseApiController
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        readonly IPhotoService _photoService;
        public PointController(ApplicationContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddPoint(LocationDTO locationDTO)
        {
            _context.Points.Add( _mapper.Map<Point>(locationDTO));
            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to add pick up point");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{currentPoint:int}")]
        public async Task<ActionResult> EditPoint(int currentPoint, LocationDTO locationDTO)
        {
            var point = await _context.Points.FindAsync(currentPoint);
            if(point == null) return NotFound();

            point.Address = locationDTO.Address;
            point.Country = locationDTO.Country;
            point.City = locationDTO.City;
            point.Floor = locationDTO.Floor;
            point.Apartment = locationDTO.Apartment;

            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to update a pick up point");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{currentPoint:int}-{moveTo:int}")]
        public async Task<ActionResult> DeletePoint(int currentPoint, int moveTo)
        {
            var point = await _context.Points.Include(f => f.Units).SingleOrDefaultAsync(f => f.Id == currentPoint);
            var anotherPoint = await _context.Points.FindAsync(moveTo);
            if(point == null || anotherPoint == null) return NotFound();

            if(point.PhotoUrl != null && point.PublicPhotoId != null)
            {
                var result = await _photoService.DeletePhotoAsync(point.PublicPhotoId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            foreach(var i in point.Units!)
            {
                i.Point = anotherPoint;
            }

            _context.Points.Remove(point);

            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to delete pick up point");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("set-photo/{id:int}")]
        public async Task<ActionResult> SetPointPhoto(int id, IFormFile file)
        {
            var point = await _context.Points.FindAsync(id);
            if(point == null) return NotFound();

            if(point.PhotoUrl != null && point.PublicPhotoId != null){
                var deleteResult = await _photoService.DeletePhotoAsync(point.PublicPhotoId!);
                if(deleteResult.Error != null) return BadRequest(deleteResult.Error.Message);
            }

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            point.PhotoUrl = result.Uri.AbsoluteUri;
            point.PublicPhotoId = result.PublicId;
            if(await _context.SaveChangesAsync() > 0) return Content(point.PhotoUrl);           
            return BadRequest("Failed to set photo");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointDTO>>> GetPoints()
        {
            var query = _context.Points.AsQueryable();
            var points = query.ProjectTo<PointDTO>(_mapper.ConfigurationProvider).AsNoTracking();
            return Ok(await points.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PointDTO>> GetPoint(int id)
        {
            var point = await _context.Points.FindAsync(id);
            if(point != null) return Ok(_mapper.Map<PointDTO>(point));            
            return NotFound();
        }
    }
}