using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
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

        public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            var customer = await _customerRepo.GetByIdAsync(createOrderDTO.CustomerId);
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
                string? productName = null;

                if (item.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    if (koiFish == null)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} not found");
                    }
                    unitPrice = koiFish.SellingPrice ?? 0;
                    productName = $"KoiFish {koiFish.RFID}";
                }
                else if (item.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {item.PacketFishId} not found");
                    }
                    unitPrice = packetFish.TotalPrice;
                    productName = packetFish.Name;
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

            decimal discountAmount = await CalculateDiscountAsync(createOrderDTO.PromotionId, subtotal);

            var totalAmount = subtotal + createOrderDTO.ShippingFee - discountAmount;

            var order = new Order
            {
                CustomerId = createOrderDTO.CustomerId,
                Status = OrderStatus.Created,
                Subtotal = subtotal,
                ShippingFee = createOrderDTO.ShippingFee,
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
                .WithInclude(o => o.Promotion)
                .Build());

            if (createdOrder != null)
            {
                var loadedOrderDetails = await _orderDetailRepo.GetAllAsync(new QueryBuilder<OrderDetail>()
                    .WithPredicate(od => od.OrderId == createdOrder.Id)
                    .Build());

                createdOrder.OrderDetails = loadedOrderDetails.ToList();

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

                if (createdOrder.Customer != null)
                {
                    var customerUser = await _customerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                        .WithPredicate(c => c.Id == createdOrder.Customer.Id)
                        .WithInclude(c => c.ApplicationUser)
                        .Build());
                    if (customerUser != null)
                    {
                        createdOrder.Customer.ApplicationUser = customerUser.ApplicationUser;
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
                .WithInclude(o => o.Promotion)
                .Build());

            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _orderRepo.GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.OrderNumber == orderNumber)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Promotion)
                .Build());

            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CustomerId == customerId)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Promotion)
                .WithOrderBy(o => o.OrderByDescending(x => x.CreatedAt))
                .Build());

            return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrdersAsync(QueryOptions<Order>? queryOptions = null)
        {
            if (queryOptions == null)
            {
                var orders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                    .WithInclude(o => o.Customer)
                    .WithInclude(o => o.Promotion)
                    .WithOrderBy(o => o.OrderByDescending(x => x.CreatedAt))
                    .Build());

                return _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
            }

            var ordersWithCustomOptions = await _orderRepo.GetAllAsync(queryOptions);
            return _mapper.Map<IEnumerable<OrderResponseDTO>>(ordersWithCustomOptions);
        }

        public async Task<OrderResponseDTO> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            order.Status = updateOrderStatusDTO.Status;
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
                    unitPrice = packetFish.TotalPrice;
                }

                subtotal += unitPrice * item.Quantity;
            }

            decimal discountAmount = await CalculateDiscountAsync(createOrderDTO.PromotionId, subtotal);

            return subtotal + createOrderDTO.ShippingFee - discountAmount;
        }
    }
}
