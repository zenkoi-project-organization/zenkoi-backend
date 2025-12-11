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
            var order = await LoadOrderWithDetailsAsync(o => o.Id == id);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }
            await LoadOrderDetailProductsAsync(order);
            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await LoadOrderWithDetailsAsync(o => o.OrderNumber == orderNumber);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }
            await LoadOrderDetailProductsAsync(order);
            return _mapper.Map<OrderResponseDTO>(order);
        }

        private async Task<Order?> LoadOrderWithDetailsAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(predicate)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .Build());
        }

        private async Task LoadOrderDetailProductsAsync(Order order)
        {
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {          
                var koiFishIds = order.OrderDetails
                    .Where(d => d.KoiFishId.HasValue)
                    .Select(d => d.KoiFishId.Value)
                    .Distinct()
                    .ToList();

                var packetFishIds = order.OrderDetails
                    .Where(d => d.PacketFishId.HasValue)
                    .Select(d => d.PacketFishId.Value)
                    .Distinct()
                    .ToList();

                Dictionary<int, KoiFish> koiFishDict = new Dictionary<int, KoiFish>();
                if (koiFishIds.Any())
                {
                    var koiFishes = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                        .WithPredicate(k => koiFishIds.Contains(k.Id))
                        .Build());
                    koiFishDict = koiFishes.ToDictionary(k => k.Id);
                }

                Dictionary<int, PacketFish> packetFishDict = new Dictionary<int, PacketFish>();
                if (packetFishIds.Any())
                {
                    var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                        .WithPredicate(pf => packetFishIds.Contains(pf.Id))
                        .Build());
                    packetFishDict = packetFishes.ToDictionary(pf => pf.Id);
                }

                foreach (var detail in order.OrderDetails)
                {
                    if (detail.KoiFishId.HasValue && koiFishDict.TryGetValue(detail.KoiFishId.Value, out var koiFish))
                    {
                        detail.KoiFish = koiFish;
                    }
                    if (detail.PacketFishId.HasValue && packetFishDict.TryGetValue(detail.PacketFishId.Value, out var packetFish))
                    {
                        detail.PacketFish = packetFish;
                    }
                }
            }
        }

        public async Task<PaginatedList<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId, OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<Order>()
                .WithTracking(false)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .WithThenInclude(q => q.Include(o => o.OrderDetails).ThenInclude(od => od.KoiFish))
                .WithThenInclude(q => q.Include(o => o.OrderDetails).ThenInclude(od => od.PacketFish))
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
                .WithThenInclude(q => q.Include(o => o.OrderDetails).ThenInclude(od => od.KoiFish))
                .WithThenInclude(q => q.Include(o => o.OrderDetails).ThenInclude(od => od.PacketFish))
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

            // If order is being cancelled, rollback inventory reservation
            if (updateOrderStatusDTO.Status == OrderStatus.Cancelled)
            {
                await RollbackInventoryReservationAsync(id);
            }

            order.Status = updateOrderStatusDTO.Status;

            if (!string.IsNullOrWhiteSpace(updateOrderStatusDTO.Note))
            {
                order.Note = updateOrderStatusDTO.Note;
            }

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
            var order = await LoadOrderWithOrderDetailsAsync(orderId);

            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.KoiFishId.HasValue)
                {
                    await ConfirmKoiFishSaleAsync(orderDetail.KoiFishId.Value);
                }

                if (orderDetail.PacketFishId.HasValue)
                {
                    await ConfirmPacketFishSaleAsync(orderDetail.PacketFishId.Value, orderDetail.Quantity);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Order> LoadOrderWithOrderDetailsAsync(int orderId)
        {
            var order = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Id == orderId)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (order == null)
                throw new ArgumentException($"Order with ID {orderId} not found");

            return order;
        }

        private async Task ConfirmKoiFishSaleAsync(int koiFishId)
        {
            var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId);
            if (koiFish != null)
            {
                koiFish.SaleStatus = SaleStatus.Sold;
                await _koiFishRepo.UpdateAsync(koiFish);
            }
        }

        private async Task ConfirmPacketFishSaleAsync(int packetFishId, int packetQuantity)
        {
            var packetFish = await LoadPacketFishWithPondsAsync(packetFishId);
            if (packetFish == null) return;

            var fishCount = packetQuantity * packetFish.FishPerPacket;
            await UpdatePondPacketSoldQuantityAsync(packetFish.PondPacketFishes, fishCount);
        }

        private async Task<PacketFish?> LoadPacketFishWithPondsAsync(int packetFishId)
        {
            return await _packetFishRepo.GetSingleAsync(new QueryBuilder<PacketFish>()
                .WithPredicate(pf => pf.Id == packetFishId)
                .WithInclude(pf => pf.PondPacketFishes)
                .Build());
        }

        private async Task UpdatePondPacketSoldQuantityAsync(ICollection<PondPacketFish> pondPackets, int fishCount)
        {
            var pondPacketFishRepo = _unitOfWork.GetRepo<PondPacketFish>();
            int remainingFish = fishCount;

            foreach (var pondPacket in pondPackets
                .Where(ppf => ppf.IsActive)
                .OrderBy(ppf => ppf.Id))
            {
                if (remainingFish <= 0) break;

                int toConfirm = Math.Min(remainingFish, pondPacket.AvailableQuantity);
                pondPacket.SoldQuantity += toConfirm;
                remainingFish -= toConfirm;

                await pondPacketFishRepo.UpdateAsync(pondPacket);
            }
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

        public async Task ValidateOrderItemsAvailabilityAsync(OrderResponseDTO order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                throw new InvalidOperationException("Order has no items");
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                await ValidateOrderItemForPaymentAsync(
                    orderDetail.KoiFishId,
                    orderDetail.PacketFishId,
                    orderDetail.Quantity,
                    order.Id);
            }
        }

        public async Task ValidateCartItemsAvailabilityAsync(IEnumerable<CartItem> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Cart has no items");
            }

            foreach (var cartItem in cartItems)
            {
                await ValidateItemAvailabilityAsync(
                    cartItem.KoiFishId,
                    cartItem.PacketFishId,
                    cartItem.Quantity);
            }
        }
        private async Task ValidateItemAvailabilityAsync(int? koiFishId, int? packetFishId, int quantity)
        {
            if (koiFishId.HasValue)
            {
                var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId.Value);
                if (koiFish == null)
                {
                    throw new InvalidOperationException($"KoiFish with ID {koiFishId} not found");
                }
                if (koiFish.SaleStatus != SaleStatus.Available)
                {
                    throw new InvalidOperationException(
                        $"KoiFish with RFID '{koiFish.RFID}' is no longer available for sale");
                }
            }

            if (packetFishId.HasValue)
            {
                var packetFish = await _packetFishRepo.GetSingleAsync(
                    new QueryBuilder<PacketFish>()
                    .WithPredicate(pf => pf.Id == packetFishId.Value)
                    .WithInclude(pf => pf.PondPacketFishes)
                    .Build());

                if (packetFish == null)
                {
                    throw new InvalidOperationException($"PacketFish with ID {packetFishId} not found");
                }
                if (!packetFish.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"PacketFish '{packetFish.Name}' is no longer available");
                }

                var totalAvailable = packetFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity);

                if (totalAvailable < quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for PacketFish '{packetFish.Name}'. " +
                        $"Requested: {quantity}, Available: {totalAvailable}");
                }
            }
        }

        private async Task ValidateOrderItemForPaymentAsync(int? koiFishId, int? packetFishId, int quantity, int orderId)
        {
            if (koiFishId.HasValue)
            {
                var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId.Value);
                if (koiFish == null)
                {
                    throw new InvalidOperationException($"KoiFish with ID {koiFishId} not found");
                }
                if (koiFish.SaleStatus == SaleStatus.Sold)
                {
                    throw new InvalidOperationException(
                        $"KoiFish with RFID '{koiFish.RFID}' has already been sold to another customer.");
                }

                if (koiFish.SaleStatus == SaleStatus.NotForSale)
                {
                    var orderDetail = await _orderDetailRepo.GetSingleAsync(
                        new QueryBuilder<OrderDetail>()
                        .WithPredicate(od => od.OrderId == orderId && od.KoiFishId == koiFishId.Value)
                        .Build());

                    if (orderDetail == null)
                    {
                        throw new InvalidOperationException(
                            $"KoiFish with RFID '{koiFish.RFID}' is not available for sale. Please contact support or cancel this order.");
                    }
                    // Fish is NotForSale but is in this order - this is expected (reserved during checkout)
                }
            }

            if (packetFishId.HasValue)
            {
                var packetFish = await _packetFishRepo.GetSingleAsync(
                    new QueryBuilder<PacketFish>()
                    .WithPredicate(pf => pf.Id == packetFishId.Value)
                    .WithInclude(pf => pf.PondPacketFishes)
                    .Build());

                if (packetFish == null)
                {
                    throw new InvalidOperationException($"PacketFish with ID {packetFishId} not found");
                }
                if (!packetFish.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"PacketFish '{packetFish.Name}' is no longer available");
                }
                var orderDetail = await _orderDetailRepo.GetSingleAsync(
                    new QueryBuilder<OrderDetail>()
                    .WithPredicate(od => od.OrderId == orderId && od.PacketFishId == packetFishId.Value)
                    .Build());

                if (orderDetail == null)
                {
                    throw new InvalidOperationException(
                        $"PacketFish '{packetFish.Name}' is not part of this order");
                }
                var totalAvailable = packetFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity);
                if (totalAvailable < 0)
                {
                    throw new InvalidOperationException(
                        $"PacketFish '{packetFish.Name}' has insufficient stock");
                }
            }
        }
        private async Task RollbackInventoryReservationAsync(int orderId)
        {
            var order = await LoadOrderWithOrderDetailsAsync(orderId);

            if (order.Status != OrderStatus.Pending)
            {
                return;
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.KoiFishId.HasValue)
                {
                    await RollbackKoiFishReservationAsync(orderDetail.KoiFishId.Value);
                }

                if (orderDetail.PacketFishId.HasValue)
                {
                    await RollbackPacketFishReservationAsync(orderDetail.PacketFishId.Value, orderDetail.Quantity);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RollbackKoiFishReservationAsync(int koiFishId)
        {
            var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId);
            if (koiFish != null && koiFish.SaleStatus == SaleStatus.NotForSale)
            {
                koiFish.SaleStatus = SaleStatus.Available;
                await _koiFishRepo.UpdateAsync(koiFish);
            }
        }

        private async Task RollbackPacketFishReservationAsync(int packetFishId, int packetQuantity)
        {
            var packetFish = await LoadPacketFishWithPondsAsync(packetFishId);
            if (packetFish == null) return;

            var fishCount = packetQuantity * packetFish.FishPerPacket;
            await RestorePondPacketAvailableQuantityAsync(packetFish.PondPacketFishes, fishCount);
        }

        private async Task RestorePondPacketAvailableQuantityAsync(ICollection<PondPacketFish> pondPackets, int fishCount)
        {
            var pondPacketFishRepo = _unitOfWork.GetRepo<PondPacketFish>();
            int remainingFish = fishCount;

            foreach (var pondPacket in pondPackets
                .Where(ppf => ppf.IsActive)
                .OrderBy(ppf => ppf.Id))
            {
                if (remainingFish <= 0) break;

                int toAddBack = Math.Min(remainingFish, pondPacket.SoldQuantity);
                pondPacket.AvailableQuantity += toAddBack;
                pondPacket.SoldQuantity -= toAddBack;
                remainingFish -= toAddBack;

                await pondPacketFishRepo.UpdateAsync(pondPacket);
            }
        }

        public async Task CancelOrderAndReleaseInventoryAsync(int orderId)
        {
            var order = await GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return;
            }

            if (order.Status != OrderStatus.Pending)
            {
                return;
            }

            // Release inventory first
            await RollbackInventoryReservationAsync(orderId);

            // Then cancel the order
            await UpdateOrderStatusAsync(orderId, new UpdateOrderStatusDTO
            {
                Status = OrderStatus.Cancelled
            });
        }

    }
}
