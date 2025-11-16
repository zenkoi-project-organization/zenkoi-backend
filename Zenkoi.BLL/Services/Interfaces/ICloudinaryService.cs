using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface ICloudinaryService
	{
		Task<ImageUploadResult> UploadImageAsync(IFormFile file);
		Task<UploadResult> UploadFileAsync(IFormFile file);
		Task<DeletionResult> DeleteImageAsync(string publicId);
		Task<DeletionResult> DeleteFileAsync(string publicId, string resourceType = "image");
	}
}
