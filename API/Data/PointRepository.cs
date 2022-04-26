using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PointRepository : IPointRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public PointRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Point> GetPointByIdAsync(int id)
        {
            return await _context.Points.SingleOrDefaultAsync(f => f.Id == id) ?? new Point { Id = -1 };
        }

        public async Task<IEnumerable<PointDTO>> GetDTOPointsAsync()
        {
            var query = _context.Points.AsQueryable();
            return await query.ProjectTo<PointDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<PointDTO> GetPointDTO(int id)
        {
            var point = await _context.Points.FindAsync(id);
            return _mapper.Map<PointDTO>(point);            
        }

        public void AddPoint(Point point)
        {
            _context.Points.Add(point);
        }
    }
}