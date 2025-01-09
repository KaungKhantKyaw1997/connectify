namespace RealTimeNotifications.Models
{
    public record Notification(string Title, string Message, string? UserId = null);
}
