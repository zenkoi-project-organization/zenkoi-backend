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
                max_tokens = 15000,
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

            sb.AppendLine("Bạn là **Smart Koi Breeder** – chuyên gia phân tích di truyền và phối giống cá Koi.");
            sb.AppendLine("Nhiệm vụ của bạn là **phân tích dữ liệu thật bên dưới** để đưa ra gợi ý phối giống phù hợp nhất.");
            sb.AppendLine("🚫 Không được suy đoán, không viết lan man hoặc lý thuyết chung.");
            sb.AppendLine("📦 Chỉ trả về JSON hợp lệ, bắt đầu bằng ký tự { và kết thúc bằng } (không markdown, không ```json, không văn bản ngoài JSON).");
            sb.AppendLine();

            // 🎯 Mục tiêu phối giống
            sb.AppendLine("🎯 Mục tiêu phối giống:");
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Mẫu màu mong muốn: {request.DesiredPattern}");
            sb.AppendLine($"- Hình dáng cơ thể mong muốn: {request.DesiredBodyShape}");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Yêu cầu tối thiểu: HatchRate ≥ {request.MinHatchRate}, SurvivalRate ≥ {request.MinSurvivalRate}, HighQualifiedRate ≥ {request.MinHighQualifiedRate}");
            sb.AppendLine();

            // 🐟 Danh sách cá bố mẹ
            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng (có lịch sử phối giống thực tế):");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} | Giống: {p.Variety} | Giới tính: {p.Gender} | Kích thước: {p.Size} cm | Tuổi: {p.Age} | Sức khỏe: {p.Health}");
                sb.AppendLine($"  Hình dáng: {p.BodyShape} | Màu sắc: {p.ColorPattern}");
                if (p.BreedingHistory?.Any() == true)
                {
                    foreach (var h in p.BreedingHistory)
                    {
                        sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}, Hatch={h.HatchRate}, Surv={h.SurvivalRate}, HQ={h.HighQualifiedRate}, Cặp với giống {h.PartnerVariety}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("📈 Nhiệm vụ của bạn:");
            sb.AppendLine("- Phân tích dữ liệu trên để **dự đoán hiệu quả phối giống** giữa các cặp cá đực và cá cái.");
            sb.AppendLine("- Với mỗi cặp, hãy ước lượng các chỉ số dự đoán sau (0–1).");
            sb.AppendLine("- Nếu thiếu dữ liệu, hãy ghi rõ giá trị \"unknown\" thay vì gán bừa hoặc để 0.");
            sb.AppendLine();
            sb.AppendLine("  • PredictedFertilizationRate");
            sb.AppendLine("  • PredictedHatchRate");
            sb.AppendLine("  • PredictedSurvivalRate");
            sb.AppendLine("  • PredictedHighQualifiedRate");
            sb.AppendLine("  • PatternMatchScore");
            sb.AppendLine("  • BodyShapeCompatibility");
            sb.AppendLine("  • PercentInbreeding (nếu không có dữ liệu huyết thống thì đặt là \"unknown\")");
            sb.AppendLine();
            sb.AppendLine("📋 Kết quả trả về:");
            sb.AppendLine("- Chọn ra 3–5 cặp tốt nhất, sắp xếp theo `Rank` từ cao xuống thấp.");
            sb.AppendLine("- Mỗi cặp phải có phần `Reason` **ngắn gọn, khác nhau**, mô tả lý do chọn cặp đó, ví dụ:");
            sb.AppendLine("  * 'Cá đực có màu đỏ sâu, phù hợp với cá cái có nền trắng sáng.'");
            sb.AppendLine("  * 'Cặp này từng cho tỉ lệ sống cao và màu sắc hài hòa.'");
            sb.AppendLine("  * 'Cá đực có thân dài cân đối, bổ sung cho cá cái thân ngắn.'");
            sb.AppendLine("- Tuyệt đối không lặp lại cùng một lý do cho các cặp khác nhau.");
            sb.AppendLine("- Không giải thích thêm ngoài JSON.");
            sb.AppendLine();

            sb.AppendLine("🚫 Chỉ trả JSON hợp lệ (ASCII, không dấu, không ký tự đặc biệt).");
            sb.AppendLine("📦 Cấu trúc JSON mẫu:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"MaleId\": 1,");
            sb.AppendLine("      \"FemaleId\": 16,");
            sb.AppendLine("      \"Reason\": \"Cap nay co mau sac tuong dong va ty le no cao.\",");
            sb.AppendLine("      \"PredictedFertilizationRate\": 0.92,");
            sb.AppendLine("      \"PredictedHatchRate\": 0.86,");
            sb.AppendLine("      \"PredictedSurvivalRate\": 0.78,");
            sb.AppendLine("      \"PredictedHighQualifiedRate\": 0.80,");
            sb.AppendLine("      \"PatternMatchScore\": 0.94,");
            sb.AppendLine("      \"BodyShapeCompatibility\": 0.88,");
            sb.AppendLine("      \"PercentInbreeding\": 0.05,");
            sb.AppendLine("      \"Rank\": 1");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
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
