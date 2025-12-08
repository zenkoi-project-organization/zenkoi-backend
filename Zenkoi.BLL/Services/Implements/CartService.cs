using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPromotionService _promotionService;
        private readonly IOrderService _orderService;

        public CartService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPromotionService promotionService,
            IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _promotionService = promotionService;
            _orderService = orderService;
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
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.KoiFish))
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.PacketFish))
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.PacketFish).ThenInclude(pf => pf.PondPacketFishes))
                .Build());

            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            var cartResponse = _mapper.Map<CartResponseDTO>(cart);

            cartResponse.TotalPrice = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);

            foreach (var item in cartResponse.CartItems)
            {
                item.ItemTotalPrice = item.UnitPrice * item.Quantity;

                var cartItemEntity = cart.CartItems.First(ci => ci.Id == item.Id);

                CalculatePacketFishStockQuantity(item, cartItemEntity);
                CheckCartItemAvailability(item, cartItemEntity);
            }

            await CalculateAndApplyPromotionAsync(cartResponse);

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
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.KoiFish))
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.PacketFish))
                .WithThenInclude(q => q.Include(c => c.CartItems).ThenInclude(ci => ci.PacketFish).ThenInclude(pf => pf.PondPacketFishes))
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
                var cartItemEntity = cart.CartItems.First(ci => ci.Id == item.Id);

                if (item.KoiFish != null)
                {
                    item.ItemTotalPrice = (item.KoiFish.SellingPrice ?? 0) * item.Quantity;
                }
                else if (item.PacketFish != null)
                {
                    item.ItemTotalPrice = item.PacketFish.PricePerPacket * item.Quantity;
                }

                CalculatePacketFishStockQuantity(item, cartItemEntity);
                CheckCartItemAvailability(item, cartItemEntity);
            }

            await CalculateAndApplyPromotionAsync(cartResponse);

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
                    throw new ArgumentException($"Cá koi với RFID {addCartItemDTO.KoiFishId} is already in your cart. Each Koi fish can only be purchased once.");
                }
            }
            else if (addCartItemDTO.PacketFishId.HasValue)
            {
                existingItem = cart.CartItems.FirstOrDefault(ci => ci.PacketFishId == addCartItemDTO.PacketFishId.Value);
            }

            if (existingItem != null && addCartItemDTO.PacketFishId.HasValue)
            {
                // Load PacketFish with PondPacketFishes to validate stock
                var packetFish = await _packetFishRepo.GetSingleAsync(new QueryBuilder<PacketFish>()
                    .WithPredicate(pf => pf.Id == addCartItemDTO.PacketFishId.Value)
                    .WithInclude(pf => pf.PondPacketFishes)
                    .Build());

                if (packetFish == null)
                {
                    throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} not found");
                }

                // Calculate new total quantity after adding
                var newTotalQuantity = existingItem.Quantity + addCartItemDTO.Quantity;
                var totalAvailableFish = packetFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity);

                var totalAvailablePackets = totalAvailableFish / packetFish.FishPerPacket;
                var requestedFishCount = newTotalQuantity * packetFish.FishPerPacket;

                if (totalAvailableFish < requestedFishCount)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for PacketFish '{packetFish.Name}'. " +
                        $"Requested total: {newTotalQuantity} packets ({requestedFishCount} fish), Available: {totalAvailablePackets} packets ({totalAvailableFish} fish)");
                }

                existingItem.Quantity = newTotalQuantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _cartItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                KoiFish? koiFish = null;
                PacketFish? packetFish = null;

                if (addCartItemDTO.KoiFishId.HasValue)
                {
                    koiFish = await _koiFishRepo.GetByIdAsync(addCartItemDTO.KoiFishId.Value);
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
                    packetFish = await _packetFishRepo.GetSingleAsync(new QueryBuilder<PacketFish>()
                        .WithPredicate(pf => pf.Id == addCartItemDTO.PacketFishId.Value)
                        .WithInclude(pf => pf.PondPacketFishes)
                        .Build());

                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} not found");
                    }

                    if (!packetFish.IsAvailable)
                    {
                        throw new ArgumentException($"PacketFish with ID {addCartItemDTO.PacketFishId} is not available");
                    }
                    var totalAvailableFish = packetFish.PondPacketFishes
                        .Where(ppf => ppf.IsActive)
                        .Sum(ppf => ppf.AvailableQuantity);

                    var totalAvailablePackets = totalAvailableFish / packetFish.FishPerPacket;
                    var requestedFishCount = addCartItemDTO.Quantity * packetFish.FishPerPacket;

                    if (totalAvailableFish < requestedFishCount)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for PacketFish '{packetFish.Name}'. " +
                            $"Requested: {addCartItemDTO.Quantity} packets ({requestedFishCount} fish), Available: {totalAvailablePackets} packets ({totalAvailableFish} fish)");
                    }
                }

                decimal unitPrice = 0;
                if (addCartItemDTO.KoiFishId.HasValue && koiFish != null)
                {
                    unitPrice = koiFish.SellingPrice ?? 0;
                }
                else if (addCartItemDTO.PacketFishId.HasValue && packetFish != null)
                {
                    unitPrice = packetFish.PricePerPacket;
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    KoiFishId = addCartItemDTO.KoiFishId,
                    PacketFishId = addCartItemDTO.PacketFishId,
                    Quantity = addCartItemDTO.Quantity,
                    UnitPrice = unitPrice,
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
                .WithThenInclude(q => q.Include(ci => ci.PacketFish).ThenInclude(pf => pf.PondPacketFishes))
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

            CalculatePacketFishStockQuantity(response, cartItemWithData);

            return response;
        }


        public async Task<CartItemResponseDTO> UpdateCartItemAsync(int cartItemId, UpdateCartItemDTO updateCartItemDTO, int customerId)
        {
            var cartItem = await _cartItemRepo.GetSingleAsync(new QueryBuilder<CartItem>()
                .WithPredicate(ci => ci.Id == cartItemId)
                .WithInclude(ci => ci.Cart)
                .WithInclude(ci => ci.KoiFish)
                .WithInclude(ci => ci.PacketFish)
                .WithThenInclude(q => q.Include(ci => ci.PacketFish).ThenInclude(pf => pf.PondPacketFishes))
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
            else if (cartItem.PacketFishId.HasValue && cartItem.PacketFish != null)
            {
                // Validate stock availability for PacketFish
                var totalAvailableFish = cartItem.PacketFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity);

                var totalAvailablePackets = totalAvailableFish / cartItem.PacketFish.FishPerPacket;
                var requestedFishCount = updateCartItemDTO.Quantity * cartItem.PacketFish.FishPerPacket;

                if (totalAvailableFish < requestedFishCount)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for PacketFish '{cartItem.PacketFish.Name}'. " +
                        $"Requested: {updateCartItemDTO.Quantity} packets ({requestedFishCount} fish), Available: {totalAvailablePackets} packets ({totalAvailableFish} fish)");
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

            CalculatePacketFishStockQuantity(response, cartItem);

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
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var cart = await LoadCartWithValidationAsync(customerId);
                await ValidateAndLockCartItemsAsync(cart.CartItems);
                var (subtotal, orderDetails) = BuildOrderDetailsFromCart(cart.CartItems);

                var currentPromotion = await GetCurrentPromotionAsync();
                decimal discountAmount = await CalculateDiscountAsync(currentPromotion?.Id, subtotal);

                var order = CreateOrderEntity(convertCartToOrderDTO, customerId, subtotal, discountAmount, currentPromotion?.Id, orderDetails);
                await _unitOfWork.GetRepo<Order>().CreateAsync(order);

                await ReserveInventoryForOrderAsync(cart.CartItems);
                await IncrementPromotionUsageAsync(currentPromotion);
                await ClearCartAfterOrderCreationAsync(cart);

                await _unitOfWork.SaveChangesAsync();

                var createdOrder = await LoadCreatedOrderWithDetailsAsync(order.Id);
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<OrderResponseDTO>(createdOrder);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        private async Task<Cart> LoadCartWithValidationAsync(int customerId)
        {
            var cart = await _cartRepo.GetSingleAsync(new QueryBuilder<Cart>()
                .WithPredicate(c => c.CustomerId == customerId)
                .WithInclude(c => c.CartItems)
                .Build());

            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            return cart;
        }

        private async Task ValidateAndLockCartItemsAsync(ICollection<CartItem> cartItems)
        {
            var koiFishIds = cartItems
                .Where(x => x.KoiFishId.HasValue)
                .Select(x => x.KoiFishId.Value)
                .Distinct()
                .ToList();

            Dictionary<int, KoiFish> koiFishDict = new Dictionary<int, KoiFish>();
            if (koiFishIds.Any())
            {
                var koiFishes = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                    .WithPredicate(k => koiFishIds.Contains(k.Id))
                    .WithTracking(true)
                    .WithLockForUpdate(true) // Add row-level lock to prevent race conditions
                    .Build());

                koiFishDict = koiFishes.ToDictionary(k => k.Id);
            }

            var packetFishIds = cartItems
                .Where(x => x.PacketFishId.HasValue)
                .Select(x => x.PacketFishId.Value)
                .Distinct()
                .ToList();

            Dictionary<int, PacketFish> packetFishDict = new Dictionary<int, PacketFish>();
            if (packetFishIds.Any())
            {
                var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                    .WithPredicate(pf => packetFishIds.Contains(pf.Id))
                    .WithInclude(pf => pf.PondPacketFishes)
                    .WithTracking(true)
                    .WithLockForUpdate(true) // Add row-level lock to prevent concurrent reservation
                    .Build());

                packetFishDict = packetFishes.ToDictionary(pf => pf.Id);
            }

            foreach (var item in cartItems)
            {
                if (item.KoiFishId.HasValue)
                {
                    if (!koiFishDict.TryGetValue(item.KoiFishId.Value, out var koiFish))
                    {
                        throw new InvalidOperationException($"KoiFish with ID {item.KoiFishId} not found");
                    }

                    item.KoiFish = koiFish;

                    if (koiFish.SaleStatus != SaleStatus.Available)
                    {
                        throw new InvalidOperationException($"KoiFish '{koiFish.RFID}' is no longer available");
                    }
                }

                if (item.PacketFishId.HasValue)
                {
                    if (!packetFishDict.TryGetValue(item.PacketFishId.Value, out var packetFish))
                    {
                        throw new InvalidOperationException($"PacketFish with ID {item.PacketFishId} not found");
                    }

                    item.PacketFish = packetFish;

                    var totalAvailableFish = packetFish.PondPacketFishes
                        .Where(ppf => ppf.IsActive)
                        .Sum(ppf => ppf.AvailableQuantity);

                    var totalAvailablePackets = totalAvailableFish / packetFish.FishPerPacket;
                    var requestedFishCount = item.Quantity * packetFish.FishPerPacket;

                    if (totalAvailableFish < requestedFishCount)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for PacketFish '{packetFish.Name}'. " +
                            $"Requested: {item.Quantity} packets ({requestedFishCount} fish), Available: {totalAvailablePackets} packets ({totalAvailableFish} fish)");
                    }
                }
            }
        }

        private (decimal subtotal, List<OrderDetail> orderDetails) BuildOrderDetailsFromCart(ICollection<CartItem> cartItems)
        {
            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in cartItems)
            {
                decimal itemTotal = item.UnitPrice * item.Quantity;
                subtotal += itemTotal;

                orderDetails.Add(new OrderDetail
                {
                    KoiFishId = item.KoiFishId,
                    PacketFishId = item.PacketFishId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            return (subtotal, orderDetails);
        }

        private async Task<Promotion?> GetCurrentPromotionAsync()
        {
            var now = DateTime.UtcNow;
            return await _promotionRepo.GetSingleAsync(new QueryBuilder<Promotion>()
                .WithPredicate(p => p.IsActive && !p.IsDeleted &&
                    p.ValidFrom <= now && p.ValidTo >= now)
                .Build());
        }

        private Order CreateOrderEntity(ConvertCartToOrderDTO dto, int customerId, decimal subtotal, decimal discountAmount, int? promotionId, List<OrderDetail> orderDetails)
        {
            decimal totalAmount = subtotal + dto.ShippingFee - discountAmount;

            return new Order
            {
                CustomerId = customerId,
                CustomerAddressId = dto.CustomerAddressId,
                Status = OrderStatus.Pending,
                Subtotal = subtotal,
                ShippingFee = dto.ShippingFee,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                PromotionId = promotionId,
                OrderDetails = orderDetails
            };
        }

        private async Task ReserveInventoryForOrderAsync(ICollection<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                if (item.KoiFishId.HasValue && item.KoiFish != null)
                {
                    if (item.KoiFish.SaleStatus != SaleStatus.Available)
                    {
                        throw new InvalidOperationException($"KoiFish '{item.KoiFish.RFID}' is no longer available");
                    }

                    item.KoiFish.SaleStatus = SaleStatus.NotForSale;
                    await _koiFishRepo.UpdateAsync(item.KoiFish);
                }

                if (item.PacketFishId.HasValue && item.PacketFish != null)
                {
                    var fishCount = item.Quantity * item.PacketFish.FishPerPacket;
                    await ReservePacketFishInventoryAsync(item.PacketFish, fishCount);
                }
            }
        }

        private async Task ReservePacketFishInventoryAsync(PacketFish packetFish, int fishCount)
        {
            var pondPacketFishRepo = _unitOfWork.GetRepo<PondPacketFish>();
            int remainingFish = fishCount;

            foreach (var pondPacket in packetFish.PondPacketFishes
                .Where(ppf => ppf.IsActive && ppf.AvailableQuantity > 0)
                .OrderBy(ppf => ppf.Id))
            {
                if (remainingFish <= 0) break;

                int deduct = Math.Min(pondPacket.AvailableQuantity, remainingFish);
                pondPacket.AvailableQuantity -= deduct;
                remainingFish -= deduct;

                await pondPacketFishRepo.UpdateAsync(pondPacket);
            }

            if (remainingFish > 0)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for PacketFish '{packetFish.Name}'. " +
                    $"Still need {remainingFish} more fish.");
            }
        }

        private async Task IncrementPromotionUsageAsync(Promotion? promotion)
        {
            if (promotion == null) return;

            // Use pessimistic locking (SQL Server UPDLOCK, ROWLOCK) to prevent race conditions
            // This ensures atomic check-and-increment of promotion usage count
            var promotionToUpdate = await _promotionRepo.GetSingleAsync(new QueryBuilder<Promotion>()
                .WithPredicate(p => p.Id == promotion.Id)
                .WithTracking(true)
                .WithLockForUpdate(true)
                .Build());

            if (promotionToUpdate != null)
            {
                if (promotionToUpdate.UsageLimit.HasValue &&
                    promotionToUpdate.UsageCount >= promotionToUpdate.UsageLimit.Value)
                {
                    throw new InvalidOperationException("Promotion usage limit has been reached");
                }

                promotionToUpdate.UsageCount++;
                await _promotionRepo.UpdateAsync(promotionToUpdate);
            }
        }

        private async Task ClearCartAfterOrderCreationAsync(Cart cart)
        {
            foreach (var item in cart.CartItems.ToList())
            {
                await _cartItemRepo.DeleteAsync(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);
        }

        private async Task<Order> LoadCreatedOrderWithDetailsAsync(int orderId)
        {
            var order = await _unitOfWork.GetRepo<Order>().GetSingleAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Id == orderId)
                .WithInclude(o => o.Customer)
                .WithInclude(o => o.Customer.ApplicationUser)
                .WithInclude(o => o.Promotion)
                .WithInclude(o => o.OrderDetails)
                .Build());

            if (order != null && order.OrderDetails.Any())
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

            return order!;
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

        private async Task CalculateAndApplyPromotionAsync(CartResponseDTO cartResponse)
        {
            try
            {
                var currentPromotion = await _promotionService.GetCurrentActivePromotionAsync();
                
                if (currentPromotion == null)
                {
                    cartResponse.DiscountAmount = 0;
                    cartResponse.FinalPrice = cartResponse.TotalPrice;
                    cartResponse.Promotion = null;
                    return;
                }

                if (cartResponse.TotalPrice < currentPromotion.MinimumOrderAmount)
                {
                    cartResponse.DiscountAmount = 0;
                    cartResponse.FinalPrice = cartResponse.TotalPrice;
                    cartResponse.Promotion = null;
                    return;
                }
                decimal discountAmount = 0;

                if (currentPromotion.DiscountType == DiscountType.Percentage)
                {
                    discountAmount = cartResponse.TotalPrice * (currentPromotion.DiscountValue / 100m);

                    if (currentPromotion.MaxDiscountAmount.HasValue)
                    {
                        discountAmount = Math.Min(discountAmount, currentPromotion.MaxDiscountAmount.Value);
                    }
                }
                else if (currentPromotion.DiscountType == DiscountType.FixedAmount)
                {
                    discountAmount = currentPromotion.DiscountValue;
                }

                discountAmount = Math.Min(discountAmount, cartResponse.TotalPrice);

                cartResponse.DiscountAmount = discountAmount;
                cartResponse.FinalPrice = cartResponse.TotalPrice - discountAmount;
                cartResponse.Promotion = new PromotionInfoDTO
                {
                    Id = currentPromotion.Id,
                    Code = currentPromotion.Code,
                    Description = currentPromotion.Description
                };
            }
            catch
            {
                cartResponse.DiscountAmount = 0;
                cartResponse.FinalPrice = cartResponse.TotalPrice;
                cartResponse.Promotion = null;
            }
        }
        private void CalculatePacketFishStockQuantity(CartItemResponseDTO itemDto, CartItem cartItemEntity)
        {
            if (itemDto.PacketFish != null && cartItemEntity.PacketFish != null && cartItemEntity.PacketFish.PondPacketFishes != null)
            {
                var totalAvailableFish = cartItemEntity.PacketFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity);
                itemDto.PacketFish.StockQuantity = totalAvailableFish / cartItemEntity.PacketFish.FishPerPacket;
            }
        }

        private void CheckCartItemAvailability(CartItemResponseDTO itemDto, CartItem cartItem)
        {
            itemDto.IsAvailable = true;
            itemDto.UnavailableReason = null;
            itemDto.AvailableStock = null;

            if (cartItem.KoiFishId.HasValue && cartItem.KoiFish != null)
            {
                var koiFish = cartItem.KoiFish;

                if (koiFish.SaleStatus == SaleStatus.Sold)
                {
                    itemDto.IsAvailable = false;
                    itemDto.UnavailableReason = "Cá đã được bán";
                }
                else if (koiFish.SaleStatus == SaleStatus.NotForSale)
                {
                    itemDto.IsAvailable = false;
                    itemDto.UnavailableReason = "Cá không còn bán";
                }
                else if (koiFish.SaleStatus != SaleStatus.Available)
                {
                    itemDto.IsAvailable = false;
                    itemDto.UnavailableReason = "Cá không còn available";
                }
            }
            else if (cartItem.KoiFishId.HasValue && cartItem.KoiFish == null)
            {
                itemDto.IsAvailable = false;
                itemDto.UnavailableReason = "Cá không tồn tại";
            }
            if (cartItem.PacketFishId.HasValue && cartItem.PacketFish != null)
            {
                var packetFish = cartItem.PacketFish;

                if (!packetFish.IsAvailable)
                {
                    itemDto.IsAvailable = false;
                    itemDto.UnavailableReason = "Sản phẩm không còn bán";
                }
                else
                {
                    var totalAvailableFish = packetFish.PondPacketFishes
                        ?.Where(ppf => ppf.IsActive)
                        .Sum(ppf => ppf.AvailableQuantity) ?? 0;

                    var totalAvailablePackets = totalAvailableFish / packetFish.FishPerPacket;
                    var requestedFishCount = cartItem.Quantity * packetFish.FishPerPacket;

                    itemDto.AvailableStock = totalAvailablePackets;

                    if (totalAvailableFish < requestedFishCount)
                    {
                        itemDto.IsAvailable = false;
                        if (totalAvailablePackets == 0)
                        {
                            itemDto.UnavailableReason = "Hết hàng";
                        }
                        else
                        {
                            itemDto.UnavailableReason = $"Không đủ số lượng (chỉ còn {totalAvailablePackets} packets)";
                        }
                    }
                }
            }
            else if (cartItem.PacketFishId.HasValue && cartItem.PacketFish == null)
            {
                itemDto.IsAvailable = false;
                itemDto.UnavailableReason = "Sản phẩm không tồn tại";
            }
        }

    }
}
