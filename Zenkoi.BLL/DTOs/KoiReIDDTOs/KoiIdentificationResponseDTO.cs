using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Zenkoi.BLL.DTOs.KoiFishDTOs;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class KoiIdentificationResponseDTO
    {
        public int Id { get; set; }
        public KoiFishResponseDTO? KoiFish { get; set; }
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
        [JsonPropertyName("fishId")]
        public string FishIdInGallery { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }

        public KoiFishResponseDTO? KoiFish { get; set; }
    }
}
