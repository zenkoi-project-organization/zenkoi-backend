using AutoMapper;
using System.Linq.Expressions;
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

	public async Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role role, int pageIndex, int pageSize, string? search = null)
	{
		var repo = _unitOfWork.GetRepo<ApplicationUser>();

		Expression<Func<ApplicationUser, bool>> predicate = x => x.Role == role && !x.IsDeleted;

		if (!string.IsNullOrWhiteSpace(search))
		{
			string searchLower = search.ToLower();
			Expression<Func<ApplicationUser, bool>> searchExpr = x =>
				(x.FullName != null && x.FullName.ToLower().Contains(searchLower)) ||
				(x.Email != null && x.Email.ToLower().Contains(searchLower)) ||
				(x.UserName != null && x.UserName.ToLower().Contains(searchLower));
			predicate = predicate.AndAlso(searchExpr);
		}

		var users = repo.Get(new QueryBuilder<ApplicationUser>()
			.WithPredicate(predicate)
			.WithTracking(false)
			.Build());

		var pagedUsers = await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex, pageSize);
		var result = _mapper.Map<List<ApplicationUserResponseDTO>>(pagedUsers);
		return new PaginatedList<ApplicationUserResponseDTO>(result, pagedUsers.TotalItems, pageIndex, pageSize);
	}
	}
}

