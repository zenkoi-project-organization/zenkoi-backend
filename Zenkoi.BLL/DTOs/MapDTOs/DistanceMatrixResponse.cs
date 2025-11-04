using System.Text.Json.Serialization;

namespace Zenkoi.BLL.DTOs.GoogleMapsDTOs
{
    public class DistanceMatrixResponse
    {
        [JsonPropertyName("destination_addresses")]
        public List<string> DestinationAddresses { get; set; } = new List<string>();

        [JsonPropertyName("origin_addresses")]
        public List<string> OriginAddresses { get; set; } = new List<string>();

        [JsonPropertyName("rows")]
        public List<DistanceMatrixRow> Rows { get; set; } = new List<DistanceMatrixRow>();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
    }

    public class DistanceMatrixRow
    {
        [JsonPropertyName("elements")]
        public List<DistanceMatrixElement> Elements { get; set; } = new List<DistanceMatrixElement>();
    }

    public class DistanceMatrixElement
    {
        [JsonPropertyName("distance")]
        public DistanceInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public DurationInfo? Duration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class DistanceInfo
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class DurationInfo
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
