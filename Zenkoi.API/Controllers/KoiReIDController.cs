using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.KoiReIDDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KoiReIDController : BaseAPIController
    {
        private readonly IKoiReIDService _koiReIDService;

        public KoiReIDController(IKoiReIDService koiReIDService)
        {
            _koiReIDService = koiReIDService;
        }

        [Authorize]
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollFromCloudinary([FromBody] EnrollFromCloudinaryRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _koiReIDService.EnrollKoiFromCloudinaryAsync(
                    dto.KoiFishId,
                    dto.CloudinaryUrls,
                    UserId,
                    dto.Override
                );

                return SaveSuccess(result, $"Đã enroll {result.NumDownloaded}/{result.NumUrlsProvided} ảnh thành công.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi enroll cá: {ex.Message}");
            }
        }


        [Authorize]
        [HttpPost("enroll-from-video")]
        public async Task<IActionResult> EnrollFromVideo([FromBody] EnrollFromVideoRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _koiReIDService.EnrollKoiFromVideoAsync(
                    dto.KoiFishId,
                    dto.VideoUrl,
                    UserId,
                    dto.Override
                );

                return SaveSuccess(result, $"Đã enroll {result.NumFramesExtracted} frames từ video thành công.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi enroll cá từ video: {ex.Message}");
            }
        }

        /// <summary>
        /// Nhận diện cá từ Cloudinary URL
        /// </summary>
        [Authorize]
        [HttpPost("identify")]
        public async Task<IActionResult> IdentifyFromUrl([FromBody] IdentifyFromUrlRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _koiReIDService.IdentifyKoiFromUrlAsync(
                    dto.ImageUrl,
                    UserId
                );

                if (result.IsUnknown)
                {
                    return Success(result, "Không tìm thấy cá trùng khớp trong gallery.");
                }

                return Success(result, $"Nhận diện thành công: {result.IdentifiedAs} (Độ tin cậy: {result.Confidence:F2}%)");
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi nhận diện cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách enrollments
        /// </summary>
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollments(
            [FromQuery] int? koiFishId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _koiReIDService.GetEnrollmentsAsync(koiFishId, isActive, pageIndex, pageSize);
                var response = new PagingDTO<KoiGalleryEnrollmentResponseDTO>(result);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy danh sách enrollments: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy lịch sử nhận diện
        /// </summary>
        [HttpGet("identifications")]
        public async Task<IActionResult> GetIdentificationHistory(
            [FromQuery] int? koiFishId = null,
            [FromQuery] bool? isUnknown = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _koiReIDService.GetIdentificationHistoryAsync(koiFishId, isUnknown, pageIndex, pageSize);
                var response = new PagingDTO<KoiIdentificationResponseDTO>(result);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy lịch sử nhận diện: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy chi tiết 1 identification
        /// </summary>
        [HttpGet("identifications/{id}")]
        public async Task<IActionResult> GetIdentificationById(int id)
        {
            try
            {
                var result = await _koiReIDService.GetIdentificationByIdAsync(id);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy thông tin identification: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivate enrollment (xóa cá khỏi gallery)
        /// </summary>
        [Authorize]
        [HttpDelete("enrollments/{id}")]
        public async Task<IActionResult> DeactivateEnrollment(int id)
        {
            try
            {
                var result = await _koiReIDService.DeactivateEnrollmentAsync(id, UserId);
                return SaveSuccess(result, "Đã deactivate enrollment thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi deactivate enrollment: {ex.Message}");
            }
        }
    }
}
