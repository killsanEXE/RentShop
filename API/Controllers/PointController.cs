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
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        readonly IPhotoService _photoService;
        public PointController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PointDTO>> AddPoint(LocationDTO locationDTO)
        {
            locationDTO = TrimStrings<LocationDTO>(locationDTO);
            var point = _mapper.Map<Point>(locationDTO);
            _unitOfWork.PointRepository.AddPoint(point);
            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<PointDTO>(point));
            return BadRequest("Failed to add pick up point");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{currentPoint:int}")]
        public async Task<ActionResult<PointDTO>> EditPoint(int currentPoint, LocationDTO locationDTO)
        {
            locationDTO = TrimStrings<LocationDTO>(locationDTO);
            var point = await _unitOfWork.PointRepository.GetPointByIdAsync(currentPoint);
            if(point == null) return NotFound();

            point.Address = locationDTO.Address;
            point.Country = locationDTO.Country;
            point.City = locationDTO.City;
            point.Floor = locationDTO.Floor;
            point.Apartment = locationDTO.Apartment;

            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<PointDTO>(point));
            return BadRequest("Failed to update a pick up point");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("disable/{id:int}")]
        public async Task<ActionResult> DisablePoint(int id)
        {
            var point = await _unitOfWork.PointRepository.GetPointByIdAsync(id);
            if(point == null) return NotFound();
            point.Disabled = true;
            await _unitOfWork.Complete();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("enable/{id:int}")]
        public async Task<ActionResult> EnablePoint(int id)
        {
            var point = await _unitOfWork.PointRepository.GetPointByIdAsync(id);
            if(point == null) return NotFound();
            point.Disabled = false;
            await _unitOfWork.Complete();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("set-photo/{id:int}")]
        public async Task<ActionResult> SetPointPhoto(int id, IFormFile file)
        {
            var point = await _unitOfWork.PointRepository.GetPointByIdAsync(id);
            if(point == null) return NotFound();

            if(point.PhotoUrl != null && point.PublicPhotoId != null){
                var deleteResult = await _photoService.DeletePhotoAsync(point.PublicPhotoId!);
                if(deleteResult.Error != null) return BadRequest(deleteResult.Error.Message);
            }

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            point.PhotoUrl = result.Uri.AbsoluteUri;
            point.PublicPhotoId = result.PublicId;
            if(await _unitOfWork.Complete()) return Content(point.PhotoUrl);           
            return BadRequest("Failed to set photo");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointDTO>>> GetPoints()
        {
            return Ok(await _unitOfWork.PointRepository.GetDTOPointsAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PointDTO>> GetPoint(int id)
        {
            return Ok(await _unitOfWork.PointRepository.GetPointDTO(id));
        }

        T TrimStrings<T>(T obj) where T: class
        {
            var stringProperties = obj.GetType().GetProperties()
                          .Where(p => p.PropertyType == typeof (string));
            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string) stringProperty.GetValue(obj, null)!;
                if(currentValue != null) stringProperty.SetValue(obj, currentValue.Trim(), null);
            }
            return obj;
        }
    }
}