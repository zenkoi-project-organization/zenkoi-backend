using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Order> _orderRepo;
        private readonly IRepoBase<OrderDetail> _orderDetailRepo;
        private readonly IRepoBase<Customer> _customerRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<PacketFish> _packetFishRepo;
        private readonly IRepoBase<Promotion> _promotionRepo;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderRepo = _unitOfWork.GetRepo<Order>();
            _orderDetailRepo = _unitOfWork.GetRepo<OrderDetail>();
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
            _promotionRepo = _unitOfWork.GetRepo<Promotion>();
        }

        public async Task<OrderResponseDTO> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Id == id)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.KoiFishId.HasValue)
                    {
                        detail.KoiFish = await _koiFishRepo.GetByIdAsync(detail.KoiFishId.Value);
                    }
                    if (detail.PacketFishId.HasValue)
                    {
                        detail.PacketFish = await _packetFishRepo.GetByIdAsync(detail.PacketFishId.Value);
                    }
                }
            }

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.OrderNumber == orderNumber)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.KoiFishId.HasValue)
                    {
                        detail.KoiFish = await _koiFishRepo.GetByIdAsync(detail.KoiFishId.Value);
                    }
                    if (detail.PacketFishId.HasValue)
                    {
                        detail.PacketFish = await _packetFishRepo.GetByIdAsync(detail.PacketFishId.Value);
                    }
                }
            }

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<PaginatedList<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId, OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<Order>()
                .WithTracking(false)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .WithInclude("OrderDetails.KoiFish")
                .WithInclude("OrderDetails.PacketFish")
                .WithOrderBy(o => o.OrderByDescending(x => x.CreatedAt));

            queryBuilder.WithPredicate(o => o.CustomerId == customerId);

            ApplyOrderFilters(queryBuilder, filter);

            var query = _orderRepo.Get(queryBuilder.Build());
            var paginatedOrders = await PaginatedList<Order>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = _mapper.Map<List<OrderResponseDTO>>(paginatedOrders);

            return new PaginatedList<OrderResponseDTO>(
                resultDto,
                paginatedOrders.TotalItems,
                paginatedOrders.PageIndex,
                pageSize);
        }

        public async Task<PaginatedList<OrderResponseDTO>> GetAllOrdersAsync(OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<Order>()
                .WithTracking(false)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .WithInclude("OrderDetails.KoiFish")
                .WithInclude("OrderDetails.PacketFish")
                .WithOrderBy(o => o.OrderByDescending(x => x.CreatedAt));

            ApplyOrderFilters(queryBuilder, filter);

            var query = _orderRepo.Get(queryBuilder.Build());
            var paginatedOrders = await PaginatedList<Order>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = _mapper.Map<List<OrderResponseDTO>>(paginatedOrders);

            return new PaginatedList<OrderResponseDTO>(
                resultDto,
                paginatedOrders.TotalItems,
                paginatedOrders.PageIndex,
                pageSize);
        }

        public async Task<OrderResponseDTO> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (!IsValidStatusTransition(order.Status, updateOrderStatusDTO.Status))
            {
                throw new InvalidOperationException(
                    $"Cannot transition from {order.Status} to {updateOrderStatusDTO.Status}");
            }

            order.Status = updateOrderStatusDTO.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return await GetOrderByIdAsync(id);
        }


        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
                {
                    // 1. Từ Pending: Hủy hoặc Thanh toán xong
                    { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },

                    // 2. Từ Processing: Chuẩn bị xong (Shipped) hoặc Lỗi không giao được (UnShiping)
                    { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.UnShiping } },

                    // 3. Từ UnShiping: Chỉ có đường đi Hoàn tiền
                    { OrderStatus.UnShiping, new List<OrderStatus> { OrderStatus.Refund } },

                    // 4. Từ Shipped: Giao thành công hoặc Khách từ chối
                    { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered, OrderStatus.Rejected } },

                    // 5. Từ Rejected: Khách từ chối -> Hoàn tiền
                    { OrderStatus.Rejected, new List<OrderStatus> { OrderStatus.Refund } },

                    // 6. Các trạng thái kết thúc (End State)
                    { OrderStatus.Delivered, new List<OrderStatus>() },
                    { OrderStatus.Cancelled, new List<OrderStatus>() },
                    { OrderStatus.Refund, new List<OrderStatus>() }
                };

            // Kiểm tra logic
            if (!validTransitions.ContainsKey(currentStatus))
            {
                return false;
            }

            return validTransitions[currentStatus].Contains(newStatus);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                return false;
            }

            await _orderRepo.DeleteAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ApplyOrderFilters(QueryBuilder<Order> queryBuilder, OrderFilterRequestDTO filter)
        {
            if (filter == null)
                return;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                queryBuilder.WithPredicate(o =>
                    o.OrderNumber.ToLower().Contains(searchLower) ||
                    (o.Customer != null &&
                     o.Customer.ApplicationUser != null &&
                     o.Customer.ApplicationUser.FullName.ToLower().Contains(searchLower)));
            }

            if (filter.Status.HasValue)
            {
                queryBuilder.WithPredicate(o => o.Status == filter.Status.Value);
            }

            if (filter.CustomerId.HasValue)
            {
                queryBuilder.WithPredicate(o => o.CustomerId == filter.CustomerId.Value);
            }

            if (filter.CreatedFrom.HasValue)
            {
                queryBuilder.WithPredicate(o => o.CreatedAt >= filter.CreatedFrom.Value);
            }

            if (filter.CreatedTo.HasValue)
            {
                queryBuilder.WithPredicate(o => o.CreatedAt <= filter.CreatedTo.Value);
            }

            if (filter.MinTotalAmount.HasValue)
            {
                queryBuilder.WithPredicate(o => o.TotalAmount >= filter.MinTotalAmount.Value);
            }

            if (filter.MaxTotalAmount.HasValue)
            {
                queryBuilder.WithPredicate(o => o.TotalAmount <= filter.MaxTotalAmount.Value);
            }

            if (filter.HasPromotion.HasValue)
            {
                queryBuilder.WithPredicate(o => filter.HasPromotion.Value ? o.PromotionId != null : o.PromotionId == null);
            }

            if (!string.IsNullOrEmpty(filter.OrderNumber))
            {
                queryBuilder.WithPredicate(o => o.OrderNumber.Contains(filter.OrderNumber));
            }
        }

        public async Task UpdateInventoryAfterPaymentSuccessAsync(int orderId)
        {
            var order = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Id == orderId)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found");
            }
            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(orderDetail.KoiFishId.Value);
                    if (koiFish != null)
                    {
                        koiFish.SaleStatus = SaleStatus.Sold;
                        await _koiFishRepo.UpdateAsync(koiFish);
                    }
                }

                if (orderDetail.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetSingleAsync(new QueryBuilder<PacketFish>()
                        .WithPredicate(pf => pf.Id == orderDetail.PacketFishId.Value)
                        .WithInclude(pf => pf.PondPacketFishes)
                        .Build());

                    if (packetFish != null)
                    {
                        var pondPacketFishRepo = _unitOfWork.GetRepo<PondPacketFish>();
                        int remainingQty = orderDetail.Quantity;

                        foreach (var pondPacket in packetFish.PondPacketFishes
                            .Where(ppf => ppf.IsActive && ppf.AvailableQuantity > 0)
                            .OrderBy(ppf => ppf.Id))
                        {
                            if (remainingQty <= 0) break;

                            int deduct = Math.Min(pondPacket.AvailableQuantity, remainingQty);
                            pondPacket.AvailableQuantity -= deduct;
                            pondPacket.SoldQuantity += deduct;
                            remainingQty -= deduct;

                            await pondPacketFishRepo.UpdateAsync(pondPacket);
                        }
                        if (remainingQty > 0)
                        {
                            throw new InvalidOperationException(
                                $"Insufficient stock for PacketFish ID {orderDetail.PacketFishId}. " +
                                $"Still need {remainingQty} more items.");
                        }
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateDiscountAsync(int? promotionId, decimal subtotal)
        {
            if (!promotionId.HasValue)
                return 0;

            var promotion = await _promotionRepo.GetByIdAsync(promotionId.Value);

            if (promotion == null)
                throw new ArgumentException("Không tìm thấy khuyến mãi");

            if (!promotion.IsActive)
                throw new ArgumentException($"Khuyến mãi '{promotion.Code}' không còn hoạt động");

            var now = DateTime.UtcNow;
            if (now < promotion.ValidFrom || now > promotion.ValidTo)
                throw new ArgumentException($"Khuyến mãi '{promotion.Code}' không trong thời gian áp dụng");

            if (subtotal < promotion.MinimumOrderAmount)
                throw new ArgumentException($"Đơn hàng phải đạt tối thiểu {promotion.MinimumOrderAmount:C}");

            if (promotion.UsageLimit.HasValue && promotion.UsageCount >= promotion.UsageLimit.Value)
                throw new ArgumentException($"Khuyến mãi '{promotion.Code}' đã hết lượt sử dụng");

            decimal discountAmount = 0;

            if (promotion.DiscountType == DiscountType.Percentage)
            {
                discountAmount = subtotal * (promotion.DiscountValue / 100m);

                if (promotion.MaxDiscountAmount.HasValue)
                {
                    discountAmount = Math.Min(discountAmount, promotion.MaxDiscountAmount.Value);
                }
            }
            else if (promotion.DiscountType == DiscountType.FixedAmount)
            {
                discountAmount = promotion.DiscountValue;
            }

            return Math.Min(discountAmount, subtotal);
        }

    }
}
