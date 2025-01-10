using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Real-Time Notifications API",
        Version = "v1",
        Description = "An API to send real-time notifications using SignalR",
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Real-Time Notifications API V1");
    options.RoutePrefix = string.Empty;
});

app.UseCors();

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://*:{port}");

app.MapHub<NotificationHub>("/notifications");

app.MapPost("/api/notifications/send", async (Notification notification, IHubContext<NotificationHub> hubContext) =>
{
    if (string.IsNullOrEmpty(notification.UserId))
    {
        await hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
    else
    {
        if (NotificationHub.TryGetConnectionId(notification.UserId, out string connectionId))
        {
            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotificationForUser", notification);
        }
        else
        {
            return Results.NotFound(new { success = false, message = "User not connected" });
        }
    }

    return Results.Ok(new
    {
        success = true,
        message = "Notification sent successfully",
        data = notification
    });
})
.WithName("SendNotification")
.WithOpenApi();

app.Run();
