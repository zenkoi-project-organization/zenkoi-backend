using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFavoriteDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KoiFavoriteController : BaseAPIController
    {
        private readonly IKoiFavoriteService _koiFavoriteService;

        public KoiFavoriteController(IKoiFavoriteService koiFavoriteService)
        {
            _koiFavoriteService = koiFavoriteService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] KoiFavoriteRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _koiFavoriteService.AddFavoriteAsync(UserId, dto.KoiFishId);
                return SaveSuccess(result, "Đã thêm cá vào danh sách yêu thích.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi thêm yêu thích: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{koiFishId}")]
        public async Task<IActionResult> RemoveFavorite(int koiFishId)
        {
            try
            {
                var result = await _koiFavoriteService.RemoveFavoriteAsync(UserId, koiFishId);
                return SaveSuccess(result, "Đã xóa cá khỏi danh sách yêu thích.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi xóa yêu thích: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("check/{koiFishId}")]
        public async Task<IActionResult> IsFavorite(int koiFishId)
        {
            try
            {
                var isFavorite = await _koiFavoriteService.IsFavoriteAsync(UserId, koiFishId);
                return GetSuccess(new { isFavorite });
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi kiểm tra yêu thích: {ex.Message}");
            }
        }
    }
}
