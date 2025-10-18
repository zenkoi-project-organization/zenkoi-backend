using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.UploadImageDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : BaseAPIController
    {
		private readonly ICloudinaryService _cloudinaryService;

		public UploadController(ICloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}

		[HttpPost("upload-image")]
		public async Task<IActionResult> UploadImage([FromForm] UploadImageDTO dto)
		{
			try
			{
				if (dto.File == null || dto.File.Length == 0)
				{
                    return Error("File không hợp lệ.");
                }

				var uploadResult = await _cloudinaryService.UploadImageAsync(dto.File);

                var result = new
                {
                    uploadResult.PublicId,
                    Url = uploadResult.Url.ToString()
                };

                return SaveSuccess(result);
            }
			catch (Exception ex)
			{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi xử lý upload: {ex.Message}");
            }
		}

		[HttpDelete("delete-image/{publicId}")]
		public async Task<IActionResult> DeleteImage(string publicId)
		{
			try
			{
				var deleteResult = await _cloudinaryService.DeleteImageAsync(publicId);
                return SaveSuccess(new { Message = "Xóa ảnh thành công.", Result = deleteResult });
            }
			catch (Exception ex)
			{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi xử lý upload: {ex.Message}");
            }
		}
	}
}
