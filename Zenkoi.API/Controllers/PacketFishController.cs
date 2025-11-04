using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacketFishController : BaseAPIController
    {
        private readonly IPacketFishService _packetFishService;

        public PacketFishController(IPacketFishService packetFishService)
        {
            _packetFishService = packetFishService;
        }

        /// <summary>
        /// Tạo gói cá mới
        /// </summary>
        /// <param name="packetFishRequestDTO">Thông tin gói cá</param>
        /// <returns>Gói cá đã tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePacketFish([FromBody] PacketFishRequestDTO packetFishRequestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _packetFishService.CreatePacketFishAsync(packetFishRequestDTO);
                return SaveSuccess(result, "Tạo gói cá thành công");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo gói cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy gói cá theo ID
        /// </summary>
        /// <param name="id">ID gói cá</param>
        /// <returns>Thông tin gói cá</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPacketFishById(int id)
        {
            try
            {
                var result = await _packetFishService.GetPacketFishByIdAsync(id);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy gói cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả gói cá
        /// </summary>
        /// <param name="isAvailable">Lọc theo trạng thái có sẵn (optional)</param>
        /// <param name="size">Lọc theo kích thước (optional)</param>
        /// <param name="minPrice">Giá tối thiểu (optional)</param>
        /// <param name="maxPrice">Giá tối đa (optional)</param>
        /// <returns>Danh sách gói cá</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPacketFishes(
            [FromQuery] string? search = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] FishSize? size = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] decimal? minAgeMonths = null,
            [FromQuery] decimal? maxAgeMonths = null,
            [FromQuery] int? minQuantity = null,
            [FromQuery] int? maxQuantity = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new PacketFishFilterRequestDTO
                {
                    Search = search,
                    IsAvailable = isAvailable,
                    Size = size,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinAgeMonths = minAgeMonths,
                    MaxAgeMonths = maxAgeMonths,
                    MinQuantity = minQuantity,
                    MaxQuantity = maxQuantity
                };

                var result = await _packetFishService.GetAllPacketFishesAsync(filter, pageIndex, pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách gói cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật gói cá
        /// </summary>
        /// <param name="id">ID gói cá</param>
        /// <param name="packetFishUpdateDTO">Thông tin cập nhật</param>
        /// <returns>Gói cá đã cập nhật</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePacketFish(int id, [FromBody] PacketFishUpdateDTO packetFishUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _packetFishService.UpdatePacketFishAsync(id, packetFishUpdateDTO);
                return SaveSuccess(result, "Cập nhật gói cá thành công");
            }
            catch (ArgumentException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật gói cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa gói cá
        /// </summary>
        /// <param name="id">ID gói cá</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePacketFish(int id)
        {
            try
            {
                var result = await _packetFishService.DeletePacketFishAsync(id);
                if (result)
                {
                    return SaveSuccess(new { message = "Xóa gói cá thành công" }, "Xóa gói cá thành công");
                }
                return GetNotFound("Không tìm thấy gói cá để xóa");
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa gói cá: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy các gói cá có sẵn
        /// </summary>
        /// <returns>Danh sách gói cá có sẵn</returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePacketFishes([FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _packetFishService.GetAvailablePacketFishesAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách gói cá có sẵn: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy gói cá theo kích thước
        /// </summary>
        /// <param name="size">Kích thước cá</param>
        /// <returns>Danh sách gói cá theo kích thước</returns>
        [HttpGet("by-size/{size}")]
        public async Task<IActionResult> GetPacketFishesBySize(FishSize size)
        {
            try
            {
                var result = await _packetFishService.GetPacketFishesBySizeAsync(size);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy gói cá theo kích thước: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy gói cá theo khoảng giá
        /// </summary>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <returns>Danh sách gói cá theo khoảng giá</returns>
        [HttpGet("by-price")]
        public async Task<IActionResult> GetPacketFishesByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    return GetError("Khoảng giá không hợp lệ");
                }

                var result = await _packetFishService.GetPacketFishesByPriceRangeAsync(minPrice, maxPrice);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy gói cá theo khoảng giá: {ex.Message}");
            }
        }
    }
}
