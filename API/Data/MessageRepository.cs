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
    public class MessageRepository : IMessageRepository
    {
        readonly ApplicationContext _context;
        readonly IMapper _mapper;
        public MessageRepository(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return (await _context.Connections.FindAsync(connectionId))!;
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return (await _context.Groups.Include(c => c.Connections)
                .Where(c => c.Connections!.Any(f => f.ConnectionId == connectionId))
                .FirstOrDefaultAsync())!;
        }

        public async Task<Message> GetMessage(int id)
        {
            return (await _context.Messages.Include(f => f.Sender).Include(f => f.Recipient)
                .SingleOrDefaultAsync(f => f.Id == id))!;
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return (await _context.Groups
                .Include(f => f.Connections)
                .FirstOrDefaultAsync(f => f.Name == groupName))!;
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Where(
                    f => f.Recipient!.UserName == currentUsername
                    && f.Sender!.UserName == recipientUsername
                    || f.Recipient.UserName == recipientUsername
                    && f.Sender!.UserName == currentUsername
                ).OrderBy(m => m.MessageSend)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var undreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUsername == currentUsername).ToList();

            if(undreadMessages.Any())
            {
                foreach(var message in undreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<IEnumerable<GroupDTO>> GetGruopsForUser(string username)
        {
            // return await _context.Groups.Where(f => f.Username1 == username || f.Username2 == username).OrderBy(f => f.Name).ToListAsync();
            var query = _context.Groups.Include(f => f.User1).Include(f => f.User2).AsQueryable();
            query = query.Where(f => f.User1!.UserName == username || f.User2!.UserName == username);
            return await query.ProjectTo<GroupDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}