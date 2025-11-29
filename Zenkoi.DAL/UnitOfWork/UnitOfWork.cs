using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Zenkoi.DAL.EF;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;


namespace Zenkoi.DAL.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ZenKoiContext _context;
		private readonly IServiceProvider _serviceProvider;
		private IDbContextTransaction _transaction;
		private IRepoBase<PaymentTransaction> _paymentTransactions;
		
	

		public UnitOfWork(ZenKoiContext masterContext, IServiceProvider serviceProvider)
		{
			_context = masterContext;
			_serviceProvider = serviceProvider;
		}

        public IRepoBase<PaymentTransaction> PaymentTransactions
		{
			get
			{
				if (_paymentTransactions == null)
				{
					_paymentTransactions = _serviceProvider.GetRequiredService<IRepoBase<PaymentTransaction>>();
				}
				return _paymentTransactions;
			}
		}
		public async Task BeginTransactionAsync()
		{
			_transaction = await _context.Database.BeginTransactionAsync();
		}

		public async Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
		{
			var isolationLevelName = isolationLevel switch
			{
				System.Data.IsolationLevel.Serializable => "SERIALIZABLE",
				System.Data.IsolationLevel.RepeatableRead => "REPEATABLE READ",
				System.Data.IsolationLevel.ReadCommitted => "READ COMMITTED",
				System.Data.IsolationLevel.ReadUncommitted => "READ UNCOMMITTED",
				System.Data.IsolationLevel.Snapshot => "SNAPSHOT",
				_ => "READ COMMITTED"
			};

			await _context.Database.ExecuteSqlRawAsync($"SET TRANSACTION ISOLATION LEVEL {isolationLevelName}");
			_transaction = await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransactionAsync()
		{
			try
			{
				await _transaction.CommitAsync();
			}
			catch
			{
				await _transaction.RollbackAsync();
			}
			finally
			{
				await _transaction.DisposeAsync();
				_transaction = null!;
			}
		}

		private bool disposed = false;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
                disposed = true;
			}
		}
		public IRepoBase<T> GetRepo<T>() where T : class
		{
			return _serviceProvider.GetRequiredService<IRepoBase<T>>();
		}



		public async Task RollBackAsync()
		{
			await _transaction.RollbackAsync();
			await _transaction.DisposeAsync();
			_transaction = null!;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<bool> SaveAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}


	}
}
