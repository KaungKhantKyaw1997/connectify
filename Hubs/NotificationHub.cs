using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Models;
using System.Collections.Concurrent;

namespace RealTimeNotifications.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        public static bool TryGetConnectionId(string userId, out string connectionId)
        {
            return UserConnections.TryGetValue(userId, out connectionId);
        }

        public override Task OnConnectedAsync()
        {
            string? userId = Context.GetHttpContext()?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("No userId provided for connection.");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var item = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!item.Equals(default(KeyValuePair<string, string>)))
            {
                UserConnections.TryRemove(item.Key, out _);
                Console.WriteLine($"User {item.Key} disconnected from connection ID: {Context.ConnectionId}");
            }

            if (exception != null)
            {
                Console.WriteLine($"An error occurred during disconnect: {exception.Message}");
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
