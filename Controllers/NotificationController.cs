using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Hubs;
using RealTimeNotifications.Models;

namespace RealTimeNotifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            if (string.IsNullOrEmpty(notification.UserId))
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            }
            else
            {
                await _hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification);
            }

            return Ok(new
            {
                success = true,
                message = "Notification sent successfully",
                data = notification
            });
        }
    }
}
