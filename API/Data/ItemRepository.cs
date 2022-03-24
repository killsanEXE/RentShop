using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ItemRepository : IItemRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public ItemRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Item> GetItemByIdAsync(int Id)
        {
            return (await _context.Items
                // .Include(f => f.Units!)
                // .ThenInclude(f => f.Unit)
                // .Include(f => f.Units!)
                // .ThenInclude(f => f.Point)
                .Include(f => f.Photos)
                .Include(f => f.PreviewPhoto)
                .FirstOrDefaultAsync(f => f.Id == Id))!;
        }

        public async Task<ItemDTO> GetItemDTOByIdAsync(int Id)
        {
            var item = await _context.Items.Include(f => f.PreviewPhoto).Include(f => f.Photos).SingleOrDefaultAsync(f => f.Id == Id);

            var units = _context.Units.AsQueryable();
            units = units.OrderBy(f => f.Id);
            units = units.Where(f => f.ItemUnitPoint!.Item!.Id == Id);

            var QueriableUnitDTOs = units.ProjectTo<UnitDTO>(_mapper.ConfigurationProvider).AsNoTracking();
            var unitDtos = await QueriableUnitDTOs.ToListAsync();

            var itemDTO = _mapper.Map<ItemDTO>(item);
            itemDTO.Units = unitDtos;
            return itemDTO;
        }

        public async Task<PagedList<ItemDTO>> GetItemsAsync(UserParams userParams, int userAge)
        {
            var query = _context.Items.AsQueryable();
            query = query.Where(f => f.AgeRestriction <= userAge);
            return await PagedList<ItemDTO>.CreateAsync(
                query.ProjectTo<ItemDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, userParams.PageSize
            );
        }
    }
}