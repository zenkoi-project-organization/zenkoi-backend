using System;

namespace Zenkoi.DAL.Entities
{
    public class KoiIdentification
    {
        public int Id { get; set; }
        public int? KoiFishId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string IdentifiedAs { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public decimal Distance { get; set; }
        public bool IsUnknown { get; set; }
        public string? TopPredictions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        // Navigation properties
        public KoiFish? KoiFish { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
    }
}
