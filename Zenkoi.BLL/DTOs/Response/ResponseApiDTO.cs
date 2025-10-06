using System.Net;

namespace Zenkoi.BLL.DTOs.Response
{
	public class ResponseApiDTO
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public List<KeyValuePair<string, string>>? Errors { get; set; }
		public object? Result { get; set; }
	}
}
