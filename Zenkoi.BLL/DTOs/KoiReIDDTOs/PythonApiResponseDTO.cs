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

        [JsonPropertyName("fishId")]
        public string FishId { get; set; }

        [JsonPropertyName("numImages")]
        public int NumImages { get; set; }

        [JsonPropertyName("numUrlsProvided")]
        public int NumUrlsProvided { get; set; }

        [JsonPropertyName("numDownloaded")]
        public int NumDownloaded { get; set; }

        [JsonPropertyName("totalFishInGallery")]
        public int TotalFishInGallery { get; set; }
    }

    public class PythonEnrollFromVideoResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("fishId")]
        public string FishId { get; set; }

        [JsonPropertyName("numFramesExtracted")]
        public int NumFramesExtracted { get; set; }

        [JsonPropertyName("numValidEmbeddings")]
        public int NumValidEmbeddings { get; set; }

        [JsonPropertyName("frameUrls")]
        public List<string> FrameUrls { get; set; } = new();

        [JsonPropertyName("totalFishInGallery")]
        public int TotalFishInGallery { get; set; }

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; }

        [JsonPropertyName("extractedPublicId")]
        public string ExtractedPublicId { get; set; }
    }

    public class PythonIdentifyResponseDTO
    {
        [JsonPropertyName("fishId")]
        public string FishId { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }

        [JsonPropertyName("isUnknown")]
        public bool IsUnknown { get; set; }

        [JsonPropertyName("topPredictions")]
        public List<PythonTopPredictionDTO> TopPredictions { get; set; }
    }

    public class PythonTopPredictionDTO
    {
        [JsonPropertyName("fishId")]
        public string FishId { get; set; }

        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }

        [JsonPropertyName("similarity")]
        public decimal Similarity { get; set; }
    }
}
