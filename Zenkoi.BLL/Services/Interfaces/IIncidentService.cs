using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.IncidentDTOs;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IIncidentService
    {
        Task<PaginatedList<IncidentResponseDTO>> GetAllIncidentsAsync(IncidentFilterDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<IncidentResponseDTO> GetByIdAsync(int id);
        Task<IncidentResponseDTO> CreateIncidentWithDetailsAsync(int userId, CreateIncidentWithDetailsDTO dto);
        Task<IncidentResponseDTO> UpdateAsync(int id, int userId, IncidentUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<IncidentResponseDTO> ChangeStatusAsync(int id, int userId, IncidentStatus status, string? resolutionNotes);
        Task<KoiIncidentResponseDTO> AddKoiIncidentAsync(int incidentId, KoiIncidentRequestDTO dto);
        Task<PondIncidentResponseDTO> AddPondIncidentAsync(int incidentId, PondIncidentRequestDTO dto);
        Task<KoiIncidentResponseDTO> UpdateKoiIncidentAsync(int koiIncidentId, UpdateKoiIncidentRequestDTO dto);
        Task<PondIncidentResponseDTO> UpdatePondIncidentAsync(int pondIncidentId, UpdatePondIncidentRequestDTO dto);
        Task<List<KoiIncidentResponseDTO>> GetKoiHealthHistoryAsync(int koiFishId);
    }
}
