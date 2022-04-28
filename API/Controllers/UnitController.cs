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
        readonly IMapper _mapper;
        public UnitController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        

        [HttpPost("{itemId:int}")]
        public async Task<ActionResult<UnitDTO>> AddUnit(int itemId, UnitDTO unitDTO)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByIdAsync(itemId);
            var point = await _unitOfWork.PointRepository.GetPointByIdAsync(unitDTO.PointId);
                
            if(point.Id == -1 || item.Id == -1) return NotFound();

            var unit = new Unit
            {
                Description = unitDTO.Description,
            };

            _unitOfWork.UnitRepository.AddUnit(item, point, unit);

            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<UnitDTO>(unit));
            return BadRequest("Failed to add unit");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UnitDTO>> EditUnit(int id, UnitDTO unitDTO)
        {

            var unit = await _unitOfWork.UnitRepository.GetUnitByIdAsync(id);
            if(unit.Id == -1) return NotFound();

            if(unit.IsAvailable && unit.ItemUnitPoint!.Point!.Id != unitDTO.PointId)
            {
                var point = await _unitOfWork.PointRepository.GetPointByIdAsync(unitDTO.PointId);
                if(point.Id != -1){
                    unit.ItemUnitPoint.Point = point;
                }
            }
            else if(!unit.IsAvailable && unit.ItemUnitPoint!.Point!.Id != unitDTO.PointId)
                return BadRequest("Failed to update pick up point");

            unit.Description = unitDTO.Description;
            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<UnitDTO>(unit));
            return BadRequest("Failed to edit unit");
        }

        [HttpPut("disable/{id:int}")]
        public async Task<ActionResult> DisableUnit(int id)
        {
            var unit = await _unitOfWork.UnitRepository.GetUnitByIdAsync(id);
            if(unit.Id == -1) return NotFound();
            unit.Disabled = true;
            await _unitOfWork.Complete();
            return Ok();
        }

        [HttpPut("enable/{id:int}")]
        public async Task<ActionResult> EnableUnit(int id)
        {
            var unit = await _unitOfWork.UnitRepository.GetUnitByIdAsync(id);
            if(unit.Id == -1) return NotFound();
            unit.Disabled = false;
            await _unitOfWork.Complete();
            return Ok();
        }
    }
}