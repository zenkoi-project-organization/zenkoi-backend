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
        private readonly IShippingFeeCalculationService _shippingFeeCalculationService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IShippingFeeCalculationService shippingFeeCalculationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shippingFeeCalculationService = shippingFeeCalculationService;
            _orderRepo = _unitOfWork.GetRepo<Order>();
            _orderDetailRepo = _unitOfWork.GetRepo<OrderDetail>();
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
            _promotionRepo = _unitOfWork.GetRepo<Promotion>();
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO, int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in createOrderDTO.Items)
            {
                if (item.KoiFishId.HasValue && item.PacketFishId.HasValue)
                {
                    throw new ArgumentException("An item cannot be both KoiFish and PacketFish");
                }

                if (!item.KoiFishId.HasValue && !item.PacketFishId.HasValue)
                {
                    throw new ArgumentException("An item must be either KoiFish or PacketFish");
                }

                decimal unitPrice = 0;

                if (item.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    if (koiFish == null)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} not found");
                    }
                    unitPrice = koiFish.SellingPrice ?? 0;
                    if (unitPrice == 0)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} does not have a selling price");
                    }
                }
                else if (item.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {item.PacketFishId} not found");
                    }
                    unitPrice = packetFish.PricePerPacket;
                }

                var totalPrice = unitPrice * item.Quantity;
                subtotal += totalPrice;

                orderDetails.Add(new OrderDetail
                {
                    KoiFishId = item.KoiFishId,
                    PacketFishId = item.PacketFishId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            decimal shippingFee = createOrderDTO.ShippingFee;

            if (createOrderDTO.CustomerAddressId.HasValue && createOrderDTO.ShippingFee == 0)
            {
                var shippingFeeBreakdown = await _shippingFeeCalculationService.CalculateShippingFeeForOrderAsync(
                    createOrderDTO.Items,
                    createOrderDTO.CustomerAddressId.Value
                );
                shippingFee = shippingFeeBreakdown.TotalShippingFee;
            }

            decimal discountAmount = await CalculateDiscountAsync(createOrderDTO.PromotionId, subtotal);
            var totalAmount = subtotal + shippingFee - discountAmount;

            var order = new Order
            {
                CustomerId = customerId,
                CustomerAddressId = createOrderDTO.CustomerAddressId,
                Status = OrderStatus.PendingPayment,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                PromotionId = createOrderDTO.PromotionId,
                OrderDetails = orderDetails
            };

            await _orderRepo.CreateAsync(order);

            if (createOrderDTO.PromotionId.HasValue)
            {
                var promotion = await _promotionRepo.GetByIdAsync(createOrderDTO.PromotionId.Value);
                if (promotion != null)
                {
                    promotion.UsageCount++;
                    await _promotionRepo.UpdateAsync(promotion);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var createdOrder = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Id == order.Id)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (createdOrder != null && createdOrder.OrderDetails.Any())
            {
                foreach (var detail in createdOrder.OrderDetails)
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

            return _mapper.Map<OrderResponseDTO>(createdOrder);
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
            var queryOptions = new QueryOptions<Order>
            {
                IncludeProperties = new List<Expression<Func<Order, object>>>
                {
                    o => o.Customer!,
                    o => o.Customer!.ApplicationUser!,
                    o => o.Promotion!,
                    o => o.OrderDetails!
                }
            };

            Expression<Func<Order, bool>>? predicate = o => o.CustomerId == customerId;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<Order, bool>> expr = o =>
                    o.OrderNumber.Contains(filter.Search) ||
                    (o.Customer != null &&
                     o.Customer.ApplicationUser != null &&
                     o.Customer.ApplicationUser.FullName.Contains(filter.Search));
                predicate = predicate.AndAlso(expr);
            }

            if (filter.Status.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.Status == filter.Status.Value;
                predicate = predicate.AndAlso(expr);
            }

            if (filter.CreatedFrom.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.CreatedAt >= filter.CreatedFrom.Value;
                predicate = predicate.AndAlso(expr);
            }

            if (filter.CreatedTo.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.CreatedAt <= filter.CreatedTo.Value;
                predicate = predicate.AndAlso(expr);
            }

            if (filter.MinTotalAmount.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.TotalAmount >= filter.MinTotalAmount.Value;
                predicate = predicate.AndAlso(expr);
            }

            if (filter.MaxTotalAmount.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.TotalAmount <= filter.MaxTotalAmount.Value;
                predicate = predicate.AndAlso(expr);
            }

            if (filter.HasPromotion.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => filter.HasPromotion.Value ? o.PromotionId != null : o.PromotionId == null;
                predicate = predicate.AndAlso(expr);
            }

            if (!string.IsNullOrEmpty(filter.OrderNumber))
            {
                Expression<Func<Order, bool>> expr = o => o.OrderNumber.Contains(filter.OrderNumber);
                predicate = predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;
            queryOptions.OrderBy = o => o.OrderByDescending(x => x.CreatedAt);

            var orders = await _orderRepo.GetAllAsync(queryOptions);

            foreach (var order in orders)
            {
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
            }

            var mappedList = _mapper.Map<List<OrderResponseDTO>>(orders);
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<OrderResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PaginatedList<OrderResponseDTO>> GetAllOrdersAsync(OrderFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Order>
            {
                IncludeProperties = new List<Expression<Func<Order, object>>>
                {
                    o => o.Customer!,
                    o => o.Customer!.ApplicationUser!,
                    o => o.Promotion!,
                    o => o.OrderDetails!
                }
            };

            Expression<Func<Order, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<Order, bool>> expr = o => 
                    o.OrderNumber.Contains(filter.Search) || 
                    (o.Customer != null && 
                     o.Customer.ApplicationUser != null && 
                     o.Customer.ApplicationUser.FullName.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Status.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.Status == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.CustomerId.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.CustomerId == filter.CustomerId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.CreatedFrom.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.CreatedAt >= filter.CreatedFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.CreatedTo.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.CreatedAt <= filter.CreatedTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinTotalAmount.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.TotalAmount >= filter.MinTotalAmount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxTotalAmount.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => o.TotalAmount <= filter.MaxTotalAmount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.HasPromotion.HasValue)
            {
                Expression<Func<Order, bool>> expr = o => filter.HasPromotion.Value ? o.PromotionId != null : o.PromotionId == null;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (!string.IsNullOrEmpty(filter.OrderNumber))
            {
                Expression<Func<Order, bool>> expr = o => o.OrderNumber.Contains(filter.OrderNumber);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;
            queryOptions.OrderBy = o => o.OrderByDescending(x => x.CreatedAt);

            var orders = await _orderRepo.GetAllAsync(queryOptions);

            foreach (var order in orders)
            {
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
            }

            var mappedList = _mapper.Map<List<OrderResponseDTO>>(orders);
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<OrderResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<OrderResponseDTO> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            order.Status = updateOrderStatusDTO.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return await GetOrderByIdAsync(id);
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

        public async Task<decimal> CalculateOrderTotalAsync(CreateOrderDTO createOrderDTO)
        {
            decimal subtotal = 0;

            foreach (var item in createOrderDTO.Items)
            {
                decimal unitPrice = 0;

                if (item.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    if (koiFish == null)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} not found");
                    }
                    unitPrice = koiFish.SellingPrice ?? 0;
                }
                else if (item.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {item.PacketFishId} not found");
                    }
                    unitPrice = packetFish.PricePerPacket;
                }

                subtotal += unitPrice * item.Quantity;
            }

            decimal shippingFee = createOrderDTO.ShippingFee;

            if (createOrderDTO.CustomerAddressId.HasValue && createOrderDTO.ShippingFee == 0)
            {
                var shippingFeeBreakdown = await _shippingFeeCalculationService.CalculateShippingFeeForOrderAsync(
                    createOrderDTO.Items,
                    createOrderDTO.CustomerAddressId.Value
                );
                shippingFee = shippingFeeBreakdown.TotalShippingFee;
            }

            decimal discountAmount = await CalculateDiscountAsync(createOrderDTO.PromotionId, subtotal);

            return subtotal + shippingFee - discountAmount;
        }
    }
}
