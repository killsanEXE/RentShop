using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatAPI.DTOs;
using ChatAPI.Entities;

namespace ChatAPI.Interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void AddMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<IEnumerable<GroupDTO>> GetGruopsForUser(string username);
        Task<bool> Complete();
        bool HasChanges();
        Task<AppUser> GetUserByUsernameAsync(string username);
    }
}