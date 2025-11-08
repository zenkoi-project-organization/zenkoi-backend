using Microsoft.Extensions.Configuration;
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

namespace Zenkoi.BLL.Services.Implements
{
    public class BreedingAdvisorService : IBreedingAdvisorService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public BreedingAdvisorService(IConfiguration config)
        {
            _http = new HttpClient();
            _apiKey = config["OpenRouter:ApiKey"]!;
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
                model = "qwen/qwen3-30b-a3b",
                messages,
                temperature = 0.6,
                max_tokens = 10000,
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

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
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
                model = "qwen/qwen3-30b-a3b",
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
            sb.AppendLine("Dựa trên dữ liệu thực tế được cung cấp, hãy **chọn ra 3–5 cặp cá đực và cá cái tối ưu nhất** để đạt mục tiêu phối giống.");
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
            sb.AppendLine($"- Giống mục tiêu: {request.TargetVariety}");
            sb.AppendLine($"- Loại đột biến mong muốn: {request.DesiredMutationType}");
            sb.AppendLine($"- Tỷ lệ đột biến mong muốn: {request.DesiredMutationRate}%");
            sb.AppendLine($"- Ưu tiên: {request.Priority}");
            sb.AppendLine($"- Ngưỡng yêu cầu: HatchRate ≥ {request.MinHatchRate}%, SurvivalRate ≥ {request.MinSurvivalRate}%, HighQualifiedRate ≥ {request.MinHighQualifiedRate}%");
            sb.AppendLine();

            // 🐟 Danh sách cá bố mẹ thực tế
            sb.AppendLine("🐟 Danh sách cá bố mẹ tiềm năng (nguồn dữ liệu thật):");
            foreach (var p in request.PotentialParents)
            {
                sb.AppendLine($"- ID {p.Id} | RFID: {p.RFID} | Giống: {p.Variety} | Giới tính: {p.Gender} | Kích thước: {p.Size} cm | Tuổi: {p.Age} | Sức khỏe: {p.Health}");
                sb.AppendLine($"  🧬 Đột biến: {(p.IsMutated ? $"{p.MutationType} ({p.MutationRate}%)" : "Không có")}");
                sb.AppendLine($"  🖼️ Hình ảnh: {p.image}");
                if (p.BreedingHistory?.Any() == true)
                {
                    foreach (var h in p.BreedingHistory)
                    {
                        sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, MutRate={h.MutationRate}%, CommonMut={h.CommonMutationType}, Note={h.ResultNote}");
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("📌 Ghi nhớ:");
            sb.AppendLine("- Chỉ được phép sử dụng các cá trong danh sách trên.");
            sb.AppendLine("- Mọi giá trị ID, RFID, Image phải trùng 100% với dữ liệu đầu vào.");
            sb.AppendLine("- Nếu không có dữ liệu phù hợp, có thể chọn cặp gần đúng nhất, nhưng phải ghi rõ lý do là 'tương tự' thay vì 'đúng loại'.");
            sb.AppendLine();

            // 📈 Nhiệm vụ chi tiết
            sb.AppendLine("📈 Nhiệm vụ của bạn:");
            sb.AppendLine("- Phân tích dữ liệu trên để **dự đoán hiệu quả phối giống** giữa các cặp cá đực và cá cái.");
            sb.AppendLine("- Ưu tiên cặp có khả năng sinh ra cá con đạt giống mục tiêu và có loại đột biến mong muốn.");
            sb.AppendLine("- Với mỗi cặp, ước lượng các chỉ số từ 0–100 (%). Nếu thiếu dữ liệu, đặt giá trị 0.");
            sb.AppendLine("- Khi tính `PredictedMutationRate`:");
            sb.AppendLine("  • Nếu chỉ một cá thể có đột biến, chỉ lấy khoảng 30–50% giá trị trung bình của đột biến đó.");
            sb.AppendLine("  • Nếu cả hai cùng có cùng loại đột biến, có thể đạt 70–90%.");
            sb.AppendLine();
            sb.AppendLine("🪶 Khi viết `Reason` (giải thích):");
            sb.AppendLine("- Hãy viết ngắn gọn (1–2 câu), nhưng mang phong cách **chuyên gia di truyền cá Koi**.");
            sb.AppendLine("- Giải thích lý do chọn cặp một cách tự nhiên, có cơ sở khoa học và mang tính thẩm mỹ.");
            sb.AppendLine("- Nên nhắc đến các yếu tố như: màu sắc, ánh kim, độ tương thích giống, sức khỏe, tỷ lệ sinh sản, hoặc di truyền ánh sáng.");
            sb.AppendLine("- Nếu không có đúng đột biến, có thể dùng các cụm như “tương tự ánh kim GinRin”, “gần với hiệu ứng Metallic” nhưng phải nói rõ đó là tương tự, không phải chính xác.");
            sb.AppendLine("- Tránh các cụm máy móc như 'phù hợp với mục tiêu', 'có tỷ lệ cao'; thay bằng cách diễn đạt sinh động hơn như 'có tiềm năng di truyền tốt', 'mang đặc tính ánh sáng mạnh', 'dòng máu ổn định' v.v.");
            sb.AppendLine("- Không viết dài, không liệt kê số liệu trong phần lý do.");
            sb.AppendLine();

            // 📋 Kết quả cần trả về
            sb.AppendLine("📋 Kết quả cần trả về:");
            sb.AppendLine("Trả về đối tượng JSON gồm `RecommendedPairs` là mảng 3–5 cặp tốt nhất, sắp xếp theo `Rank` tăng dần (1 là tốt nhất).");
            sb.AppendLine("Mỗi phần tử gồm:");
            sb.AppendLine("  • MaleId, MaleRFID, MaleImage, MaleIsMutated, MaleMutationType, MaleMutationRate");
            sb.AppendLine("  • FemaleId, FemaleRFID, FemaleImage, FemaleIsMutated, FemaleMutationType, FemaleMutationRate");
            sb.AppendLine("  • PredictedFertilizationRate, PredictedHatchRate, PredictedSurvivalRate, PredictedHighQualifiedRate");
            sb.AppendLine("  • PredictedMutationRate, PredictedCommonMutationType, PercentInbreeding, Rank");
            sb.AppendLine("  • Reason: Một hoặc hai câu, diễn đạt tự nhiên, chuyên nghiệp, mang ngôn ngữ của chuyên gia lai tạo.");
            sb.AppendLine();

            // ⚠️ Lưu ý quan trọng
            sb.AppendLine("⚠️ Lưu ý quan trọng:");
            sb.AppendLine("- Male phải là cá đực (Gender = Male). Female phải là cá cái (Gender = Female).");
            sb.AppendLine("- Không được ghép cùng giới tính hoặc tạo ID mới.");
            sb.AppendLine("- `PercentInbreeding` là double (0 nếu không có dữ liệu).");
            sb.AppendLine("- Ưu tiên đột biến trùng khớp với `DesiredMutationType`. Nếu không có, cho phép loại tương tự và ghi rõ 'gần đúng'.");
            sb.AppendLine("- Chỉ ghi lý do có thật, ngắn gọn, không trùng lặp giữa các cặp.");
            sb.AppendLine();

            // 📦 Mẫu JSON chuẩn
            sb.AppendLine("📦 Cấu trúc JSON mẫu hợp lệ:");
            sb.AppendLine("{");
            sb.AppendLine("  \"RecommendedPairs\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"MaleId\": 3,");
            sb.AppendLine("      \"MaleRFID\": \"KOI-003\",");
            sb.AppendLine("      \"MaleImage\": \"https://example.com/male.jpg\",");
            sb.AppendLine("      \"MaleIsMutated\": true,");
            sb.AppendLine("      \"MaleMutationType\": \"GinRin\",");
            sb.AppendLine("      \"MaleMutationRate\": 70.5,");
            sb.AppendLine();
            sb.AppendLine("      \"FemaleId\": 8,");
            sb.AppendLine("      \"FemaleRFID\": \"KOI-008\",");
            sb.AppendLine("      \"FemaleImage\": \"https://example.com/female.jpg\",");
            sb.AppendLine("      \"FemaleIsMutated\": false,");
            sb.AppendLine("      \"FemaleMutationType\": \"None\",");
            sb.AppendLine("      \"FemaleMutationRate\": 0.0,");
            sb.AppendLine();
            sb.AppendLine("      \"PredictedFertilizationRate\": 92.5,");
            sb.AppendLine("      \"PredictedHatchRate\": 88.1,");
            sb.AppendLine("      \"PredictedSurvivalRate\": 79.6,");
            sb.AppendLine("      \"PredictedHighQualifiedRate\": 82.0,");
            sb.AppendLine("      \"PredictedMutationRate\": 25.3,");
            sb.AppendLine("      \"PredictedCommonMutationType\": \"GinRin\",");
            sb.AppendLine("      \"PercentInbreeding\": 0.0,");
            sb.AppendLine("      \"Reason\": \"Cặp này có khả năng sinh ra cá con khỏe mạnh, mang đặc tính ánh kim tương tự GinRin.\",");
            sb.AppendLine("      \"Rank\": 1");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("⚠️ Chỉ trả về JSON hợp lệ, bắt đầu bằng { và kết thúc bằng }. Không thêm văn bản khác.");

            return sb.ToString();
        }




        private string BuildPairAnalysisPrompt(AIPairAnalysisRequestDTO request)
        {
            bool maleHasData = request.Male != null &&
                request.Male.BreedingHistory?.Any(h =>
                    h.FertilizationRate.HasValue ||
                    h.HatchRate.HasValue ||
                    h.SurvivalRate.HasValue ||
                    h.MutationRate.HasValue) == true;

            bool femaleHasData = request.Female != null &&
                request.Female.BreedingHistory?.Any(h =>
                    h.FertilizationRate.HasValue ||
                    h.HatchRate.HasValue ||
                    h.SurvivalRate.HasValue ||
                    h.MutationRate.HasValue) == true;

            if (!maleHasData || !femaleHasData)
                throw new InvalidOperationException("Dữ liệu không đủ để phân tích. Vui lòng chọn cá trống và cá mái có lịch sử sinh sản.");

            var sb = new StringBuilder();

            sb.AppendLine("Bạn là **Smart Koi Breeder**, chuyên gia di truyền cá Koi.");
            sb.AppendLine("Phân tích khả năng phối giống giữa **một cặp cá đực và cá cái cụ thể** dựa trên dữ liệu thật bên dưới.");
            sb.AppendLine("Hãy đánh giá toàn diện về **giống, sức khỏe, hình thể, hiệu quả sinh sản, và xu hướng di truyền đột biến (Mutation)**.");
            sb.AppendLine();
            sb.AppendLine("🎯 Mục tiêu:");
            sb.AppendLine("- Dự đoán độ tương thích, hiệu quả sinh sản, và khả năng sinh ra đời con mang loại đột biến mong muốn (DesiredMutationType).");
            sb.AppendLine("- Cung cấp kết quả định lượng (0–100%) và phần tóm tắt ngắn gọn, rõ ràng.");
            sb.AppendLine();
            sb.AppendLine("📊 Quy tắc đánh giá:");
            sb.AppendLine("- Chỉ dựa vào dữ liệu thật, không được suy diễn ngẫu nhiên.");
            sb.AppendLine("- Đánh giá dựa trên 6 yếu tố chính:");
            sb.AppendLine("  1️⃣ Giống và độ tương thích di truyền (30%)");
            sb.AppendLine("  2️⃣ Sức khỏe và độ tuổi sinh sản (15%)");
            sb.AppendLine("  3️⃣ Dáng và kích thước cơ thể tương đồng (15%)");
            sb.AppendLine("  4️⃣ Hiệu quả sinh sản trung bình (20%)");
            sb.AppendLine("  5️⃣ Ảnh hưởng đột biến (MutationType, MutationRate) (10%)");
            sb.AppendLine("  6️⃣ Khả năng sinh ra loại đột biến mong muốn (DesiredMutationType) (10%)");
            sb.AppendLine("- Nếu giống khác nhau, trừ 30 điểm PatternMatchScore.");
            sb.AppendLine("- Nếu cá cái có Health = 'Warning' hoặc 'Bad', giảm 20 điểm FertilizationRate.");
            sb.AppendLine("- Nếu thiếu dữ liệu, đặt giá trị 0 thay vì 'unknown'.");
            sb.AppendLine("- Không được thêm bất kỳ giải thích hoặc văn bản nào ngoài JSON.");
            sb.AppendLine();

            // 🧬 Cá đực
            sb.AppendLine("🐟 Cá đực (Male):");
            sb.AppendLine($"- ID: {request.Male.Id} | RFID: {request.Male.RFID} | Giống: {request.Male.Variety}");
            sb.AppendLine($"- Kích thước: {request.Male.Size} | Tuổi: {request.Male.Age} | Sức khỏe: {request.Male.Health}");
            sb.AppendLine($"- Đột biến: {(request.Male.IsMutated ? $"{request.Male.MutationType} ({request.Male.MutationRate}%)" : "Không có")}");
            sb.AppendLine($"- Ảnh: {request.Male.image}");
            if (request.Male.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Male.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, MutRate={h.MutationRate}%, CommonMut={h.CommonMutationType}, Note={h.ResultNote}");
                }
            }
            sb.AppendLine();

            // 🧬 Cá cái
            sb.AppendLine("🐠 Cá cái (Female):");
            sb.AppendLine($"- ID: {request.Female.Id} | RFID: {request.Female.RFID} | Giống: {request.Female.Variety}");
            sb.AppendLine($"- Kích thước: {request.Female.Size} | Tuổi: {request.Female.Age} | Sức khỏe: {request.Female.Health}");
            sb.AppendLine($"- Đột biến: {(request.Female.IsMutated ? $"{request.Female.MutationType} ({request.Female.MutationRate}%)" : "Không có")}");
            sb.AppendLine($"- Ảnh: {request.Female.image}");
            if (request.Female.BreedingHistory?.Any() == true)
            {
                foreach (var h in request.Female.BreedingHistory)
                {
                    sb.AppendLine($"  ↳ Lịch sử: Fert={h.FertilizationRate}%, Hatch={h.HatchRate}%, Surv={h.SurvivalRate}%, MutRate={h.MutationRate}%, CommonMut={h.CommonMutationType}, Note={h.ResultNote}");
                }
            }
            sb.AppendLine();

            sb.AppendLine("📋 Trả về kết quả **JSON hợp lệ duy nhất**, KHÔNG markdown, KHÔNG văn bản ngoài JSON.");
            sb.AppendLine("JSON phải khớp cấu trúc sau (giá trị mẫu chỉ để minh họa):");
            sb.AppendLine("{");
            sb.AppendLine($"  \"MaleId\": {request.Male.Id},");
            sb.AppendLine($"  \"FemaleId\": {request.Female.Id},");
            sb.AppendLine("  \"PredictedFertilizationRate\": 85.2,");
            sb.AppendLine("  \"PredictedHatchRate\": 78.6,");
            sb.AppendLine("  \"PredictedSurvivalRate\": 81.4,");
            sb.AppendLine("  \"PredictedHighQualifiedRate\": 76.9,");
            sb.AppendLine("  \"PredictedMutationRate\": 12.4,");
            sb.AppendLine("  \"PredictedCommonMutationType\": \"Doitsu\",");
            sb.AppendLine("  \"PredictedMatchToDesiredMutationType\": 90.3,");
            sb.AppendLine("  \"PatternMatchScore\": 88.5,");
            sb.AppendLine("  \"BodyShapeCompatibility\": 85.7,");
            sb.AppendLine("  \"PercentInbreeding\": 0.0,");
            sb.AppendLine("  \"Summary\": \"Cặp này tương thích cao, có khả năng sinh ra cá Doitsu khỏe mạnh.\",");

            sb.AppendLine("  \"MaleBreedingInfo\": {");
            sb.AppendLine("    \"Summary\": \"Cá đực có sức khỏe tốt, ổn định di truyền.\",");
            sb.AppendLine("    \"BreedingSuccessRate\": 84.0,");
            sb.AppendLine("    \"MutationInfluence\": 10.5");
            sb.AppendLine("  },");

            sb.AppendLine("  \"FemaleBreedingInfo\": {");
            sb.AppendLine("    \"Summary\": \"Cá cái có lịch sử nở tốt, ổn định.\",");
            sb.AppendLine("    \"BreedingSuccessRate\": 87.0,");
            sb.AppendLine("    \"MutationInfluence\": 8.0");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine("⚠️ Yêu cầu bắt buộc:");
            sb.AppendLine("- Không bỏ sót hoặc đổi tên trường JSON.");
            sb.AppendLine("- Không thêm markdown, emoji hoặc ký tự đặc biệt.");
            sb.AppendLine("- Mọi giá trị số là double trong khoảng 0–100.");
            sb.AppendLine("- Các trường enum (ví dụ MutationType) phải là chuỗi (string) hợp lệ, ví dụ: \"None\", \"Doitsu\", \"Ginrin\".");
            sb.AppendLine("- Nếu thiếu dữ liệu, đặt 0 thay vì null hoặc unknown.");
            sb.AppendLine("- Đảm bảo JSON bắt đầu bằng `{` và kết thúc bằng `}`.");

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
