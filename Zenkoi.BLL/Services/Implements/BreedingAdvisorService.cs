using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class BreedingAdvisorService : IBreedingAdvisorService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBreedingProcessService _service;

        public BreedingAdvisorService(IConfiguration config, IUnitOfWork unitOfWork, IBreedingProcessService service)
        {
            _http = new HttpClient();
            _apiKey = config["OpenRouter:ApiKey"]!;
            _unitOfWork = unitOfWork;
            _service = service;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        

        }

        public async Task<AIBreedingResponseDTO> RecommendPairsAsync(BreedingRequestDTO request)
        {
            // Bảo đảm console hỗ trợ tiếng Việt & emoji
            Console.OutputEncoding = Encoding.UTF8;

            string prompt = BuildPrompt(request);

            var messages = new[]
            {
        new { role = "system", content = "Bạn là Smart Koi Breeder – chuyên gia di truyền cá Koi. Trả lời duy nhất bằng JSON hợp lệ." },
        new { role = "user", content = prompt }
    };

            var body = new
            {
                model = "openai/gpt-4.1-mini",
                messages,
                temperature = 0.6,
                max_tokens = 8000,
                response_format = new { type = "json_object" }
            };

            // === Header cấu hình OpenRouter ===
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost:7087");
            _http.DefaultRequestHeaders.Add("X-Title", "Smart Koi Breeder");

            // === Log & lưu dữ liệu gửi đến AI ===
            string bodyJson = JsonSerializer.Serialize(body, new JsonSerializerOptions { WriteIndented = true });
            string logDir = Path.Combine(AppContext.BaseDirectory, "Logs", "SmartKoi");
            Directory.CreateDirectory(logDir);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string promptPath = Path.Combine(logDir, $"Prompt_{timestamp}.txt");
            string bodyPath = Path.Combine(logDir, $"Body_{timestamp}.json");

            File.WriteAllText(promptPath, prompt, Encoding.UTF8);
            File.WriteAllText(bodyPath, bodyJson, Encoding.UTF8);

            Console.WriteLine("🧠 ==== DỮ LIỆU GỬI ĐẾN OPENROUTER ====");
            Console.WriteLine(bodyJson);
            Console.WriteLine("🧠 ==== NỘI DUNG PROMPT GỬI ====");
            Console.WriteLine(prompt);
            Console.WriteLine("📂 Đã lưu log vào:");
            Console.WriteLine($"   - {promptPath}");
            Console.WriteLine($"   - {bodyPath}");
            Console.WriteLine("===========================================================");

            // === Gửi request đến AI ===
            var response = await _http.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🌐 Raw response từ OpenRouter:\n{content}");

            // === Lấy JSON từ phản hồi AI ===
            var message = ExtractAiMessage(content);

            // === Parse kết quả JSON an toàn ===
            try
            {
                string jsonPart = SanitizeJson(message);
                var result = JsonSerializer.Deserialize<AIBreedingResponseDTO>(jsonPart, _jsonOptions);

                if (result == null)
                    throw new Exception("Không thể deserialize JSON từ AI.");

                foreach (var pair in result.RecommendedPairs)
                {

                    double Fx = await _service.GetOffspringInbreedingAsync(pair.MaleId, pair.FemaleId);

                    Fx = Math.Clamp(Fx, 0.0, 1.0);

                    pair.PercentInbreeding = Math.Round(Fx * 100, 2);
                }


                string resultPath = Path.Combine(logDir, $"Result_{timestamp}.json");
                File.WriteAllText(resultPath, JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }), Encoding.UTF8);

                Console.WriteLine($"✅ Parse JSON thành công — lưu tại: {resultPath}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi parse JSON: {ex.Message}");
                Console.WriteLine($"⚠️ Nội dung AI trả về:\n{message}");
                throw new Exception("Dữ liệu AI trả về không đúng định dạng JSON mong đợi.");
            }
        }



        public async Task<AIPairAnalysisResponseDTO> AnalyzePairAsync(AIPairAnalysisRequestDTO request)
        {
            string prompt = BuildPairAnalysisPrompt(request);

            var messages = new[]
          {
            new { role = "system", content = "Bạn là Smart Koi Breeder – chuyên gia di truyền cá Koi. Chỉ được sử dụng dữ liệu trong danh sách được cung cấp. Không được tạo thêm cá hoặc dữ liệu mới. Trả lời duy nhất bằng JSON hợp lệ." },
            new { role = "user", content = prompt }
        };

            var body = new
            {
                model = "openai/gpt-4.1-mini",
                messages,
                temperature = 0.3,
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

            var message = ExtractAiMessage(content);

            try
            {
                string jsonPart = SanitizeJson(message);
                var result = JsonSerializer.Deserialize<AIPairAnalysisResponseDTO>(jsonPart, _jsonOptions);


                if (result == null)
                    throw new Exception("Không thể deserialize JSON từ AI.");

                double Fx = await _service.GetOffspringInbreedingAsync(request.Male.Id, request.Female.Id);
                Fx = Math.Clamp(Fx, 0.0, 1.0);

                result.PercentInbreeding = Math.Round(Fx * 100, 2);
                result.MaleId = request.Male.Id;
                result.FemaleId = request.Female.Id;

                Console.WriteLine($"✅ Parse JSON thành công:\n{JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true })}");
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

            // 🧠 Giới thiệu & hướng dẫn cơ bản
            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia di truyền và phân tích phối giống cá Koi.");
            sb.AppendLine();
            sb.AppendLine("⚠️ Quy tắc bắt buộc:");
            sb.AppendLine("❗Không được tạo, giả định hoặc thêm bất kỳ cá nào không có trong danh sách đầu vào.");
            sb.AppendLine("❗Mọi ID, RFID, Image, và thông tin cá phải lấy NGUYÊN VẸN từ dữ liệu thật bên dưới.");
            sb.AppendLine("❗Chỉ ghép **cá đực (Gender = Male)** với **cá cái (Gender = Female)**.");
            sb.AppendLine("❗Không được chọn cùng một cá trong nhiều cặp trừ khi không thể khác được.");
            sb.AppendLine("❗Chỉ trả về **JSON hợp lệ**, không được có markdown hay văn bản ngoài JSON.");
            sb.AppendLine();

            // 🎯 Mục tiêu phối giống
            sb.AppendLine("🎯 Mục tiêu phối giống:");
            sb.AppendLine("Chúng tôi cần chọn những cặp cá có khả năng phối giống tối ưu để đạt được giống mục tiêu.");
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Ngưỡng yêu cầu: HatchRate ≥ {request.MinHatchRate}%, SurvivalRate ≥ {request.MinSurvivalRate}%");
            sb.AppendLine();

            // 🐟 Danh sách cá bố mẹ thực tế
            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng (nguồn dữ liệu thật):");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} | RFID: {p.RFID} | Giống: {p.Variety} | Giới tính: {p.Gender} | Kích thước: {p.Size} cm | Tuổi: {p.Age} | Sức khỏe: {p.Health}");
                sb.AppendLine($"  🧬 Đột biến: {(p.IsMutated ? $"{p.MutationDescription} " : "Không có")}");
                sb.AppendLine($"  🖼️ Hình ảnh: {p.image}");
                if (p.BreedingHistory?.Any() == true)
                {
                    foreach (var h in p.BreedingHistory)
                    {
                        sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, AveEggs={h.AvgEggs}, Note={h.ResultNote}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("📌 Ghi nhớ:");
            sb.AppendLine("- Chỉ được phép sử dụng các cá trong danh sách trên.");
            sb.AppendLine("- Mọi giá trị ID, RFID, Image phải trùng 100% với dữ liệu đầu vào.");
            sb.AppendLine("- Nếu không có dữ liệu phù hợp, có thể chọn cặp gần đúng nhất, nhưng phải ghi rõ lý do là 'tương tự' thay vì 'đúng loại'.");

            // 📈 Nhiệm vụ chi tiết
            sb.AppendLine("📈 Nhiệm vụ của bạn:");
            sb.AppendLine("- Phân tích dữ liệu trên để **dự đoán hiệu quả phối giống** giữa các cặp cá đực và cá cái.");
            sb.AppendLine("- **Chỉ chọn các cặp cá có tỷ lệ HatchRate, SurvivalRate và HighQualifiedRate đạt yêu cầu từ đầu vào.**");
            sb.AppendLine("- Ưu tiên cặp có khả năng sinh ra cá con đạt giống mục tiêu và có loại đột biến mong muốn.");
            sb.AppendLine("- Với mỗi cặp, ước lượng các chỉ số từ 0–100 (%). Nếu thiếu dữ liệu, đặt giá trị 0.");
            sb.AppendLine("- Khi tính `PredictedMutationRate`:");
            sb.AppendLine("  • Nếu chỉ một cá thể có đột biến, chỉ lấy khoảng 30–50% giá trị trung bình của đột biến đó.");
            sb.AppendLine("  • Nếu cả hai cùng có cùng loại đột biến, có thể đạt 70–90%.");
            sb.AppendLine();
            sb.AppendLine("🪶 Khi viết `Reason` (giải thích):");
            sb.AppendLine("- Hãy viết ngắn gọn (1–2 câu), nhưng mang phong cách **chuyên gia di truyền cá Koi**.");
            sb.AppendLine("- Giải thích lý do chọn cặp một cách tự nhiên, có cơ sở khoa học và mang tính thẩm mỹ.");

            // 📋 Kết quả cần trả về
            sb.AppendLine("📋 Kết quả cần trả về:");
            sb.AppendLine("Trả về đối tượng JSON gồm `RecommendedPairs` là mảng 3–5 cặp tốt nhất, sắp xếp theo `Rank` tăng dần (1 là tốt nhất).");
            sb.AppendLine("Mỗi phần tử gồm:");
            sb.AppendLine("  • MaleId, MaleRFID, MaleImage, MaleIsMutated, MaleMutationDescription, MaleMutationRate");
            sb.AppendLine("  • FemaleId, FemaleRFID, FemaleImage, FemaleIsMutated, FemaleMutationDescription, FemaleMutationRate");
            sb.AppendLine("  • PredictedFertilizationRate, PredictedHatchRate, PredictedSurvivalRate, PredictedHighQualifiedRate");
            sb.AppendLine("  • PredictedMutationRate, PredictedMutationDescription, PercentInbreeding, Rank");
            sb.AppendLine("  • Reason: Một hoặc hai câu, diễn đạt tự nhiên, chuyên nghiệp, mang ngôn ngữ của chuyên gia lai tạo.");

            sb.AppendLine();

            // ⚠️ Lưu ý quan trọng
            sb.AppendLine("⚠️ Lưu ý quan trọng:");
            sb.AppendLine("- Male phải là cá đực (Gender = Male). Female phải là cá cái (Gender = Female).");
            sb.AppendLine("- Không được ghép cùng giới tính hoặc tạo ID mới.");
            sb.AppendLine("- `PercentInbreeding` là double (0 nếu không có dữ liệu).");
            sb.AppendLine("- Ưu tiên đột biến trùng khớp với `DesiredMutationDescription`. Nếu không có, cho phép loại tương tự và ghi rõ 'gần đúng'.");

            // 📦 Mẫu JSON chuẩn
            sb.AppendLine("📋 Kết quả cần trả về:");
            sb.AppendLine("Trả về đối tượng JSON hợp lệ gồm mảng `RecommendedPairs`, mỗi phần tử tương ứng với 1 đối tượng kiểu `AIPairAnalysisResponseDTO` như sau:");
            sb.AppendLine("{");
            sb.AppendLine(" \"RecommendedPairs\": [");
            sb.AppendLine("   {");
            sb.AppendLine("     \"MaleId\": 3,");
            sb.AppendLine("     \"FemaleId\": 8,");
            sb.AppendLine("     \"PredictedFertilizationRate\": 92.5,");
            sb.AppendLine("     \"PredictedHatchRate\": 88.1,");
            sb.AppendLine("     \"PredictedSurvivalRate\": 79.6,");
            sb.AppendLine("     \"PredictedHighQualifiedRate\": 0.07,");
            sb.AppendLine("     \"PercentInbreeding\": 0.0,");
            sb.AppendLine("     \"PredictedMutationRate\": 25.3,");
            sb.AppendLine("     \"MutationDescription\": \"Đột biến ánh kim tương tự GinRin\",");
            sb.AppendLine("     \"PredictedMutationDescription\": 78.5,");
            sb.AppendLine("     \"Summary\": \"Cặp này có khả năng sinh ra cá con mang ánh sáng mạnh và di truyền ổn định.\",");
            sb.AppendLine("     \"MaleBreedingInfo\": {");
            sb.AppendLine("         \"Summary\": \"Cá đực sức khỏe tốt, từng đạt tỷ lệ nở cao trong các lần phối giống trước.\",");
            sb.AppendLine("         \"BreedingSuccessRate\": 85.3");
            sb.AppendLine("     },");
            sb.AppendLine("     \"FemaleBreedingInfo\": {");
            sb.AppendLine("         \"Summary\": \"Cá cái có nền di truyền ổn định, màu sắc sáng và tỷ lệ sống con cao.\",");
            sb.AppendLine("         \"BreedingSuccessRate\": 88.1");
            sb.AppendLine("     }");
            sb.AppendLine("   }");
            sb.AppendLine(" ]");
            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        private string BuildPairAnalysisPrompt(AIPairAnalysisRequestDTO request)
        {
            bool maleHasData = request.Male != null &&
                request.Male.BreedingHistory?.Any(h =>
                    h.FertilizationRate.HasValue ||
                    h.HatchRate.HasValue ||
                    h.SurvivalRate.HasValue) == true;

            bool femaleHasData = request.Female != null &&
                request.Female.BreedingHistory?.Any(h =>
                    h.FertilizationRate.HasValue ||
                    h.HatchRate.HasValue ||
                    h.SurvivalRate.HasValue) == true;

            int maleId = request.Male.Id;
            int fememaleId = request.Female.Id;

            // Check if data is missing for Male or Female
            if (!maleHasData || !femaleHasData)
            {
                return null;  // Return null if there's no data
            }

            var sb = new StringBuilder();

            // 🧠 Giới thiệu & mục tiêu
            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia di truyền cá Koi.");
            sb.AppendLine("Hãy phân tích chi tiết **khả năng phối giống của một cặp cá đực và cá cái** dựa trên dữ liệu thật bên dưới.");
            sb.AppendLine();
            sb.AppendLine("⚠️ Quy tắc bắt buộc:");
            sb.AppendLine("- Chỉ dùng dữ liệu thật, không được tạo, suy diễn hoặc thêm thông tin ngoài danh sách.");
            sb.AppendLine("- Mọi giá trị ID, RFID, Image phải giữ nguyên 100% như dữ liệu đầu vào.");
            sb.AppendLine("- Không thêm markdown, ký hiệu emoji hoặc văn bản ngoài JSON.");
            sb.AppendLine("- Mọi tỷ lệ (Rate, Percent, Match) nằm trong khoảng 0–100.");
            sb.AppendLine("- Nếu thiếu dữ liệu, đặt giá trị 0 thay vì null hoặc unknown.");
            sb.AppendLine();

            // 🧬 Thông tin cá đực
            sb.AppendLine("🐟 Cá đực (Male):");
            sb.AppendLine($"- ID: {request.Male.Id} | RFID: {request.Male.RFID} | Giống: {request.Male.Variety}");
            sb.AppendLine($"- Kích thước: {request.Male.Size} | Tuổi: {request.Male.Age} | Sức khỏe: {request.Male.Health}");
            sb.AppendLine($"- Đột biến: {(request.Male.IsMutated ? $"{request.Male.MutationDescription} " : "Không có")}");

            if (string.IsNullOrEmpty(request.Male.image)) return null;
            sb.AppendLine($"- Ảnh: {request.Male.image}");

            if (request.Male.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Male.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, AveEggs={h.AvgEggs}, Note={h.ResultNote}");
                }
            }
            sb.AppendLine();

            // 🧬 Thông tin cá cái
            sb.AppendLine("🐠 Cá cái (Female):");
            sb.AppendLine($"- ID: {request.Female.Id} | RFID: {request.Female.RFID} | Giống: {request.Female.Variety}");
            sb.AppendLine($"- Kích thước: {request.Female.Size} | Tuổi: {request.Female.Age} | Sức khỏe: {request.Female.Health}");
            sb.AppendLine($"- Đột biến: {(request.Female.IsMutated ? $"{request.Female.MutationDescription} " : "Không có")}");

            if (string.IsNullOrEmpty(request.Female.image)) return null;
            sb.AppendLine($"- Ảnh: {request.Female.image}");

            if (request.Female.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Female.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, AveEggs={h.AvgEggs}, Note={h.ResultNote}");
                }
            }
            sb.AppendLine();

            sb.AppendLine("📋 Trả về kết quả **JSON hợp lệ duy nhất**, KHÔNG markdown, KHÔNG văn bản ngoài JSON.");
            sb.AppendLine("JSON phải đúng cấu trúc sau (giá trị mẫu chỉ minh họa):");
            sb.AppendLine("{");

            
            sb.AppendLine($"  \"MaleId\": {maleId.ToString()}");
            sb.AppendLine($"  \"FemaleId\": {fememaleId.ToString()}");

        
            sb.AppendLine($"  \"PredictedFertilizationRate\": {(request.Male.BreedingHistory?.FirstOrDefault()?.FertilizationRate ?? (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"PredictedHatchRate\": {(request.Male.BreedingHistory?.FirstOrDefault()?.HatchRate ?? (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"PredictedSurvivalRate\": {(request.Male.BreedingHistory?.FirstOrDefault()?.SurvivalRate ?? (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"PredictedHighQualifiedRate\": {(request.Male.BreedingHistory?.FirstOrDefault()?.SurvivalRate ?? (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"PercentInbreeding\": null,");  
            sb.AppendLine($"  \"PredictedMutationRate\": {(request.Male.IsMutated ? (double?)12.4 : (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"MutationDescription\": {(request.Male.IsMutated ? "Đột biến ánh kim tương tự GinRin" : "null")},");
            sb.AppendLine($"  \"PredictedMutationDescription\": {(request.Male.IsMutated ? (double?)90.3 : (double?)null)?.ToString() ?? "null"},");
            sb.AppendLine($"  \"Summary\": \"{(request.Male.IsMutated ? "Cặp này tương thích tốt, có tiềm năng sinh ra cá con mang đặc tính ánh kim ổn định." : "null")}\",");

          
            sb.AppendLine("  \"MaleBreedingInfo\": {");
            sb.AppendLine($"    \"Summary\": \"Cá đực có sức khỏe tốt, ổn định di truyền và tỷ lệ sinh sản cao.\",");
            sb.AppendLine($"    \"BreedingSuccessRate\": {(maleHasData ? (double?)85.3 : (double?)null)?.ToString() ?? "null"}");
            sb.AppendLine($"    \"AvgFertilizationRate\": {request.Male.BreedingHistory[0].FertilizationRate}");
        //  sb.AppendLine($"    \"AveEggs\": null ");
            sb.AppendLine("  },");

            sb.AppendLine("  \"FemaleBreedingInfo\": {");
            sb.AppendLine($"    \"Summary\": \"Cá cái có lịch sử nở tốt, sức khỏe ổn định và màu sắc sáng.\",");
            sb.AppendLine($"    \"BreedingSuccessRate\": {(femaleHasData ? (double?)88.1 : (double?)null)?.ToString() ?? "null"}");
            //  sb.AppendLine($"    \"AvgFertilizationRate\": null ");
            sb.AppendLine($"    \"AvgEggs\": {request.Female.BreedingHistory?[0].AvgEggs ?? 0}");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine("⚠️ Lưu ý cuối cùng:");
            sb.AppendLine("- Không đổi tên trường JSON hoặc kiểu dữ liệu.");
            sb.AppendLine("- Không thêm các trường khác như PatternMatchScore hoặc PredictedCommonMutationType.");
            sb.AppendLine("- Đảm bảo JSON bắt đầu bằng `{` và kết thúc bằng `}`.");
            sb.AppendLine("- Summary phải là 1–2 câu tự nhiên, phong cách chuyên gia lai tạo cá Koi.");
            sb.AppendLine("- Nếu không có dữ liệu, trả về `null`.");

            return sb.ToString();
        }



        private static string ExtractAiMessage(string content)
        {
            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                throw new Exception("Không có phản hồi từ mô hình AI.");

            var message = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(message))
                throw new Exception("AI trả về nội dung trống.");

            return message!;
        }
        private static string SanitizeJson(string input)
        {
            string clean = input.Trim();

            // Regex tìm đoạn JSON chuẩn (từ { ... })
            var match = Regex.Match(clean, @"\{[\s\S]*\}");
            if (match.Success)
                clean = match.Value;

            clean = clean
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            if (clean.StartsWith("e."))
                clean = clean.Substring(2).Trim();

            return clean;
        }
    }
}
