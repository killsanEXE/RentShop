using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<OrderDTO> GetOrderDTOByIdAsync(int id);
        Task<PagedList<OrderDTO>> GetOrdersAsync(PaginationParams paginationParams, bool showAll = false);
        Task<IEnumerable<Order>> GetActiveDeliveriesForDeliverymanAsync(string username);
        Task<IEnumerable<OrderDTO>> GetAvailableOrdersAsync(AppUser deliveryman);
        Task<IEnumerable<OrderDTO>> GetTakenOrdersAsync(AppUser user);
        Task<IEnumerable<OrderDTO>> GetClientOrdersAsync(string username);
        void AddOrder(Order order);
    }
}