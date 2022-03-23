using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public UserRepository(ApplicationContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<ClientDTO>> GetClientsAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            return await PagedList<ClientDTO>.CreateAsync(
                query.ProjectTo<ClientDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, userParams.PageSize
            );
        }

        public int GetUserAge(string username)
        {
            return _context.Users.FirstOrDefault(f => f.UserName == username)!.DateOfBirth.CalculateAge();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return (await _context.Users.FindAsync(id))!;
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return (await _context.Users.SingleOrDefaultAsync(f => f.UserName == username))!;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}