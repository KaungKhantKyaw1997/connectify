using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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

app.MapPost("/api/notifications/send", async (Notification notification, IHubContext<NotificationHub> hubContext) =>
{
    if (string.IsNullOrEmpty(notification.UserId))
    {
        await hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
    else
    {
        await hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification);
    }

    return Results.Ok(new
    {
        success = true,
        message = "Notification sent successfully",
        data = notification
    });
});

app.Run();
