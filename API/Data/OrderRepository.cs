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
            return await _context.Orders
                .Where(f => f.Id == id)
                .Include(f => f.Unit)
                .ThenInclude(f => f!.ItemUnitPoint!.Point)
                .Include(f => f.Client)
                .Include(f => f.DeliveryMan)
                .Include(f => f.DeliveryLocation)
                .Include(f => f.ReturnDeliveryman)
                .Include(f => f.ReturnFromLocation)
                .Include(f => f.ReturnPoint)
                .SingleOrDefaultAsync() ?? new() { Id = -1 };
        }

        public async Task<OrderDTO> GetOrderDTOByIdAsync(int id)
        {
            return await _context.Orders
                .AsQueryable()
                .Where(f => f.Id == id)
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? new OrderDTO() { Id = -1 };
        }

        public async Task<PagedList<OrderDTO>> GetOrdersAsync(PaginationParams paginationParams, bool showOnlyActive = false)
        {
            var query = _context.Orders.AsQueryable();
            if(!showOnlyActive) query = query.Where(f => !f.Cancelled && !f.UnitReturned);
            return await PagedList<OrderDTO>.CreateAsync(
                query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                paginationParams.PageNumber, paginationParams.PageSize
            );
        }

        public async Task<IEnumerable<Order>> GetActiveDeliveriesForDeliverymanAsync(string username)
        {
            return await _context.Orders.Include(f => f.DeliveryMan)
                .Include(f => f.ReturnDeliveryman)
                .Where(f => (
                    (f.DeliveryMan != null && f.DeliveryMan.UserName == username && !f.ClientGotDelivery) || 
                    (f.ReturnDeliveryman != null && f.ReturnDeliveryman.UserName == username)) 
                && (!f.Cancelled && !f.UnitReturned))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetAvailableOrdersAsync(AppUser deliveryman)
        {
            var query = _context.Orders
                .Include(f => f.DeliveryLocation)
                .Include(f => f.ReturnFromLocation)
                .Include(f => f.ReturnDeliveryman)
                .Include(f => f.DeliveryMan)
                .AsQueryable();
                
            query = query.Where(f => f.DeliveryMan == null || (f.ReturnFromLocation != null && f.ReturnDeliveryman == null));
            query = query.Where(f => (f.DeliveryLocation!.Country!.Trim().ToLower() == deliveryman.Location!.Country!.Trim().ToLower()) 
            || (f.ReturnFromLocation!.Country!.Trim().ToLower() == deliveryman.Location!.Country!.Trim().ToLower()));
            query = query.Where(f => !f.Cancelled && !f.UnitReturned);

            return await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetTakenOrdersAsync(AppUser user)
        {
            return await _context.Orders
                .Include(f => f.DeliveryMan)
                .Include(f => f.ReturnDeliveryman)
                .AsQueryable()
                .Where(f => f.Client!.Id != user.Id)
                .Where(f => (f.DeliveryMan != null && f.DeliveryMan.Id == user.Id && !f.ClientGotDelivery) 
                || (f.ReturnDeliveryman != null && f.ReturnDeliveryman.Id == user.Id && !f.UnitReturned))
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetClientOrdersAsync(string username)
        {
            return await _context.Orders
                .Include(f => f.Client)
                .OrderBy(f => f.Id)
                .AsQueryable()
                .Where(f => f.Client!.UserName == username && !f.Cancelled && !f.UnitReturned)
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }
    }
}