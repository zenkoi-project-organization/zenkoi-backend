using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        
        public int? KoiFishId { get; set; }
        public KoiFish? KoiFish { get; set; }
        
        public int? PacketFishId { get; set; }
        public PacketFish? PacketFish { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

