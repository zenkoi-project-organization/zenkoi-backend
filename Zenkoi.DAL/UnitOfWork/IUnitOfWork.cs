using Zenkoi.DAL.Repositories;

namespace Zenkoi.DAL.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepoBase<T> GetRepo<T>() where T : class;
		Task SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollBackAsync();
		Task<bool> SaveAsync();
	}
}
