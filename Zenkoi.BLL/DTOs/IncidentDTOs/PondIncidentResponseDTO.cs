namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class PondIncidentResponseDTO
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public int PondId { get; set; }
        public string? PondName { get; set; }
        public string? EnvironmentalChanges { get; set; }
        public bool RequiresWaterChange { get; set; }
        public int? FishDiedCount { get; set; }
        public string? CorrectiveActions { get; set; }
        public string? Notes { get; set; }
    }
}
