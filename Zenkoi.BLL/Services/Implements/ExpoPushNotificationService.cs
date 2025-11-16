using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Zenkoi.BLL.Services.Implements
{
    public class ExpoPushNotificationService
    {
        private readonly HttpClient _client;

        public ExpoPushNotificationService(HttpClient client)
        {
            _client = client;
        }

        public async Task SendPushNotificationAsync(string expoPushToken, string title, string body, object data = null)
        {
            var message = new
            {
                to = expoPushToken,
                sound = "default",
                title = title,
                body = body,
                data = data
            };

            var response = await _client.PostAsJsonAsync("https://exp.host/--/api/v2/push/send", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendPushNotificationToMultipleAsync(IEnumerable<string> expoPushTokens, string title, string body, object data = null)
        {
            foreach (var token in expoPushTokens)
            {
                try
                {
                    Console.WriteLine($"[MOCK PUSH] To: {string.Join(",", token)}, Title: {title}, Body: {body}");
                    await SendPushNotificationAsync(token, title, body, data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send push to {token}: {ex.Message}");
                }
            }
        }
        public async Task SendAsync(string toToken, string title, string body)
        {
            var message = new
            {
                to = toToken,
                sound = "default",
                title,
                body,
                data = new { test = "backend-only" }
            };

            var response = await _client.PostAsJsonAsync(
                "https://exp.host/--/api/v2/push/send",
                message
            );

            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Expo Response: " + result);
        }
    }
}