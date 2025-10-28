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

            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia phân tích di truyền và phối giống cá Koi.");
            sb.AppendLine("Hãy dựa vào dữ liệu thực tế bên dưới để **chọn ra 3–5 cặp cá đực và cá cái tối ưu nhất**.");
            sb.AppendLine("🚫 Không viết lý thuyết, không giải thích dài dòng.");
            sb.AppendLine("📦 Chỉ trả về JSON hợp lệ, bắt đầu bằng ký tự { và kết thúc bằng } (không markdown, không ```json, không văn bản ngoài JSON).");
            sb.AppendLine();

            // 🎯 Mục tiêu phối giống
            sb.AppendLine("🎯 Mục tiêu phối giống:");
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Mẫu màu mong muốn: {request.DesiredPattern}");
            sb.AppendLine($"- Hình dáng cơ thể mong muốn: {request.DesiredBodyShape}");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Ngưỡng yêu cầu: HatchRate ≥ {request.MinHatchRate}%, SurvivalRate ≥ {request.MinSurvivalRate}%, HighQualifiedRate ≥ {request.MinHighQualifiedRate}%");
            sb.AppendLine();

            // 🐟 Danh sách cá bố mẹ
            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng (có lịch sử phối giống):");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} | RFID: {p.RFID} | Giống: {p.Variety} | Giới tính: {p.Gender} | Kích thước: {p.Size} cm | Tuổi: {p.Age} | Sức khỏe: {p.Health}");
                sb.AppendLine($"  Hình dáng: {p.BodyShape} | Màu sắc: {p.ColorPattern}");
                sb.AppendLine($"  🖼️ Hình ảnh: {p.image}");
                if (p.BreedingHistory?.Any() == true)
                {
                    foreach (var h in p.BreedingHistory)
                    {
                        sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, HQ={h.HighQualifiedRate}%, Cặp với giống {h.PartnerVariety}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("📈 Nhiệm vụ của bạn:");
            sb.AppendLine("- Phân tích dữ liệu trên để **dự đoán hiệu quả phối giống** giữa các cặp cá đực và cá cái.");
            sb.AppendLine("- Với mỗi cặp, hãy ước lượng các chỉ số **trong khoảng 0–100 (%)**.");
            sb.AppendLine("- Nếu thiếu dữ liệu, ghi rõ giá trị \"unknown\" thay vì đoán hoặc để 0.");
            sb.AppendLine("- ⚠️ Khi trả về kết quả, **sử dụng đúng đường dẫn ảnh (`image`) của từng cá thể từ dữ liệu đầu vào** thay vì tạo ảnh giả.");
            sb.AppendLine();

            sb.AppendLine("📋 Kết quả trả về:");
            sb.AppendLine("- Trả về danh sách `RecommendedPairs` gồm 3–5 cặp tốt nhất, sắp xếp theo `Rank` từ cao xuống thấp.");
            sb.AppendLine("- Mỗi cặp gồm thông tin sau:");
            sb.AppendLine("  • MaleId, MaleRFID, MaleImage (lấy từ cá đực gốc)");
            sb.AppendLine("  • FemaleId, FemaleRFID, FemaleImage (lấy từ cá cái gốc)");
            sb.AppendLine("  • Reason: Giải thích ngắn gọn, không trùng lặp.");
            sb.AppendLine("  • Các chỉ số dự đoán: PredictedFertilizationRate, PredictedHatchRate, PredictedSurvivalRate, PredictedHighQualifiedRate, PatternMatchScore, BodyShapeCompatibility, PercentInbreeding, Rank");
            sb.AppendLine("- Không viết thêm bất kỳ văn bản nào ngoài JSON.");
            sb.AppendLine();

            sb.AppendLine("📦 Cấu trúc JSON mẫu:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"MaleId\": 1,");
            sb.AppendLine("      \"MaleRFID\": \"RF1234\",");
            sb.AppendLine("      \"MaleImage\": \"(sử dụng image từ cá đực)\",");
            sb.AppendLine("      \"FemaleId\": 16,");
            sb.AppendLine("      \"FemaleRFID\": \"RF5678\",");
            sb.AppendLine("      \"FemaleImage\": \"(sử dụng image từ cá cái)\",");
            sb.AppendLine("      \"Reason\": \"Cặp này có màu và dáng phù hợp.\",");
            sb.AppendLine("      \"PredictedFertilizationRate\": 92.5,");
            sb.AppendLine("      \"PredictedHatchRate\": 88.1,");
            sb.AppendLine("      \"PredictedSurvivalRate\": 79.6,");
            sb.AppendLine("      \"PredictedHighQualifiedRate\": 82.0,");
            sb.AppendLine("      \"PatternMatchScore\": 94.2,");
            sb.AppendLine("      \"BodyShapeCompatibility\": 90.3,");
            sb.AppendLine("      \"PercentInbreeding\": \"unknown\",");
            sb.AppendLine("      \"Rank\": 1");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("⚠️ Chỉ trả JSON hợp lệ, không chứa ký tự đặc biệt, không thêm văn bản ngoài JSON.");

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
