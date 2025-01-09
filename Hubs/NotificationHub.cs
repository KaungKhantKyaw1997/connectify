using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Models;
using System.Collections.Concurrent;

namespace RealTimeNotifications.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        public override Task OnConnectedAsync()
        {
            string? userId = Context.GetHttpContext()?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var item = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!item.Equals(default(KeyValuePair<string, string>)))
            {
                UserConnections.TryRemove(item.Key, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string userId, Notification notification)
        {
            if (UserConnections.TryGetValue(userId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotificationForUser", notification);
            }
        }

        public async Task SendBroadcastNotification(Notification notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
