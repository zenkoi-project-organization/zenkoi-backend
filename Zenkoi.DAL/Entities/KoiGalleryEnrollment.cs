using System;

namespace Zenkoi.DAL.Entities
{
    public class KoiGalleryEnrollment
    {
        public int Id { get; set; }
        public int KoiFishId { get; set; }
        public string FishIdInGallery { get; set; } = string.Empty;
        public int NumImages { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public int? EnrolledBy { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }

        public KoiFish KoiFish { get; set; } = null!;
        public ApplicationUser? EnrolledByUser { get; set; }
    }
}
