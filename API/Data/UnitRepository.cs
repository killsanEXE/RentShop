using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UnitRepository : IUnitRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public UnitRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            return await _context.Units
                .Include(f => f.ItemUnitPoint)
                .ThenInclude(f => f!.Point)
                .SingleOrDefaultAsync(f => f.Id == id) ?? new Unit() { Id = -1 };
        }

        public void AddUnit(Item item, Point point, Unit unit)
        {
            _context.ItemUnitPoints.Add(new() { Item = item, Point = point, Unit = unit});
        }
    }
}