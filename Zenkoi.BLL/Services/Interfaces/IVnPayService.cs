using Microsoft.AspNetCore.Http;
using Zenkoi.BLL.DTOs.VnPayDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IVnPayService
	{
		Task<string> CreatePaymentUrlAsync(int userId, HttpContext context, VnPayRequestDTO vnPayRequest);
		VnPayResponseDTO PaymentExcute(IQueryCollection collection);
		Task<VnPayCallbackResultDTO> ProcessVnPayReturnAsync(IQueryCollection queryParams);
	}
}
