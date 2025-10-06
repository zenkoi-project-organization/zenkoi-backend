using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface ICloudinaryService
	{
		Task<ImageUploadResult> UploadImageAsync(IFormFile file);
		Task<DeletionResult> DeleteImageAsync(string publicId);
	}
}
