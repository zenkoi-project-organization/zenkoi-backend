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

		public async Task<UploadResult> UploadFileAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				throw new ArgumentException("File is empty or null");
			}

			var contentType = file.ContentType.ToLower();
			
			if (contentType.StartsWith("image/"))
			{
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, file.OpenReadStream()),
					Folder = "images"
				};
				return await _cloudinary.UploadAsync(uploadParams);
			}
			else if (contentType.StartsWith("video/"))
			{
				var uploadParams = new VideoUploadParams
				{
					File = new FileDescription(file.FileName, file.OpenReadStream()),
					Folder = "videos"
				};
				return await _cloudinary.UploadAsync(uploadParams);
			}
			else
			{
				var uploadParams = new RawUploadParams
				{
					File = new FileDescription(file.FileName, file.OpenReadStream()),
					Folder = "files"
				};
				return await _cloudinary.UploadAsync(uploadParams);
			}
		}

		public async Task<DeletionResult> DeleteImageAsync(string publicId)
		{
			var deleteParams = new DeletionParams(publicId);
			var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
			return deleteResult;
		}

		public async Task<DeletionResult> DeleteFileAsync(string publicId, string resourceType = "image")
		{
			var deleteParams = new DeletionParams(publicId)
			{
				ResourceType = resourceType switch
				{
					"video" => ResourceType.Video,
					"raw" => ResourceType.Raw,
					_ => ResourceType.Image
				}
			};
			var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
			return deleteResult;
		}
	}
}
