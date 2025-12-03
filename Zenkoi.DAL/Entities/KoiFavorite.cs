using System;

namespace Zenkoi.DAL.Entities
{
    public class KoiFavorite
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int KoiFishId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Customer Customer { get; set; } = null!;
        public KoiFish KoiFish { get; set; } = null!;
    }
}
