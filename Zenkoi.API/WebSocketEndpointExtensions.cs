using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Zenkoi.BLL.WebSockets;


namespace Zenkoi.API.Extensions
{
    public static class WebSocketEndpointExtensions
    {
        public static void MapAlertWebSocket(this WebApplication app)
        {
            app.Map("api/ws/alerts", async context =>
            {
                var handler = context.RequestServices.GetRequiredService<AlertWebSocketEndpoint>();
                await handler.HandleAsync(context);
            });
        }
    }
}