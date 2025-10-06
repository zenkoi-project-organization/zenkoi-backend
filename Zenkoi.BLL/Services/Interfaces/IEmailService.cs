using Zenkoi.BLL.DTOs.EmailDTOs;
using Zenkoi.BLL.DTOs.Response;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IEmailService
	{
		public BaseResponse SendEmail(EmailDTO emailDTO);
	}
}
