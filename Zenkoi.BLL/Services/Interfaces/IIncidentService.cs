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
        Task<IncidentResponseDTO> CreateAsync(int userId, IncidentRequestDTO dto);
        Task<IncidentResponseDTO> UpdateAsync(int id, IncidentUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id, UpdateStatusDto dto);
        Task<bool> ResolveAsync(int id, int userId, ResolveIncidentDTO dto);
        Task<KoiIncidentSimpleDTO> AddKoiIncidentAsync(int incidentId, KoiIncidentRequestDTO dto);
        Task<bool> RemoveKoiIncidentAsync(int koiIncidentId);
        Task<PondIncidentSimpleDTO> AddPondIncidentAsync(int incidentId, PondIncidentRequestDTO dto);
        Task<bool> RemovePondIncidentAsync(int pondIncidentId);
    }
}
