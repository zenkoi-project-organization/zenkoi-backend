using Microsoft.Extensions.Options;
using System.Text.Json;
using Zenkoi.BLL.DTOs.GoogleMapsDTOs;
using Zenkoi.BLL.Helpers.Config;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.Implements
{
    public class MapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly MapConfiguration _config;

        public MapService(HttpClient httpClient, IOptions<MapConfiguration> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<decimal> CalculateDistanceAsync(decimal originLat, decimal originLng, decimal destLat, decimal destLng)
        {
            var result = await GetDistanceAndDurationAsync(originLat, originLng, destLat, destLng);
            return result.distanceKm;
        }

        public async Task<(decimal distanceKm, int durationMinutes)> GetDistanceAndDurationAsync(
            decimal originLat, decimal originLng, decimal destLat, decimal destLng)
        {
            try
            {
                var origin = $"{originLat},{originLng}";
                var destination = $"{destLat},{destLng}";

                var url = $"{_config.BaseUrl}/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_config.ApiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<DistanceMatrixResponse>(content);

                if (result == null || result.Status != "OK")
                {
                    throw new Exception($"Map API Error: {result?.ErrorMessage ?? result?.Status ?? "Unknown error"}");
                }

                if (result.Rows.Count == 0 || result.Rows[0].Elements.Count == 0)
                {
                    throw new Exception("No route found between origin and destination");
                }

                var element = result.Rows[0].Elements[0];

                if (element.Status == "ZERO_RESULTS")
                {
                    var haversineDistance = CalculateHaversineDistance(
                        (double)originLat, (double)originLng,
                        (double)destLat, (double)destLng
                    );
                    var estimatedDuration = (int)(haversineDistance / 40 * 60);
                    return ((decimal)haversineDistance, estimatedDuration);
                }

                if (element.Status != "OK")
                {
                    throw new Exception($"Unable to calculate distance: {element.Status}");
                }

                if (element.Distance == null || element.Duration == null)
                {
                    throw new Exception("Distance or duration information not available");
                }

                var distanceInMeters = element.Distance.Value;
                var distanceInKm = (decimal)distanceInMeters / 1000m;

                var durationInSeconds = element.Duration.Value;
                var durationInMinutes = durationInSeconds / 60;

                return (distanceInKm, durationInMinutes);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to call Map API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse Map API response: {ex.Message}", ex);
            }
        }

        public async Task<decimal> CalculateDistanceByAddressAsync(string originAddress, string destinationAddress)
        {
            try
            {
                var encodedOrigin = Uri.EscapeDataString(originAddress);
                var encodedDestination = Uri.EscapeDataString(destinationAddress);

                var url = $"{_config.BaseUrl}/maps/api/distancematrix/json?origins={encodedOrigin}&destinations={encodedDestination}&key={_config.ApiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<DistanceMatrixResponse>(content);

                if (result == null || result.Status != "OK")
                {
                    throw new Exception($"Map API Error: {result?.ErrorMessage ?? result?.Status ?? "Unknown error"}");
                }

                if (result.Rows.Count == 0 || result.Rows[0].Elements.Count == 0)
                {
                    throw new Exception("No route found between origin and destination");
                }

                var element = result.Rows[0].Elements[0];

                if (element.Status == "ZERO_RESULTS")
                {
                    throw new Exception("Cannot calculate distance by address: ZERO_RESULTS. Please use coordinates instead.");
                }

                if (element.Status != "OK")
                {
                    throw new Exception($"Unable to calculate distance: {element.Status}");
                }

                if (element.Distance == null)
                {
                    throw new Exception("Distance information not available");
                }

                var distanceInMeters = element.Distance.Value;
                var distanceInKm = (decimal)distanceInMeters / 1000m;

                return distanceInKm;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to call Map API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse Map API response: {ex.Message}", ex);
            }
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
