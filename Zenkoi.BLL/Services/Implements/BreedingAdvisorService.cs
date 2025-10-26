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
                max_tokens = 8000,
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
            sb.AppendLine("🎯 Mục tiêu phối giống:");
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Mẫu màu mong muốn: {request.DesiredPattern}");
            sb.AppendLine($"- Hình dáng cơ thể mong muốn: {request.DesiredBodyShape}");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Yêu cầu tối thiểu: HatchRate ≥ {request.MinHatchRate}, SurvivalRate ≥ {request.MinSurvivalRate}, HighQualifiedRate ≥ {request.MinHighQualifiedRate}");
            sb.AppendLine();

            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng (có lịch sử phối giống thực tế):");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} | {p.Variety} ({p.Gender}) | {p.Size} cm | Age: {p.Age} | Health: {p.Health}");
                sb.AppendLine($"  Body: {p.BodyShape}, Pattern: {p.ColorPattern}");
                if (p.BreedingHistory?.Any() == true)
                {
                    foreach (var h in p.BreedingHistory)
                    {
                        sb.AppendLine($"  ↳ History: Fert={h.FertilizationRate}, Hatch={h.HatchRate}, Surv={h.SurvivalRate}, HQ={h.HighQualifiedRate}, Partner={h.PartnerVariety}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("📈 Nhiệm vụ của bạn:");
            sb.AppendLine("- Phân tích các dữ liệu trên để **dự đoán hiệu quả phối giống** giữa các cặp cá đực và cái.");
            sb.AppendLine("- Với mỗi cặp, hãy ước lượng các chỉ số dự đoán sau (0–1).");
            sb.AppendLine("- Nếu thiếu dữ liệu để tính toán hoặc không thể xác định, hãy ghi rõ giá trị \"unknown\" thay vì tự phỏng đoán hoặc gán số 0.");
            sb.AppendLine();
            sb.AppendLine("  • PredictedFertilizationRate");
            sb.AppendLine("  • PredictedHatchRate");
            sb.AppendLine("  • PredictedSurvivalRate");
            sb.AppendLine("  • PredictedHighQualifiedRate");
            sb.AppendLine("  • PatternMatchScore");
            sb.AppendLine("  • BodyShapeCompatibility");
            sb.AppendLine("  • PercentInbreeding (nếu không có dữ liệu huyết thống thì đặt là \"unknown\")");
            sb.AppendLine();
            sb.AppendLine("📋 Trả về JSON gồm 3–5 cặp phù hợp nhất, sắp xếp theo `rank` (tốt nhất đến thấp nhất).");
            sb.AppendLine("⚠️ Khi trả JSON, hãy chỉ dùng ký tự ASCII (không dấu, không emoji). Nếu có tiếng Việt, hãy loại bỏ dấu để đảm bảo JSON hợp lệ.");
            sb.AppendLine();
            sb.AppendLine("🚫 Chỉ trả JSON hợp lệ, không văn bản giải thích.");
            sb.AppendLine("📦 Cấu trúc JSON mẫu:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"MaleId\": 1,");
            sb.AppendLine("      \"FemaleId\": 16,");
            sb.AppendLine("      \"Reason\": \"Cặp này có tỉ lệ phối cao, màu phù hợp.\",");
            sb.AppendLine("      \"PredictedFertilizationRate\": 0.92,");
            sb.AppendLine("      \"PredictedHatchRate\": 0.86,");
            sb.AppendLine("      \"PredictedSurvivalRate\": 0.78,");
            sb.AppendLine("      \"PredictedHighQualifiedRate\": 0.80,");
            sb.AppendLine("      \"PatternMatchScore\": 0.94,");
            sb.AppendLine("      \"BodyShapeCompatibility\": 0.88,");
            sb.AppendLine("      \"PercentInbreeding\": 0.05,");
            sb.AppendLine("      \"Rank\": 1");
            sb.AppendLine("    }");
            sb.AppendLine("  ],");
      //    sb.AppendLine("  \"Message\": \"Dự đoán hoàn tất. Ưu tiên cặp Kohaku thuần với màu đỏ đều.\"");
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
