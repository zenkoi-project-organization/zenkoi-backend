using AutoMapper;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.DTOs.UserDetailDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
	public class UserDetailService : IUserDetailService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UserDetailService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BaseResponse> CreateUpdateUserDetail(UserDetailRequestDTO dto, int userId)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<UserDetail>();
				await _unitOfWork.BeginTransactionAsync();

				var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
												.WithPredicate(x => x.ApplicationUserId == userId)
												.Build());
				if (any)
				{
					var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
																.WithPredicate(x => x.ApplicationUserId == userId)
																.Build());
					if (userId != userDetail.ApplicationUserId) return new BaseResponse { IsSuccess = false, Message = "Bạn không có quyền chỉnh sửa thông tin người dùng này" };

					var updateUserDetail = _mapper.Map(dto, userDetail);
					await repo.UpdateAsync(updateUserDetail);
				}
				else
				{
					var userDetail = _mapper.Map<UserDetail>(dto);
					userDetail.ApplicationUserId = userId;
					await repo.CreateAsync(userDetail);
				}
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Không thể lưu thông tin người dùng vào cơ sở dữ liệu" };
				return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công" };
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> DeleteUserDetail(int userId)
		{
			var repo = _unitOfWork.GetRepo<UserDetail>();
			var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
											.WithPredicate(x => x.ApplicationUserId == userId)
											.Build());
			if (any)
			{
				var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
															.WithPredicate(x => x.ApplicationUserId == userId)
															.Build());
				if (userId != userDetail.ApplicationUserId) return new BaseResponse { IsSuccess = false, Message = "Bạn không có quyền xóa thông tin người dùng này" };
				await repo.DeleteAsync(userDetail);
				var saver = await _unitOfWork.SaveAsync();
				if (!saver) return new BaseResponse { IsSuccess = false, Message = "Không thể xóa thông tin người dùng khỏi cơ sở dữ liệu" };
				return new BaseResponse { IsSuccess = true, Message = "Xóa dữ liệu thành công" };
			}

			return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy thông tin người dùng trong hệ thống" };
		}

        public async Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetails(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
                                        .WithPredicate(x => true)
                                        .WithInclude(x => x.User)
                                        .Build());
            var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
            var resultDTO = _mapper.Map<List<UserDetailResponseDTO>>(pagedRecords);
            return new PaginatedList<UserDetailResponseDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetailsByName(int pageIndex, int pageSize, string? name)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
                                        .WithPredicate(x => true)
                                        .WithInclude(x => x.User)
                                        .Build());
            if (!string.IsNullOrEmpty(name))
            {
                loadedRecords = loadedRecords.Where(x => x.User.FullName.Contains(name));
            }
            var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
            var resultDTO = _mapper.Map<List<UserDetailResponseDTO>>(pagedRecords);
            return new PaginatedList<UserDetailResponseDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
        }

        public async Task<UserDetailResponseDTO> GetUserDetailByUserId(int userId)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var response = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
                                                    .WithPredicate(x => x.ApplicationUserId ==userId)
                                                    .WithInclude(x => x.User)
                                                    .WithTracking(false)
                                                    .Build());
            if (response == null) return null;
            return _mapper.Map<UserDetailResponseDTO>(response);
        }
    }
}
