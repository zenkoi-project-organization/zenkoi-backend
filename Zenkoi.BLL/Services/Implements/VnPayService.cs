using Microsoft.AspNetCore.Http;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.BLL.Helpers.Config;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;
using static Zenkoi.DAL.Enums.PaymentTransactionStatus;

namespace Zenkoi.BLL.Services.Implements
{
	public class VnPayService : IVnPayService
	{
		private readonly VnPayConfiguration _vnPayConfig;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderService _orderService;

		public VnPayService(VnPayConfiguration vnPayConfig, IUnitOfWork unitOfWork, IOrderService orderService)
		{
			_vnPayConfig = vnPayConfig;
			_unitOfWork = unitOfWork;
			_orderService = orderService;
		}

		public async Task<string> CreatePaymentUrlAsync(int userId, HttpContext context, VnPayRequestDTO vnPayRequest)
		{
			try
			{
				if (!vnPayRequest.OrderId.HasValue)
				{
					throw new ArgumentException("OrderId (PaymentTransaction.Id) is required for VNPay payment");
				}

				var vnpay = new VnPayLibrary();

				vnpay.AddRequestData("vnp_Version", _vnPayConfig.Version);
				vnpay.AddRequestData("vnp_Command", _vnPayConfig.Command);
				vnpay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);

				var amountInVND = (vnPayRequest.Amount * 100).ToString("F0");
				vnpay.AddRequestData("vnp_Amount", amountInVND);

				var createDate = vnPayRequest.CreatedDate.ToString("yyyyMMddHHmmss");
				vnpay.AddRequestData("vnp_CreateDate", createDate);
				vnpay.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrCode);
				vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
				vnpay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);

				vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho ZenKoi mã: " + vnPayRequest.OrderId);
				vnpay.AddRequestData("vnp_OrderType", "other");
				vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
				var txnRef = vnPayRequest.OrderId.Value.ToString();
				vnpay.AddRequestData("vnp_TxnRef", txnRef);			
				var paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);

				return paymentUrl;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public VnPayResponseDTO PaymentExcute(IQueryCollection collection)
		{
			var vnpay = new VnPayLibrary();

			foreach (var (key, value) in collection)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value.ToString());
				}
			}

			var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
			var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
			var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
			var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
			var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

			bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);

			if (!checkSignature)
			{
				return new VnPayResponseDTO
				{
					IsSuccess = false,
					VnPayResponseCode = vnp_ResponseCode,
					Amount = 0
				};
			}

			if (!long.TryParse(vnp_Amount, out var rawAmount))
			{
				rawAmount = 0;
			}
			var actualAmount = rawAmount / 100.0;
			bool isPaymentSuccess = vnp_ResponseCode == "00";

			var result = new VnPayResponseDTO
			{
				IsSuccess = isPaymentSuccess,
				PaymentMethod = "VnPay",
				OrderDescription = vnp_OrderInfo,
				OrderId = vnp_orderId.ToString(),
				TransactionId = vnp_TransactionId,
				Token = vnp_SecureHash,
				VnPayResponseCode = vnp_ResponseCode,
				Amount = actualAmount,
			};

			return result;
		}

		public async Task<VnPayCallbackResultDTO> ProcessVnPayReturnAsync(IQueryCollection queryParams)
		{
			try
			{
				int? orderIdFromQuery = null;
				if (queryParams.ContainsKey("vnp_TxnRef"))
				{
					var txnRefStr = queryParams["vnp_TxnRef"].ToString();
					if (long.TryParse(txnRefStr, out var txnRef))
					{
						var paymentTransactionFromQuery = await _unitOfWork.PaymentTransactions.GetSingleAsync(
							new QueryBuilder<PaymentTransaction>()
							.WithPredicate(pt => pt.Id == txnRef && pt.PaymentMethod == "VnPay")
							.WithOrderBy(q => q.OrderByDescending(pt => pt.CreatedAt))
							.Build());

						if (paymentTransactionFromQuery != null && paymentTransactionFromQuery.ActualOrderId.HasValue)
						{
							orderIdFromQuery = paymentTransactionFromQuery.ActualOrderId.Value;
						}
					}
				}

				var vnpayRes = PaymentExcute(queryParams);

				if (!vnpayRes.IsSuccess)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = "97",
						ErrorMessage = "Invalid signature",
						OrderId = orderIdFromQuery
					};
				}
				var paymentTransaction = await _unitOfWork.PaymentTransactions.GetSingleAsync(
					new QueryBuilder<PaymentTransaction>()
					.WithPredicate(pt => pt.OrderId == vnpayRes.OrderId && pt.PaymentMethod == "VnPay")
					.WithOrderBy(q => q.OrderByDescending(pt => pt.CreatedAt))
					.WithTracking(true)
					.Build());

				if (paymentTransaction == null)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = "01",
						ErrorMessage = "Transaction not found"
					};
				}

				var actualOrderId = paymentTransaction.ActualOrderId;
				if (actualOrderId == null)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = "01",
						ErrorMessage = "Order ID not found"
					};
				}
				if (paymentTransaction.Status == Success)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = true,
						OrderId = actualOrderId.Value,
						Amount = (decimal)vnpayRes.Amount
					};
				}
				if (paymentTransaction.Status == Failed)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = vnpayRes.VnPayResponseCode,
						ErrorMessage = "Payment failed",
						OrderId = actualOrderId.Value
					};
				}

				var order = await _orderService.GetOrderByIdAsync(actualOrderId.Value);
				if (order == null)
				{
					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = "01",
						ErrorMessage = "Order not found"
					};
				}

				if (Math.Abs(order.TotalAmount - (decimal)vnpayRes.Amount) > 0.01m)
				{
					paymentTransaction.Status = Failed;
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;
					await _unitOfWork.SaveChangesAsync();

					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = "04",
						ErrorMessage = "Invalid amount",
						OrderId = actualOrderId.Value
					};
				}
				if (vnpayRes.VnPayResponseCode == "00")
				{
					await _unitOfWork.BeginTransactionAsync();
					try
					{
						paymentTransaction.Status = Success;
						paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
						paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
						paymentTransaction.UpdatedAt = DateTime.UtcNow;

						await _orderService.UpdateOrderStatusAsync(
							actualOrderId.Value,
							new UpdateOrderStatusDTO { Status = OrderStatus.Processing }
						);

						await _orderService.UpdateInventoryAfterPaymentSuccessAsync(actualOrderId.Value);

						var payment = new Payment
						{
							OrderId = actualOrderId.Value,
							Method = PaymentMethod.VNPAY,
							Amount = order.TotalAmount,
							PaidAt = DateTime.UtcNow,
							TransactionId = vnpayRes.TransactionId.ToString(),
							Gateway = "VnPay",
							UserId = paymentTransaction.UserId
						};
						await _unitOfWork.GetRepo<Payment>().CreateAsync(payment);

						await _unitOfWork.SaveChangesAsync();
						await _unitOfWork.CommitTransactionAsync();

						return new VnPayCallbackResultDTO
						{
							IsSuccess = true,
							OrderId = actualOrderId.Value,
							Amount = (decimal)vnpayRes.Amount
						};
					}
					catch (Exception)
					{
						await _unitOfWork.RollBackAsync();
						throw;
					}
				}
				else
				{
					paymentTransaction.Status = Failed;
					paymentTransaction.TransactionId = vnpayRes.TransactionId.ToString();
					paymentTransaction.ResponseData = System.Text.Json.JsonSerializer.Serialize(vnpayRes);
					paymentTransaction.UpdatedAt = DateTime.UtcNow;

					await _orderService.UpdateOrderStatusAsync(
						actualOrderId.Value,
						new UpdateOrderStatusDTO { Status = OrderStatus.Pending }
					);

					await _unitOfWork.SaveChangesAsync();

					return new VnPayCallbackResultDTO
					{
						IsSuccess = false,
						ErrorCode = vnpayRes.VnPayResponseCode,
						ErrorMessage = "Payment failed",
						OrderId = actualOrderId.Value
					};
				}
			}
			catch (Exception ex)
			{
				int? orderIdFromQuery = null;
				if (queryParams.ContainsKey("vnp_TxnRef"))
				{
					var txnRefStr = queryParams["vnp_TxnRef"].ToString();
					if (long.TryParse(txnRefStr, out var txnRef))
					{
						var paymentTransactionFromQuery = await _unitOfWork.PaymentTransactions.GetSingleAsync(
							new QueryBuilder<PaymentTransaction>()
							.WithPredicate(pt => pt.Id == txnRef && pt.PaymentMethod == "VnPay")
							.WithOrderBy(q => q.OrderByDescending(pt => pt.CreatedAt))
							.Build());

						if (paymentTransactionFromQuery != null && paymentTransactionFromQuery.ActualOrderId.HasValue)
						{
							orderIdFromQuery = paymentTransactionFromQuery.ActualOrderId.Value;
						}
					}
				}

				return new VnPayCallbackResultDTO
				{
					IsSuccess = false,
					ErrorCode = "99",
					ErrorMessage = ex.Message,
					OrderId = orderIdFromQuery
				};
			}
		}
	}
}
