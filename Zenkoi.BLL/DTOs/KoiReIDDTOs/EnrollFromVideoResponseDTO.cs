using System;
using System.Collections.Generic;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class EnrollFromVideoResponseDTO
    {
        public bool Success { get; set; }
        public string FishId { get; set; } = string.Empty;
        public int NumFramesExtracted { get; set; }
        public int NumValidEmbeddings { get; set; }
        public List<string> FrameUrls { get; set; } = new();
        public int TotalFishInGallery { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string ExtractedPublicId { get; set; } = string.Empty;

        public int EnrollmentId { get; set; }
        public int KoiFishId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string EnrolledBy { get; set; } = string.Empty;
    }
}
