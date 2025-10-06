using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.Implements
{
	public class CloudinaryService : ICloudinaryService
	{
		private readonly Cloudinary _cloudinary;

		public CloudinaryService(IConfiguration configuration)
		{
			var account = new Account(
				configuration["Cloudinary:CloudName"],
				configuration["Cloudinary:ApiKey"],
				configuration["Cloudinary:ApiSecret"]
			);

			_cloudinary = new Cloudinary(account);
		}

		public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
		{
			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, file.OpenReadStream())
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);
			return uploadResult;
		}

		public async Task<DeletionResult> DeleteImageAsync(string publicId)
		{
			var deleteParams = new DeletionParams(publicId);
			var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
			return deleteResult;
		}
	}
}
