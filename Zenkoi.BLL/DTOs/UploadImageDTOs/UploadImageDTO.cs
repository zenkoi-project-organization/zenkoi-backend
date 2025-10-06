using Microsoft.AspNetCore.Http;

namespace Zenkoi.BLL.DTOs.UploadImageDTOs
{
	public class UploadImageDTO
	{
		public IFormFile File { get; set; }
	}
}
