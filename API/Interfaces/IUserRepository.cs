using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<PagedList<ClientDTO>> GetClientsAsync(UserParams userParams);
        Task<IEnumerable<DeliverymanDTO>> GetDeliverymansAsync();
        Task<AppUser> GetDeliverymanByUsernameAsync(string username);
        int GetUserAge(string username);
        Task<IEnumerable<DeliverymanDTO>> GetDeliverymanRequestsAsync();
        Task<IEnumerable<AppUser>> GetDeliverymansFromCountryAsync(string country);
    }
}