using Microsoft.AspNetCore.SignalR;
using RealTimeNotifications.Hubs;
using RealTimeNotifications.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true);
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<NotificationHub>("/notifications");

app.MapPost("/send-notification", async (Notification notification, IHubContext<NotificationHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("ReceiveNotification", $"{notification.Title}: {notification.Message}");
    return Results.Ok(new { Status = "Notification sent" });
});

app.Run();
