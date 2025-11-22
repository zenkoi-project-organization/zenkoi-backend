using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.PaymentTransactionDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentTransactionController : BaseAPIController
    {
        private readonly IPaymentTransactionService _transactionService;

        public PaymentTransactionController(IPaymentTransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllTransactions(
            [FromQuery] PaymentTransactionFilterDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _transactionService.GetAllTransactionsAsync(filter, pageIndex, pageSize);
                var response = new PagingDTO<PaymentTransactionResponseDTO>(result);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy danh sách transactions: {ex.Message}");
            }
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyTransactions(
            [FromQuery] PaymentTransactionFilterDTO filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _transactionService.GetMyTransactionsAsync(UserId, filter, pageIndex, pageSize);
                var response = new PagingDTO<PaymentTransactionResponseDTO>(result);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy danh sách transactions: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var result = await _transactionService.GetTransactionByIdAsync(id);

                if (!User.IsInRole("Admin") && !User.IsInRole("Staff") && result.UserId != UserId)
                {
                    return GetUnAuthorized("Bạn không có quyền xem transaction này.");
                }

                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy thông tin transaction: {ex.Message}");
            }
        }

        [HttpGet("by-order/{actualOrderId}")]
        public async Task<IActionResult> GetTransactionByActualOrderId(int actualOrderId)
        {
            try
            {
                var result = await _transactionService.GetTransactionByActualOrderIdAsync(actualOrderId);

                if (!User.IsInRole("Admin") && !User.IsInRole("SaleStaff") && result.UserId != UserId)
                {
                    return GetUnAuthorized("Bạn không có quyền xem transaction này.");
                }

                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"Lỗi khi lấy thông tin transaction: {ex.Message}");
            }
        }
    }
}
