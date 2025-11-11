using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.IncidentTypeDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IIncidentTypeService
    {
        Task<PaginatedList<IncidentTypeResponseDTO>> GetAllIncidentTypesAsync(
            IncidentTypeFilterRequestDTO filter,
            int pageIndex = 1,
            int pageSize = 10);
        Task<IncidentTypeResponseDTO> GetIncidentTypeByIdAsync(int id);
        Task<IncidentTypeResponseDTO> CreateIncidentTypeAsync(IncidentTypeRequestDTO dto);
        Task<IncidentTypeResponseDTO> UpdateIncidentTypeAsync(int id, IncidentTypeUpdateRequestDTO dto);
        Task<bool> DeleteIncidentTypeAsync(int id);
    }
}

