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


        [HttpPost]
        [Authorize]
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


        [HttpGet]
        public async Task<IActionResult> GetAllPacketFishes(
            [FromQuery] PacketFishFilterRequestDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _packetFishService.GetAllPacketFishesAsync(filter, pageIndex, pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách gói cá: {ex.Message}");
            }
        }

        [Authorize]
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

        [Authorize]
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


        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePacketFishes([FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _packetFishService.GetAvailablePacketFishesAsync(pageIndex,pageSize);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách gói cá có sẵn: {ex.Message}");
            }
        }

        [HttpGet("by-size")]
        public async Task<IActionResult> GetPacketFishesBySize(
        [FromQuery] double minSize,
        [FromQuery] double maxSize,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                if (minSize < 0 || maxSize < 0 || minSize > maxSize)
                {
                    return GetError("Khoảng giá không hợp lệ");
                }
                var result = await _packetFishService.GetPacketFishesBySizeAsync(minSize,maxSize,pageIndex,pageSize);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy gói cá theo kích thước: {ex.Message}");
            }
        }

        [HttpGet("by-price")]
        public async Task<IActionResult> GetPacketFishesByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)

        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    return GetError("Khoảng giá không hợp lệ");
                }

                var result = await _packetFishService.GetPacketFishesByPriceRangeAsync(minPrice, maxPrice,pageIndex, pageSize);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy gói cá theo khoảng giá: {ex.Message}");
            }
        }
    }
}
