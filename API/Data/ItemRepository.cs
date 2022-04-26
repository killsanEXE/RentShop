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
                .Include(f => f.Photos)
                .Include(f => f.PreviewPhoto)
                .FirstOrDefaultAsync(f => f.Id == Id))!;
        }

        public async Task<ItemDTO> GetItemDTOByIdAsync(int Id, int age, bool admin)
        {
            Item? item;
            if(admin){
                item = await _context.Items
                    .Include(f => f.PreviewPhoto)
                    .Include(f => f.Photos)
                    .SingleOrDefaultAsync(f => f.Id == Id);
            }
            else
            {
                item = await _context.Items
                    .Include(f => f.PreviewPhoto)
                    .Include(f => f.Photos)
                    .SingleOrDefaultAsync(f => f.Id == Id && !f.Disabled && f.AgeRestriction <= age);
            } 

            if(item == null) return null!;

            var units = _context.Units.AsQueryable();
            units = units.OrderBy(f => f.Id).Where(f => f.ItemUnitPoint!.Item!.Id == Id);

            var QueriableUnitDTOs = units.ProjectTo<UnitDTO>(_mapper.ConfigurationProvider).AsNoTracking();
            var unitDtos = await QueriableUnitDTOs.ToListAsync();

            var itemDTO = _mapper.Map<ItemDTO>(item);
            itemDTO.Units = unitDtos;
            return itemDTO;
        }

        public async Task<PagedList<ItemDTO>> GetItemsAsync(UserParams userParams, int userAge, bool admin)
        {
            var query = _context.Items.AsQueryable();
            query = query.OrderBy(f => f.Id);
            if(!admin){
                query = query.Where(f => f.AgeRestriction <= userAge && !f.Disabled);
            }
            return await PagedList<ItemDTO>.CreateAsync(
                query.ProjectTo<ItemDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, userParams.PageSize
            );
        }

        public async Task<IEnumerable<DatasetItemDTO>> GetDatasetItemsAsync()
        {
            var query = _context.Items.AsQueryable();
            var items = await query.ProjectTo<DatasetItemDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return items;
        }

        public async Task<IEnumerable<Item>> GetItemsByNameAsync(string name)
        {
            return await _context.Items.Include(f => f.Photos)
                    .Where(f => f.Name!.Trim().ToLower() == name.Trim().ToLower()).ToListAsync();
        }

        public void AddItem(Item item)
        {
            _context.Items.Add(item);
        }
    }
}