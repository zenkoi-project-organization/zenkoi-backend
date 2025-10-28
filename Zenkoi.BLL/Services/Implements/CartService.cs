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

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
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

            if (cart != null && cart.CartItems.Any())
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

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
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
                    return ci.PacketFish.TotalPrice * ci.Quantity;
                }
                return 0;
            });

            foreach (var item in cartResponse.CartItems)
            {
                item.ItemTotalPrice = (item.KoiFishPrice ?? item.PacketFishPrice ?? 0) * item.Quantity;
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

            if (cart != null && cart.CartItems.Any())
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

            var cartResponse = _mapper.Map<CartResponseDTO>(cart);
            cartResponse.TotalPrice = cart.CartItems.Sum(ci =>
            {
                if (ci.KoiFishId.HasValue && ci.KoiFish != null)
                {
                    return (ci.KoiFish.SellingPrice ?? 0) * ci.Quantity;
                }
                else if (ci.PacketFishId.HasValue && ci.PacketFish != null)
                {
                    return ci.PacketFish.TotalPrice * ci.Quantity;
                }
                return 0;
            });

            foreach (var item in cartResponse.CartItems)
            {
                item.ItemTotalPrice = (item.KoiFishPrice ?? item.PacketFishPrice ?? 0) * item.Quantity;
            }

            return cartResponse;
        }

        public async Task<CartItemResponseDTO> AddCartItemAsync(AddCartItemDTO addCartItemDTO)
        {

            var customer = await _customerRepo.GetByIdAsync(addCartItemDTO.CustomerId);
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

            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == addCartItemDTO.CustomerId)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = addCartItemDTO.CustomerId,
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
            }
            else if (addCartItemDTO.PacketFishId.HasValue)
            {
                existingItem = cart.CartItems.FirstOrDefault(ci => ci.PacketFishId == addCartItemDTO.PacketFishId.Value);
            }

            if (existingItem != null)
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
                }
                else if (addCartItemDTO.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(addCartItemDTO.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} not found");
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
            response.ItemTotalPrice = (response.KoiFishPrice ?? response.PacketFishPrice ?? 0) * response.Quantity;

            return response;
        }

        public async Task<CartItemResponseDTO> UpdateCartItemAsync(int cartItemId, UpdateCartItemDTO updateCartItemDTO)
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

            cartItem.Quantity = updateCartItemDTO.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cartItem.Cart);

            await _cartItemRepo.UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CartItemResponseDTO>(cartItem);
            response.ItemTotalPrice = (response.KoiFishPrice ?? response.PacketFishPrice ?? 0) * response.Quantity;

            return response;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _cartItemRepo.GetSingleAsync(new QueryBuilder<CartItem>()
                .WithPredicate(ci => ci.Id == cartItemId)
                .WithInclude(ci => ci.Cart)
                .Build());

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
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

        public async Task<OrderResponseDTO> ConvertCartToOrderAsync(ConvertCartToOrderDTO convertCartToOrderDTO)
        {
            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == convertCartToOrderDTO.CustomerId)
                .WithInclude(c => c.Customer)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart != null && cart.CartItems.Any())
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

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            if (!cart.CartItems.Any())
            {
                throw new ArgumentException("Cart is empty. Cannot create order from empty cart.");
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
                    unitPrice = item.PacketFish.TotalPrice;
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
                var promotion = await _unitOfWork.GetRepo<Promotion>().GetByIdAsync(convertCartToOrderDTO.PromotionId.Value);
                if (promotion == null)
                {
                    throw new ArgumentException("Promotion not found");
                }
            }

            var totalAmount = subtotal + convertCartToOrderDTO.ShippingFee - convertCartToOrderDTO.DiscountAmount;

            var order = new Order
            {
                CustomerId = convertCartToOrderDTO.CustomerId,
                Status = OrderStatus.Created,
                Subtotal = subtotal,
                ShippingFee = convertCartToOrderDTO.ShippingFee,
                DiscountAmount = convertCartToOrderDTO.DiscountAmount,
                TotalAmount = totalAmount,
                PromotionId = convertCartToOrderDTO.PromotionId,
                OrderDetails = orderDetails
            };

            await _unitOfWork.GetRepo<Order>().CreateAsync(order);

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
    }
}

