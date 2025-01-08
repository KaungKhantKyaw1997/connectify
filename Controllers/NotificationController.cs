using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
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
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            return Ok(new { Status = "Notification sent" });
        }
    }
}
