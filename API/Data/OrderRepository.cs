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
    public class OrderRepository : IOrderRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public OrderRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(f => f.DeliveryMan)
                .Include(f => f.DeliveryLocation)
                .SingleOrDefaultAsync(f => f.Id == id);
            return order ?? null!;
        }

        public async Task<PagedList<OrderDTO>> GetOrdersAsync(PaginationParams paginationParams)
        {
            var query = _context.Orders.AsQueryable();
            return await PagedList<OrderDTO>.CreateAsync(
                query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                paginationParams.PageNumber, paginationParams.PageSize
            );
        }
    }
}