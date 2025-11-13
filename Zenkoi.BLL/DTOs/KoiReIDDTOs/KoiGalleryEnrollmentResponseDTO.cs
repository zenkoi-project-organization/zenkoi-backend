using System;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class KoiGalleryEnrollmentResponseDTO
    {
        public int Id { get; set; }
        public int KoiFishId { get; set; }
        public string KoiFishRFID { get; set; }
        public string FishIdInGallery { get; set; }
        public int NumImages { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string? EnrolledByName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
