using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        readonly PresenceTracker _tracker;
        readonly IUnitOfWork _unitOfWork;
        public PresenceHub(PresenceTracker tracker, IUnitOfWork unitOfWork)
        {
            _tracker = tracker;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User!.GetUsername();
            var isOnline = await _tracker.UserConnected(username, Context.ConnectionId);
            if(isOnline)
                await Clients.Others.SendAsync("UserIsOnline", username);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

            var groups = await _unitOfWork.MessageRepository.GetGruopsForUser(username);
            await Clients.Caller.SendAsync("ReceiveGroupsMessages", groups);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User!.GetUsername(), Context.ConnectionId);
            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User!.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }
    }
}