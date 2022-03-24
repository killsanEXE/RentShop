using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UnitController : BaseApiController
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public UnitController(IUnitOfWork unitOfWork, ApplicationContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }
        

        [HttpPost("{itemId:int}")]
        public async Task<ActionResult<UnitDTO>> AddUnit(int itemId, UnitDTO unitDTO)
        {
            var item = await _context.Items
                .Include(f => f.Units)
                .Where(f => f.Id == itemId)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
            var point = await _context.Points
                .Include(f => f.Units)
                .Where(f => f.Id == unitDTO.PointId)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
                
            if(point == null || item == null) return NotFound();

            var unit = new Unit
            {
                Description = unitDTO.Description,
            };

            await _context.ItemUnitPoints.AddAsync(new() { Item = item, Point = point, Unit = unit});
            // item.Units!.Add(unit);
            // point.Units!.Add(unit);
            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<UnitDTO>(unit));
            return BadRequest("Failed to add unit");
        }

        [HttpPut("{itemId:int}-{unitId:int}")]
        public async Task<ActionResult<UnitDTO>> EditUnit(int itemId, int unitId, UnitDTO unitDTO)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            if(item == null) return NotFound();
            var unit = item.Units?.FirstOrDefault(f => f.Unit?.Id == unitId)?.Unit;
            if(unit == null) return NotFound();

            if(unit.IsAvaliable && unit.ItemUnitPoint!.Point!.Id != unitDTO.PointId)
            {
                var Point = _context.Points.FirstOrDefault(f => f.Id == unitDTO.PointId);
                if(Point != null){
                    unit.ItemUnitPoint.Point = Point;
                }
            }
            else if(!unit.IsAvaliable) return BadRequest("Failed to update pick up point");

            unit.Description = unitDTO.Description;
            if(await _context.SaveChangesAsync() > 0) return Ok(_mapper.Map<UnitDTO>(unit));
            return BadRequest("Failed to edit unit");
        }

        [HttpDelete("{itemId:int}-{unitId:int}")]
        public async Task<ActionResult> DeleteUnit(int itemId, int unitId)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            var itemUnitPoint = item.Units?.FirstOrDefault(f => f.Unit?.Id == unitId);
            var unit = itemUnitPoint?.Unit;
            if(item == null || unit == null || itemUnitPoint == null) return NotFound();

            _context.ItemUnitPoints.Remove(itemUnitPoint!);
            if(await _context.SaveChangesAsync() > 0) return Ok();
            return BadRequest("Failed to delete a unit");
        }
    }
}