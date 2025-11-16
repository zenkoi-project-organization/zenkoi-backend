using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly List<WebSocket> _connections = new();

        public void AddConnection(WebSocket socket)
        {
            _connections.Add(socket);
        }

        public async Task BroadcastAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            foreach (var socket in _connections.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        // Ignore errors
                    }
                }
                else
                {
                    _connections.Remove(socket); // Loại bỏ socket đóng
                }
            }
        }
    }
}
