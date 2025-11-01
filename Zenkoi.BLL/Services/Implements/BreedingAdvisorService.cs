using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs;
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

            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia di truyền và phân tích phối giống cá Koi.");
            sb.AppendLine("Dựa vào dữ liệu thực tế bên dưới, hãy **chọn ra 3–5 cặp cá đực và cá cái tối ưu nhất** để đạt mục tiêu phối giống.");
            sb.AppendLine();
            sb.AppendLine("🚫 Không viết lý thuyết, không giải thích dài dòng.");
            sb.AppendLine("📦 Chỉ trả về JSON hợp lệ, bắt đầu bằng ký tự { và kết thúc bằng ký tự }.");
            sb.AppendLine("❌ Không dùng markdown, không ```json, không thêm văn bản ngoài JSON.");
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
            sb.AppendLine("- Với mỗi cặp, ước lượng các chỉ số trong khoảng 0–100 (%).");
            sb.AppendLine("- Nếu thiếu dữ liệu, đặt giá trị 0 thay vì \"unknown\".");
            sb.AppendLine("- ⚠️ Khi trả về kết quả, **phải dùng đúng hình ảnh (`image`) từ dữ liệu đầu vào**, không tạo ảnh mới.");
            sb.AppendLine();

            sb.AppendLine("📋 Kết quả cần trả về:");
            sb.AppendLine("- Trả về danh sách `RecommendedPairs` gồm 3–5 cặp tốt nhất, sắp xếp theo `Rank` giảm dần (Rank = 1 là tốt nhất).");
            sb.AppendLine("- Mỗi phần tử trong danh sách phải có đủ các trường sau:");
            sb.AppendLine("  • MaleId, MaleRFID, MaleImage (lấy từ cá đực gốc)");
            sb.AppendLine("  • FemaleId, FemaleRFID, FemaleImage (lấy từ cá cái gốc)");
            sb.AppendLine("  • Reason: Giải thích ngắn gọn, không trùng lặp.");
            sb.AppendLine("  • PredictedFertilizationRate, PredictedHatchRate, PredictedSurvivalRate, PredictedHighQualifiedRate, PatternMatchScore, BodyShapeCompatibility, PercentInbreeding, Rank");
            sb.AppendLine("- Tất cả các trường số phải là số thực (double) trong khoảng 0–100.");
            sb.AppendLine();

            sb.AppendLine("⚠️ Lưu ý quan trọng:");
            sb.AppendLine("- Luôn đảm bảo **MaleId / MaleRFID / MaleImage thuộc cá đực (Gender = Male)**.");
            sb.AppendLine("- Luôn đảm bảo **FemaleId / FemaleRFID / FemaleImage thuộc cá cái (Gender = Female)**.");
            sb.AppendLine("- Trường `PercentInbreeding` phải là **số thực (double)**. Nếu chưa có dữ liệu, đặt 0 thay vì \"unknown\".");
            sb.AppendLine("- Không tự tạo hoặc hoán đổi giới tính cá.");
            sb.AppendLine();

            // Cấu trúc JSON mẫu
            sb.AppendLine("📦 Cấu trúc JSON mẫu:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"MaleId\": 1,");
            sb.AppendLine("      \"MaleRFID\": \"RF1234\",");
            sb.AppendLine("      \"MaleImage\": \"(image từ cá đực)\",");
            sb.AppendLine("      \"FemaleId\": 16,");
            sb.AppendLine("      \"FemaleRFID\": \"RF5678\",");
            sb.AppendLine("      \"FemaleImage\": \"(image từ cá cái)\",");
            sb.AppendLine("      \"Reason\": \"Cặp này có màu và dáng phù hợp.\",");
            sb.AppendLine("      \"PredictedFertilizationRate\": 92.5,");
            sb.AppendLine("      \"PredictedHatchRate\": 88.1,");
            sb.AppendLine("      \"PredictedSurvivalRate\": 79.6,");
            sb.AppendLine("      \"PredictedHighQualifiedRate\": 82.0,");
            sb.AppendLine("      \"PatternMatchScore\": 94.2,");
            sb.AppendLine("      \"BodyShapeCompatibility\": 90.3,");
            sb.AppendLine("      \"PercentInbreeding\": 0.0,");
            sb.AppendLine("      \"Rank\": 1");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("⚠️ Chỉ trả JSON hợp lệ, không chứa ký tự đặc biệt, không ghi chú, không văn bản ngoài JSON.");

            return sb.ToString();
        }

        public async Task<AIPairAnalysisResponseDTO> AnalyzePairAsync(AIPairAnalysisRequestDTO request)
        {
            string prompt = BuildPairAnalysisPrompt(request);

            var messages = new[]
            {
                new { role = "system", content = "Bạn là Smart Koi Breeder – chuyên gia di truyền cá Koi. Trả lời duy nhất bằng JSON hợp lệ." },
                new { role = "user", content = prompt }
            };

            var body = new
            {
                model = "qwen/qwen3-30b-a3b", 
                messages,
                temperature = 0.2,
                max_tokens = 8000,
                response_format = new { type = "json_object" }
            };

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost:7087");
            _http.DefaultRequestHeaders.Add("X-Title", "Smart Koi Pair Analyzer");

            var response = await _http.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🌐 Raw response từ OpenRouter:\n{content}");

            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                throw new Exception("Không có phản hồi từ mô hình AI.");

            var message = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(message))
                throw new Exception("AI trả về nội dung trống.");

            try
            {
                string jsonPart = ExtractJson(message!);

                var result = JsonSerializer.Deserialize<AIPairAnalysisResponseDTO>(
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

        private string BuildPairAnalysisPrompt(AIPairAnalysisRequestDTO request)
        {

            bool maleHasData = request.Male != null &&
                      request.Male.BreedingHistory?.Any(h =>
                          h.FertilizationRate.HasValue ||
                          h.HatchRate.HasValue ||
                          h.SurvivalRate.HasValue ||
                          h.HighQualifiedRate.HasValue) == true;

            bool femaleHasData = request.Female != null &&
                                 request.Female.BreedingHistory?.Any(h =>
                                     h.FertilizationRate.HasValue ||
                                     h.HatchRate.HasValue ||
                                     h.SurvivalRate.HasValue ||
                                     h.HighQualifiedRate.HasValue) == true;

            if (!maleHasData || !femaleHasData)
            {
                throw new  InvalidOperationException(" Dữ liệu không đủ để phân tích. Vui lòng chọn cá trống hoặc cá máy đã có lịch sử sinh sản .");
            }

            var sb = new StringBuilder();

            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia di truyền cá Koi.");
            sb.AppendLine("Hãy phân tích khả năng phối giống giữa **một cặp cá đực và cá cái cụ thể** dựa trên dữ liệu thật bên dưới.");
            sb.AppendLine();
            sb.AppendLine("🎯 Mục tiêu:");
            sb.AppendLine("- Dự đoán độ tương thích và tiềm năng sinh sản của cặp cá này.");
            sb.AppendLine("- Đưa ra phân tích chi tiết và kết quả định lượng ở dạng JSON.");
            sb.AppendLine();
            sb.AppendLine("📊 Quy tắc đánh giá:");
            sb.AppendLine("- Dựa 100% vào dữ liệu thật, không được suy diễn ngẫu nhiên.");
            sb.AppendLine("- Tính điểm dựa trên 4 nhóm yếu tố có trọng số:");
            sb.AppendLine("  1️⃣ Giống và độ tương thích di truyền (40%)");
            sb.AppendLine("  2️⃣ Sức khỏe và độ tuổi sinh sản (25%)");
            sb.AppendLine("  3️⃣ Dáng và kích thước cơ thể tương đồng (20%)");
            sb.AppendLine("  4️⃣ Hiệu quả lịch sử sinh sản trung bình (15%)");
            sb.AppendLine("- Nếu giống khác nhau, trừ 30 điểm PatternMatchScore.");
            sb.AppendLine("- Nếu cá cái có Health = 'Warning' hoặc 'Bad', giảm 20 điểm FertilizationRate.");
            sb.AppendLine("- Nếu thiếu dữ liệu, đặt giá trị 0 thay vì 'unknown'.");
            sb.AppendLine("- Không được tạo dữ liệu giả hoặc giá trị tưởng tượng.");
            sb.AppendLine();

            sb.AppendLine("🐟 Cá đực (Male):");
            sb.AppendLine($"- ID: {request.Male.Id} | RFID: {request.Male.RFID} | Giống: {request.Male.Variety}");
            sb.AppendLine($"- Kích thước: {request.Male.Size} | Tuổi: {request.Male.Age} | Sức khỏe: {request.Male.Health}");
            sb.AppendLine($"- Hình dáng: {request.Male.BodyShape} | Màu sắc: {request.Male.ColorPattern}");
            sb.AppendLine($"- Ảnh: {request.Male.image}");
            if (request.Male.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Male.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, HQ={h.HighQualifiedRate}%, Partner={h.PartnerVariety}");
                }
            }
            sb.AppendLine();

            sb.AppendLine("🐠 Cá cái (Female):");
            sb.AppendLine($"- ID: {request.Female.Id} | RFID: {request.Female.RFID} | Giống: {request.Female.Variety}");
            sb.AppendLine($"- Kích thước: {request.Female.Size} | Tuổi: {request.Female.Age} | Sức khỏe: {request.Female.Health}");
            sb.AppendLine($"- Hình dáng: {request.Female.BodyShape} | Màu sắc: {request.Female.ColorPattern}");
            sb.AppendLine($"- Ảnh: {request.Female.image}");
            if (request.Female.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Female.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, HQ={h.HighQualifiedRate}%, Partner={h.PartnerVariety}");
                }
            }
            sb.AppendLine();

            sb.AppendLine("📋 Hãy phân tích và dự đoán **chính xác nhất có thể**, sau đó trả về JSON hợp lệ theo cấu trúc sau:");
            sb.AppendLine("{");
            sb.AppendLine($"  \"MaleId\": {request.Male.Id},");
            sb.AppendLine($"  \"FemaleId\": {request.Female.Id},");
            sb.AppendLine("  \"PredictedFertilizationRate\": 0.0,");
            sb.AppendLine("  \"PredictedHatchRate\": 0.0,");
            sb.AppendLine("  \"PredictedSurvivalRate\": 0.0,");
            sb.AppendLine("  \"PredictedHighQualifiedRate\": 0.0,");
            sb.AppendLine("  \"PatternMatchScore\": 0.0,");
            sb.AppendLine("  \"BodyShapeCompatibility\": 0.0,");
            sb.AppendLine("  \"PercentInbreeding\": 0.0,");
            sb.AppendLine("  \"Summary\": \"Phân tích ngắn gọn, bám sát dữ liệu thực tế (không quá 200 ký tự).\",");
            sb.AppendLine("  \"MaleBreedingInfo\": {");
            sb.AppendLine("    \"Summary\": \"Đánh giá tiềm năng sinh sản và phong độ của cá đực (tối đa 100 ký tự).\",");
            sb.AppendLine("    \"BreedingSuccessRate\": 0.0");
            sb.AppendLine("  },");
            sb.AppendLine("  \"FemaleBreedingInfo\": {");
            sb.AppendLine("    \"Summary\": \"Đánh giá tiềm năng sinh sản và sức khỏe của cá cái (tối đa 100 ký tự).\",");
            sb.AppendLine("    \"BreedingSuccessRate\": 0.0");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("⚠️ Yêu cầu bắt buộc:");
            sb.AppendLine("- Giữ nguyên MaleId và FemaleId như dữ liệu đầu vào.");
            sb.AppendLine("- Không bỏ sót hoặc đổi tên trường JSON.");
            sb.AppendLine("- Không thêm ghi chú, markdown hoặc văn bản ngoài JSON.");
            sb.AppendLine("- Mọi giá trị phải là số thực (double) trong khoảng 0–100.");
            sb.AppendLine("- Trả về đúng định dạng JSON, không có dấu * hoặc emoji.");

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

