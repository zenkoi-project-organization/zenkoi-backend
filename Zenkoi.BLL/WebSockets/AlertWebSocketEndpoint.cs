using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.WebSockets
{

    public class AlertWebSocketEndpoint
    {
        private readonly WebSocketConnectionManager _wsManager;

        public AlertWebSocketEndpoint(WebSocketConnectionManager wsManager)
        {
            _wsManager = wsManager;
        }

        public async Task HandleAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("WebSocket request required");
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _wsManager.AddConnection(socket);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.ReceiveAsync(buffer, CancellationToken.None);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
