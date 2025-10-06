using AutoMapper;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UserService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role role, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<ApplicationUser>();

			var users = repo.Get(new QueryBuilder<ApplicationUser>()
				.WithPredicate(x => x.Role == role && !x.IsDeleted)			
				.Build());

			var pagedUsers = await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex, pageSize);
			var result = _mapper.Map<List<ApplicationUserResponseDTO>>(pagedUsers);
			return new PaginatedList<ApplicationUserResponseDTO>(result, pagedUsers.TotalItems, pageIndex, pageSize);
		}
	}
}

