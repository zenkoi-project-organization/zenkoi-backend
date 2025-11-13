using System;
using System.Collections.Generic;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class KoiIdentificationResponseDTO
    {
        public int Id { get; set; }
        public int? KoiFishId { get; set; }
        public string? KoiFishRFID { get; set; }
        public string ImageUrl { get; set; }
        public string IdentifiedAs { get; set; }
        public decimal Confidence { get; set; }
        public decimal Distance { get; set; }
        public bool IsUnknown { get; set; }
        public List<TopPredictionDTO>? TopPredictions { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class TopPredictionDTO
    {
        public string FishId { get; set; }
        public decimal Distance { get; set; }
        public decimal Similarity { get; set; }
    }
}
