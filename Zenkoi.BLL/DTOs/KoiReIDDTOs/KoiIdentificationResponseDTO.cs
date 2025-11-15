using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("fish_id")]
        public string FishIdInGallery { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }
    }
}
