using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Either a Koi or a PacketFish can be sold
        public int? KoiFishId { get; set; }
        public KoiFish? KoiFish { get; set; }

        public int? PacketFishId { get; set; }
        public PacketFish? PacketFish { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
