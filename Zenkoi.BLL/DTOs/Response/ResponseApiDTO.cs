using System.Net;
using System.Text.Json.Serialization;

namespace Zenkoi.BLL.DTOs.Response
{
	public class ResponseApiDTO
	{
		public int StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public object? Result { get; set; }
	}
}
