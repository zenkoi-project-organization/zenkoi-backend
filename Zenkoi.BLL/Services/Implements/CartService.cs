using AutoMapper;
using Zenkoi.BLL.DTOs.CartDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Cart> _cartRepo;
        private readonly IRepoBase<CartItem> _cartItemRepo;
        private readonly IRepoBase<Customer> _customerRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<PacketFish> _packetFishRepo;
        private readonly IRepoBase<Promotion> _promotionRepo;

        public CartService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartRepo = _unitOfWork.GetRepo<Cart>();
            _cartItemRepo = _unitOfWork.GetRepo<CartItem>();
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
            _promotionRepo = _unitOfWork.GetRepo<Promotion>();
        }

        public async Task<CartResponseDTO> GetCartByCustomerIdAsync(int customerId)
        {
            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.Customer)
                .WithInclude(c => c.Customer.ApplicationUser)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            if (cart.CartItems != null && cart.CartItems.Any())
            {
                foreach (var item in cart.CartItems)
                {
                    if (item.KoiFishId.HasValue)
                    {
                        item.KoiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    }
                    if (item.PacketFishId.HasValue)
                    {
                        item.PacketFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    }
                }
            }

            var cartResponse = _mapper.Map<CartResponseDTO>(cart);

            cartResponse.TotalPrice = cart.CartItems.Sum(ci =>
            {
                if (ci.KoiFishId.HasValue && ci.KoiFish != null)
                {
                    return (ci.KoiFish.SellingPrice ?? 0) * ci.Quantity;
                }
                else if (ci.PacketFishId.HasValue && ci.PacketFish != null)
                {
                    return ci.PacketFish.PricePerPacket * ci.Quantity;
                }
                return 0;
            });
      
            foreach (var item in cartResponse.CartItems)
            {
                if (item.KoiFish != null)
                {
                    item.ItemTotalPrice = (item.KoiFish.SellingPrice ?? 0) * item.Quantity;
                }
                else if (item.PacketFish != null)
                {
                    item.ItemTotalPrice = item.PacketFish.PricePerPacket * item.Quantity;
                }
            }

            return cartResponse;
        }

        public async Task<CartResponseDTO> GetOrCreateCartForCustomerAsync(int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.Customer)
                .WithInclude(c => c.Customer.ApplicationUser)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepo.CreateAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }
        
            if (cart.CartItems != null && cart.CartItems.Any())
            {
                foreach (var item in cart.CartItems)
                {
                    if (item.KoiFishId.HasValue)
                    {
                        item.KoiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    }
                    if (item.PacketFishId.HasValue)
                    {
                        item.PacketFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    }
                }
            }

            var cartResponse = _mapper.Map<CartResponseDTO>(cart);

            cartResponse.TotalPrice = cart.CartItems.Sum(ci =>
            {
                if (ci.KoiFishId.HasValue && ci.KoiFish != null)
                {
                    return (ci.KoiFish.SellingPrice ?? 0) * ci.Quantity;
                }
                else if (ci.PacketFishId.HasValue && ci.PacketFish != null)
                {
                    return ci.PacketFish.PricePerPacket * ci.Quantity;
                }
                return 0;
            });

            foreach (var item in cartResponse.CartItems)
            {
                if (item.KoiFish != null)
                {
                    item.ItemTotalPrice = (item.KoiFish.SellingPrice ?? 0) * item.Quantity;
                }
                else if (item.PacketFish != null)
                {
                    item.ItemTotalPrice = item.PacketFish.PricePerPacket * item.Quantity;
                }
            }

            return cartResponse;
        }

        public async Task<CartItemResponseDTO> AddCartItemAsync(AddCartItemDTO addCartItemDTO, int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            if (addCartItemDTO.KoiFishId.HasValue && addCartItemDTO.PacketFishId.HasValue)
            {
                throw new ArgumentException("An item cannot be both KoiFish and PacketFish");
            }

            if (!addCartItemDTO.KoiFishId.HasValue && !addCartItemDTO.PacketFishId.HasValue)
            {
                throw new ArgumentException("An item must be either KoiFish or PacketFish");
            }
            if (addCartItemDTO.KoiFishId.HasValue)
            {
                if (addCartItemDTO.Quantity != 1)
                {
                    throw new ArgumentException("KoiFish can only be purchased with quantity of 1");
                }
            }

            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepo.CreateAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            CartItem? existingItem = null;
            if (addCartItemDTO.KoiFishId.HasValue)
            {
                existingItem = cart.CartItems.FirstOrDefault(ci => ci.KoiFishId == addCartItemDTO.KoiFishId.Value);

                if (existingItem != null)
                {
                    throw new ArgumentException($"KoiFish with ID {addCartItemDTO.KoiFishId} is already in your cart. Each Koi fish can only be purchased once.");
                }
            }
            else if (addCartItemDTO.PacketFishId.HasValue)
            {
                existingItem = cart.CartItems.FirstOrDefault(ci => ci.PacketFishId == addCartItemDTO.PacketFishId.Value);
            }

            if (existingItem != null && addCartItemDTO.PacketFishId.HasValue)
            {
                existingItem.Quantity += addCartItemDTO.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                if (addCartItemDTO.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(addCartItemDTO.KoiFishId.Value);
                    if (koiFish == null)
                    {
                        throw new ArgumentException($"KoiFish with ID {addCartItemDTO.KoiFishId} not found");
                    }

                    if (koiFish.SaleStatus != SaleStatus.Available)
                    {
                        throw new ArgumentException($"KoiFish with ID {addCartItemDTO.KoiFishId} is not available for sale");
                    }
                }
                else if (addCartItemDTO.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(addCartItemDTO.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} not found");
                    }

                    if (!packetFish.IsAvailable)
                    {
                        throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} is not available");
                    }
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    KoiFishId = addCartItemDTO.KoiFishId,
                    PacketFishId = addCartItemDTO.PacketFishId,
                    Quantity = addCartItemDTO.Quantity,
                    AddedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartItemRepo.CreateAsync(cartItem);
                existingItem = cartItem;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            var cartItemWithData = await _cartItemRepo.GetSingleAsync(new QueryBuilder<CartItem>()
                .WithPredicate(ci => ci.Id == existingItem.Id)
                .WithInclude(ci => ci.KoiFish)
                .WithInclude(ci => ci.PacketFish)
                .Build());

            var response = _mapper.Map<CartItemResponseDTO>(cartItemWithData);

            if (response.KoiFish != null)
            {
                response.ItemTotalPrice = (response.KoiFish.SellingPrice ?? 0) * response.Quantity;
            }
            else if (response.PacketFish != null)
            {
                response.ItemTotalPrice = response.PacketFish.PricePerPacket * response.Quantity;
            }

            return response;
        }


        public async Task<CartItemResponseDTO> UpdateCartItemAsync(int cartItemId, UpdateCartItemDTO updateCartItemDTO, int customerId)
        {
            var cartItem = await _cartItemRepo.GetSingleAsync(new QueryBuilder<CartItem>()
                .WithPredicate(ci => ci.Id == cartItemId)
                .WithInclude(ci => ci.Cart)
                .WithInclude(ci => ci.KoiFish)
                .WithInclude(ci => ci.PacketFish)
                .Build());

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }
            if (cartItem.Cart.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this cart item");
            }


            if (cartItem.KoiFishId.HasValue)
            {
                if (updateCartItemDTO.Quantity != 1)
                {
                    throw new ArgumentException("KoiFish quantity cannot be changed. Each Koi fish can only be purchased once with quantity of 1.");
                }
            }

            cartItem.Quantity = updateCartItemDTO.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cartItem.Cart);

            await _cartItemRepo.UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CartItemResponseDTO>(cartItem);

            if (response.KoiFish != null)
            {
                response.ItemTotalPrice = (response.KoiFish.SellingPrice ?? 0) * response.Quantity;
            }
            else if (response.PacketFish != null)
            {
                response.ItemTotalPrice = response.PacketFish.PricePerPacket * response.Quantity;
            }

            return response;
        }


        public async Task<bool> RemoveCartItemAsync(int cartItemId, int customerId)
        {
            var cartItem = await _cartItemRepo.GetSingleAsync(new QueryBuilder<CartItem>()
                .WithPredicate(ci => ci.Id == cartItemId)
                .WithInclude(ci => ci.Cart)
                .Build());

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }
            if (cartItem.Cart.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this cart item");
            }

            var cartId = cartItem.CartId;

            await _cartItemRepo.DeleteAsync(cartItem);

            var cart = await _cartRepo.GetByIdAsync(cartId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepo.UpdateAsync(cart);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            foreach (var item in cart.CartItems.ToList())
            {
                await _cartItemRepo.DeleteAsync(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<OrderResponseDTO> ConvertCartToOrderAsync(ConvertCartToOrderDTO convertCartToOrderDTO, int customerId)
        {
            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.Customer)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            if (!cart.CartItems.Any())
            {
                throw new ArgumentException("Cart is empty. Cannot create order from empty cart.");
            }

            foreach (var item in cart.CartItems)
            {
                if (item.KoiFishId.HasValue)
                {
                    item.KoiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                }
                if (item.PacketFishId.HasValue)
                {
                    item.PacketFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                }
            }

            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in cart.CartItems)
            {
                decimal unitPrice = 0;

                if (item.KoiFishId.HasValue && item.KoiFish != null)
                {
                    unitPrice = item.KoiFish.SellingPrice ?? 0;
                    if (unitPrice == 0)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} does not have a selling price");
                    }
                }
                else if (item.PacketFishId.HasValue && item.PacketFish != null)
                {
                    unitPrice = item.PacketFish.PricePerPacket;
                }
                else
                {
                    throw new ArgumentException("Invalid cart item");
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

            if (convertCartToOrderDTO.PromotionId.HasValue)
            {
                var promotion = await _promotionRepo.GetByIdAsync(convertCartToOrderDTO.PromotionId.Value);
                if (promotion == null)
                {
                    throw new ArgumentException("Promotion not found");
                }
            }

            decimal shippingFee = convertCartToOrderDTO.ShippingFee;
            decimal discountAmount = await CalculateDiscountAsync(convertCartToOrderDTO.PromotionId, subtotal);
            var totalAmount = subtotal + shippingFee - discountAmount;

            var order = new Order
            {
                CustomerId = customerId,
                CustomerAddressId = convertCartToOrderDTO.CustomerAddressId,
                Status = OrderStatus.PendingPayment,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                PromotionId = convertCartToOrderDTO.PromotionId,
                OrderDetails = orderDetails
            };

            await _unitOfWork.GetRepo<Order>().CreateAsync(order);

            if (convertCartToOrderDTO.PromotionId.HasValue)
            {
                var promotion = await _promotionRepo.GetByIdAsync(convertCartToOrderDTO.PromotionId.Value);
                if (promotion != null)
                {
                    promotion.UsageCount++;
                    await _promotionRepo.UpdateAsync(promotion);
                }
            }

            foreach (var item in cart.CartItems.ToList())
            {
                await _cartItemRepo.DeleteAsync(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);

            await _unitOfWork.SaveChangesAsync();

            var createdOrder = await _unitOfWork.GetRepo<Order>().GetSingleAsync(new QueryBuilder<Order>()
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
