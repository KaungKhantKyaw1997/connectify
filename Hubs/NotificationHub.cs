using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Models;

namespace RealTimeNotifications.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(Notification notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
