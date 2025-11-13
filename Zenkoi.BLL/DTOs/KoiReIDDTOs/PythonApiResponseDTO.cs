using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class PythonEnrollResponseDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("fish_id")]
        public string FishId { get; set; }

        [JsonPropertyName("num_images")]
        public int NumImages { get; set; }

        [JsonPropertyName("num_urls_provided")]
        public int NumUrlsProvided { get; set; }

        [JsonPropertyName("num_downloaded")]
        public int NumDownloaded { get; set; }

        [JsonPropertyName("total_fish_in_gallery")]
        public int TotalFishInGallery { get; set; }
    }
    public class PythonIdentifyResponseDTO
    {
        [JsonPropertyName("fish_id")]
        public string FishId { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }

        [JsonPropertyName("is_unknown")]
        public bool IsUnknown { get; set; }

        [JsonPropertyName("top_predictions")]
        public List<PythonTopPredictionDTO> TopPredictions { get; set; }
    }

    public class PythonTopPredictionDTO
    {
        [JsonPropertyName("fish_id")]
        public string FishId { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }
    }
}
