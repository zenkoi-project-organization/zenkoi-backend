using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;

namespace Zenkoi.DAL.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepoBase<T> GetRepo<T>() where T : class;
		IRepoBase<PaymentTransaction> PaymentTransactions { get; }
		Task SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollBackAsync();
		Task<bool> SaveAsync();
	}
}
