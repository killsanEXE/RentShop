using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        readonly IMapper _mapper;
        readonly IHubContext<PresenceHub> _presenceHub;
        readonly PresenceTracker _tracker;
        readonly IUnitOfWork _unitOfWork;
        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper,
            IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _mapper = mapper;
            _presenceHub = presenceHub;
            _tracker = tracker;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext!.Request.Query["user"].ToString();
            string username = Context.User!.GetUsername();
            var groupName = GetGroupName(username, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName, username, otherUser);
            if(group.UnreadMessages)
            {
                if(group.LastMessageSender == otherUser) group.UnreadMessages = false;
            }
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User!.GetUsername(), otherUser);
            
            if(_unitOfWork.HasChanges()) await _unitOfWork.Complete(); 
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

            var callerConnections = await _tracker.GetConnectionsForUser(username);
            var callerGroups = await _unitOfWork.MessageRepository.GetGruopsForUser(username);
            await _presenceHub.Clients.Clients(callerConnections).SendAsync("ReceiveGroupsMessages", callerGroups);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name!).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User!.GetUsername();
            if(username == createMessageDTO.RecipientUsername!.ToLower()) 
                throw new HubException("You cannot send messages to yourself");
            
            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);
            if(recipient == null) throw new HubException("User was not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            group.LastMessageContent = createMessageDTO.Content;
            group.LastMessageSender = username;

            if(group.Connections!.Any(f => f.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                group.UnreadMessages = true;
                await _unitOfWork.Complete();
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                        new {username = sender.UserName, knownAs = sender.Name});

                    var recepientGroups = await _unitOfWork.MessageRepository.GetGruopsForUser(recipient.UserName);
                    await _presenceHub.Clients.Clients(connections).SendAsync("ReceiveGroupsMessages", recepientGroups);
                }
                var callerConnections = await _tracker.GetConnectionsForUser(username);
                var callerGroups = await _unitOfWork.MessageRepository.GetGruopsForUser(username);
                await _presenceHub.Clients.Clients(callerConnections).SendAsync("ReceiveGroupsMessages", callerGroups);
            }

            _unitOfWork.MessageRepository.AddMessage(message);
            
            if(await _unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
            }
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections!.FirstOrDefault(f => f.ConnectionId == Context.ConnectionId);
            _unitOfWork.MessageRepository.RemoveConnection(connection!);
            if(await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to remove form group");
        }

        private async Task<Group> AddToGroup(string groupName, string username1, string username2)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User!.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                group.User1 = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username1);
                group.User2 = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username2);
                _unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections!.Add(connection);
            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to join group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}