using System.Collections.Generic;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiReIDDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IKoiReIDService
    {
        Task<EnrollFromCloudinaryResponseDTO> EnrollKoiFromCloudinaryAsync(
            int koiFishId,
            List<string> cloudinaryUrls,
            int userId,
            bool overrideExisting = false);

        Task<EnrollFromVideoResponseDTO> EnrollKoiFromVideoAsync(
            int koiFishId,
            string videoUrl,
            int userId,
            bool overrideExisting = false);

        Task<KoiIdentificationResponseDTO> IdentifyKoiFromUrlAsync(
            string imageUrl,
            int userId);

        Task<PaginatedList<KoiGalleryEnrollmentResponseDTO>> GetEnrollmentsAsync(
            int? koiFishId = null,
            bool? isActive = null,
            int pageIndex = 1,
            int pageSize = 10);

        Task<PaginatedList<KoiIdentificationResponseDTO>> GetIdentificationHistoryAsync(
            int? koiFishId = null,
            bool? isUnknown = null,
            int pageIndex = 1,
            int pageSize = 10);

        Task<KoiIdentificationResponseDTO> GetIdentificationByIdAsync(int id);

        Task<bool> DeactivateEnrollmentAsync(int enrollmentId, int userId);
    }
}
