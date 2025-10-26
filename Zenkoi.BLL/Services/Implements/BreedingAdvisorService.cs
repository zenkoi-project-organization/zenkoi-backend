using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.Implements
{
    public class BreedingAdvisorService : IBreedingAdvisorService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public BreedingAdvisorService(IConfiguration config)
        {
            _http = new HttpClient();
            _apiKey = config["OpenRouter:ApiKey"]!;
        }

        public async Task<AIBreedingResponseDTO> RecommendPairsAsync(BreedingRequestDTO request)
        {
            string prompt = BuildPrompt(request);

            var messages = new[]
            {
                new { role = "system", content = "Bạn là Smart Koi Breeder – chuyên gia di truyền cá Koi. Trả lời duy nhất bằng JSON hợp lệ." },
                new { role = "user", content = prompt }
            };

            var body = new
            {
                model = "qwen/qwen3-30b-a3b", // bạn có thể đổi sang "gpt-4o-mini" nếu dùng OpenAI
                messages,
                temperature = 0.6,
                max_tokens = 1500,
                response_format = new { type = "json_object" } // ép model trả JSON hợp lệ
            };

            // === Cấu hình header cho OpenRouter ===
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost:7087");
            _http.DefaultRequestHeaders.Add("X-Title", "Smart Koi Breeder");

            // === Gửi request đến OpenRouter ===
            var response = await _http.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🌐 Raw response từ OpenRouter:\n{content}");

            // === Xử lý phản hồi ===
            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                throw new Exception("Không có phản hồi từ mô hình AI.");

            var message = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(message))
                throw new Exception("AI trả về nội dung trống.");

            // === Thử parse JSON ===
            try
            {
                string jsonPart = ExtractJson(message!);

                var result = JsonSerializer.Deserialize<AIBreedingResponseDTO>(
                    jsonPart,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (result == null)
                    throw new Exception("Không thể deserialize JSON từ AI.");

                Console.WriteLine("✅ Parse JSON thành công.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi parse JSON: {ex.Message}");
                Console.WriteLine($"⚠️ Nội dung AI trả về:\n{message}");
                throw new Exception("Dữ liệu AI trả về không đúng định dạng JSON mong đợi.");
            }
        }

        private string BuildPrompt(BreedingRequestDTO request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"🎯 Mục tiêu phối giống:");
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Mẫu màu mong muốn: {request.DesiredPattern}");
            sb.AppendLine($"- Hình dáng cơ thể mong muốn: {request.DesiredBodyShape}");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Yêu cầu tối thiểu: HatchRate ≥ {request.MinHatchRate}, SurvivalRate ≥ {request.MinSurvivalRate}, HighQualifiedRate ≥ {request.MinHighQualifiedRate}");
            sb.AppendLine();

            if (request.PotentialParents == null || request.PotentialParents.Count < 2)
            {
                sb.AppendLine("⚠️ Dữ liệu chỉ có 1 hoặc 0 cá thể, không thể tạo cặp phối giống hợp lệ. Vui lòng trả về JSON sau:");
                sb.AppendLine("{ \"RecommendedPairs\": [], \"Message\": \"Không đủ cá thể để đề xuất cặp phối giống.\" }");
                return sb.ToString();
            }

            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng:");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} ({p.Variety}, {p.Gender})");
                sb.AppendLine($"  Size: {p.Size} cm, Health: {p.Health}");
                sb.AppendLine($"  Body: {p.BodyShape}, Pattern: {p.ColorPattern}");
            }

            sb.AppendLine();
            sb.AppendLine("📋 Hãy chọn 3–5 cặp ghép phù hợp nhất dựa trên các mục tiêu trên.");
            sb.AppendLine("Nếu không đủ dữ liệu hoặc chỉ có một cá thể => chỉ trả về JSON:");
            sb.AppendLine("{ \"RecommendedPairs\": [], \"Message\": \"Không đủ cá thể để đề xuất.\" }");
            sb.AppendLine();
            sb.AppendLine("🚫 Luôn trả **duy nhất JSON hợp lệ**, không có giải thích hay văn bản khác.");
            sb.AppendLine("📦 Cấu trúc JSON mong muốn:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    { \"MaleId\": 1, \"FemaleId\": 2, \"PatternMatchScore\": 0.95, \"Note\": \"Màu và hình dáng bổ trợ tốt.\" }");
            sb.AppendLine("  ],");
            sb.AppendLine("  \"Message\": \"Tóm tắt ngắn gọn lý do chọn lựa.\"");
            sb.AppendLine("}");

            return sb.ToString();
        }
        private static string ExtractJson(string input)
        {
            int start = input.IndexOf('{');
            int end = input.LastIndexOf('}');
            if (start >= 0 && end > start)
                return input.Substring(start, end - start + 1);
            throw new Exception("Không tìm thấy JSON hợp lệ trong phản hồi AI.");
        }
    }
}
