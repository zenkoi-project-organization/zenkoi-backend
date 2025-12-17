using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.UploadImageDTOs;
using Zenkoi.BLL.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zenkoi.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
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

		[HttpPost("upload-file")]
		public async Task<IActionResult> UploadFile([FromForm] UploadImageDTO dto)
		{
			try
			{
				if (dto.File == null || dto.File.Length == 0)
				{
                    return Error("File không hợp lệ.");
                }

				var uploadResult = await _cloudinaryService.UploadFileAsync(dto.File);

				var fileType = dto.File.ContentType.ToLower() switch
				{
					var ct when ct.StartsWith("image/") => "image",
					var ct when ct.StartsWith("video/") => "video",
					_ => "file"
				};

                var result = new
                {
                    uploadResult.PublicId,
                    Url = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString(),
					Format = uploadResult.Format,
					FileType = fileType,
					Size = dto.File.Length,
					OriginalFilename = dto.File.FileName
                };

                return SaveSuccess(result, "Save data success");
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

		[HttpDelete("delete-file/{publicId}")]
		public async Task<IActionResult> DeleteFile(string publicId, [FromQuery] string resourceType = "image")
		{
			try
			{
				var deleteResult = await _cloudinaryService.DeleteFileAsync(publicId, resourceType);
                return SaveSuccess(new { Message = $"Xóa {resourceType} thành công.", Result = deleteResult });
            }
			catch (Exception ex)
			{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi xử lý xóa file: {ex.Message}");
            }
		}
	}
}
