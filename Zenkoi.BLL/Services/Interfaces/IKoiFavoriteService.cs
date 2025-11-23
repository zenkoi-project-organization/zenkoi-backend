using System.Threading.Tasks;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IKoiFavoriteService
    {
        Task<bool> AddFavoriteAsync(int userId, int koiFishId);
        Task<bool> RemoveFavoriteAsync(int userId, int koiFishId);
        Task<bool> IsFavoriteAsync(int userId, int koiFishId);
    }
}
