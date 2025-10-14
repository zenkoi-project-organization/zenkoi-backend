using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;

namespace Zenkoi.DAL.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepoBase<T> GetRepo<T>() where T : class;
		IRepoBase<Area> Areas {  get; }
		IRepoBase<PondType> PondTypes { get; }
		IRepoBase<Pond> Ponds { get; }
        IRepoBase<Variety> Varieties { get; }
		IRepoBase<KoiFish> KoiFishes { get; }
        IRepoBase<PaymentTransaction> PaymentTransactions { get; }
		Task SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollBackAsync();
		Task<bool> SaveAsync();
	}
}
