using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
	public interface IUserService
	{
		Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role? role, int pageIndex, int pageSize, string? search = null, bool? isBlocked = null);
		Task<ApplicationUserResponseDTO> UpdateUserProfileAsync(int userId, UpdateUserProfileDTO dto);
	}
}
