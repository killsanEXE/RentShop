using ChatAPI.Extensions;
using ChatAPI.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.SignalR
{
    public class PresenceHub : Hub
    {
        readonly PresenceTracker _tracker;
        readonly IMessageRepository _messageRepository;
        public PresenceHub(PresenceTracker tracker, IMessageRepository messageRepository)
        {
            _tracker = tracker;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User!.GetUsername();
            var isOnline = await _tracker.UserConnected(username, Context.ConnectionId);
            if(isOnline)
                await Clients.Others.SendAsync("UserIsOnline", username);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

            var groups = await _messageRepository.GetGruopsForUser(username);
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