namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IMapService
    {
        Task<decimal> CalculateDistanceAsync(decimal originLat, decimal originLng, decimal destLat, decimal destLng);
        Task<(decimal distanceKm, int durationMinutes)> GetDistanceAndDurationAsync(decimal originLat, decimal originLng, decimal destLat, decimal destLng);
        Task<decimal> CalculateDistanceByAddressAsync(string originAddress, string destinationAddress);
    }
}
