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

	public async Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role? role, int pageIndex, int pageSize, string? search = null, bool? isBlocked = null)
	{
		var repo = _unitOfWork.GetRepo<ApplicationUser>();

		Expression<Func<ApplicationUser, bool>> predicate = x => !x.IsDeleted;

		Expression<Func<ApplicationUser, bool>> excludeManagerExpr = x => x.Role != Role.Manager;
		predicate = predicate.AndAlso(excludeManagerExpr);

		if (role.HasValue)
		{
			Expression<Func<ApplicationUser, bool>> roleExpr = x => x.Role == role.Value;
			predicate = predicate.AndAlso(roleExpr);
		}

		if (isBlocked.HasValue)
		{
			Expression<Func<ApplicationUser, bool>> blockedExpr = x => x.IsBlocked == isBlocked.Value;
			predicate = predicate.AndAlso(blockedExpr);
		}

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

	public async Task<ApplicationUserResponseDTO> UpdateUserProfileAsync(int userId, UpdateUserProfileDTO dto)
	{
		var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
		var userDetailRepo = _unitOfWork.GetRepo<UserDetail>();

		var user = await userRepo.GetSingleAsync(new QueryBuilder<ApplicationUser>()
			.WithPredicate(u => u.Id == userId && !u.IsDeleted)
			.WithInclude(u => u.UserDetail)
			.Build());

		if (user == null)
		{
			throw new ArgumentException("Không tìm thấy người dùng.");
		}

		await _unitOfWork.BeginTransactionAsync();

		try
		{		
			if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber != user.PhoneNumber)
			{	
				var phonePattern = @"^(0\d{9}|\+84\d{9})$";
				if (!System.Text.RegularExpressions.Regex.IsMatch(dto.PhoneNumber, phonePattern))
				{
					throw new ArgumentException("Số điện thoại không hợp lệ. Định dạng: 10 chữ số bắt đầu bằng 0 (vd: 0912345678) hoặc +84 theo sau 9 chữ số (vd: +84912345678).");
				}

				var phoneExists = await userRepo.AnyAsync(new QueryBuilder<ApplicationUser>()
					.WithPredicate(u => u.PhoneNumber == dto.PhoneNumber && u.Id != userId && !u.IsDeleted)
					.Build());

				if (phoneExists)
				{
					throw new ArgumentException($"Số điện thoại '{dto.PhoneNumber}' đã được sử dụng bởi tài khoản khác.");
				}
			}

			if (!string.IsNullOrWhiteSpace(dto.FullName))
			{
				user.FullName = dto.FullName;
			}

			if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
			{
				user.PhoneNumber = dto.PhoneNumber;
			}

			await userRepo.UpdateAsync(user);

			if (user.UserDetail == null)
			{
				user.UserDetail = new UserDetail
				{
					ApplicationUserId = userId,
					DateOfBirth = dto.DateOfBirth,
					Gender = dto.Gender ?? Gender.Male,
					AvatarURL = dto.AvatarURL,
					Address = dto.Address
				};
				await userDetailRepo.CreateAsync(user.UserDetail);
			}
			else
			{
				if (dto.DateOfBirth.HasValue)
				{
					user.UserDetail.DateOfBirth = dto.DateOfBirth;
				}

				if (dto.Gender.HasValue)
				{
					user.UserDetail.Gender = dto.Gender.Value;
				}

				if (dto.AvatarURL != null)
				{
					user.UserDetail.AvatarURL = dto.AvatarURL;
				}

				if (dto.Address != null)
				{
					user.UserDetail.Address = dto.Address;
				}

				await userDetailRepo.UpdateAsync(user.UserDetail);
			}

			await _unitOfWork.SaveChangesAsync();
			await _unitOfWork.CommitTransactionAsync();

			return _mapper.Map<ApplicationUserResponseDTO>(user);
		}
		catch
		{
			await _unitOfWork.RollBackAsync();
			throw;
		}
	}
	}
}

