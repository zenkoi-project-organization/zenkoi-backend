using System;
using System.Threading.Tasks;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class KoiFavoriteService : IKoiFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepoBase<KoiFavorite> _favoriteRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;

        public KoiFavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _favoriteRepo = _unitOfWork.GetRepo<KoiFavorite>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
        }

        public async Task<bool> AddFavoriteAsync(int userId, int koiFishId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Không tìm thấy user với id {userId}.");
            }

            var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId);
            if (koiFish == null)
            {
                throw new ArgumentException($"Không tìm thấy cá Koi với id {koiFishId}.");
            }

            var existingFavorite = await _favoriteRepo.GetSingleAsync(
                new QueryBuilder<KoiFavorite>()
                    .WithPredicate(f => f.UserId == userId && f.KoiFishId == koiFishId)
                    .Build()
            );

            if (existingFavorite != null)
            {
                throw new InvalidOperationException("Cá này đã có trong danh sách yêu thích.");
            }

            var favorite = new KoiFavorite
            {
                UserId = userId,
                KoiFishId = koiFishId,
                CreatedAt = DateTime.UtcNow
            };

            await _favoriteRepo.CreateAsync(favorite);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, int koiFishId)
        {
            var favorite = await _favoriteRepo.GetSingleAsync(
                new QueryBuilder<KoiFavorite>()
                    .WithPredicate(f => f.UserId == userId && f.KoiFishId == koiFishId)
                    .Build()
            );

            if (favorite == null)
            {
                throw new KeyNotFoundException("Không tìm thấy cá trong danh sách yêu thích.");
            }

            await _favoriteRepo.DeleteAsync(favorite);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsFavoriteAsync(int userId, int koiFishId)
        {
            var favorite = await _favoriteRepo.GetSingleAsync(
                new QueryBuilder<KoiFavorite>()
                    .WithPredicate(f => f.UserId == userId && f.KoiFishId == koiFishId)
                    .Build()
            );

            return favorite != null;
        }
    }
}
