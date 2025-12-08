using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IKoiFishService
    {
        Task<PaginatedList<KoiFishResponseDTO>> GetAllKoiFishAsync(KoiFishFilterRequestDTO filter, int pageIndex = 1,int pageSize = 10, int? userId = null);
        Task<KoiFishResponseDTO?> GetByIdAsync(int id);
        Task<KoiFishResponseDTO?> ScanRFID(string RFID);
        Task<BreedingParentDTO> GetAnalysisAsync(int id);
        Task<KoiFishResponseDTO> CreateAsync(KoiFishRequestDTO dto);
        Task<bool> TransferFish(int id, int PondId);
        Task<bool> UpdateAsync(int id, KoiFishUpdateRequestDTO dto);
        Task<bool> UpdateKoiSpawning(int id);
        Task<bool> DeleteAsync(int id);
        Task<KoiFishFamilyResponseDTO> GetFamilyTreeAsync(int koiId);
    }
}
