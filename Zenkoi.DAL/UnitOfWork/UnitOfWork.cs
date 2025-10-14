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
		private IRepoBase<Area> _areas;
		private IRepoBase<PondType> _pondTypes;
		private IRepoBase<Pond> _ponds;
		private IRepoBase<Variety> _varieties;
		private IRepoBase<KoiFish> _koiFishes;
	

		public UnitOfWork(ZenKoiContext masterContext, IServiceProvider serviceProvider)
		{
			_context = masterContext;
			_serviceProvider = serviceProvider;
		}


		public IRepoBase<KoiFish> KoiFishes
		{
			get
			{
                if (_koiFishes == null)
                {
                    _koiFishes = _serviceProvider.GetRequiredService<IRepoBase<KoiFish>>();
                }
                return _koiFishes;
            }
		}
		public IRepoBase<Variety> Varieties
		{
			get
			{
                if (_varieties == null)
                {
                    _varieties = _serviceProvider.GetRequiredService<IRepoBase<Variety>>();
                }
                return _varieties;
            }
		}
		public IRepoBase<Pond> Ponds
		{
			get
			{
				if (_ponds == null)
				{
					_ponds = _serviceProvider.GetRequiredService<IRepoBase<Pond>>();
				}
				return _ponds;
			}
		}
        public IRepoBase<PondType> PondTypes
        {
            get
            {
                if (_pondTypes == null)
                {
                    _pondTypes = _serviceProvider.GetRequiredService<IRepoBase<PondType>>();
                }
                return _pondTypes;
            }
        }
        public IRepoBase<Area> Areas
        {
            get
            {
                if (_areas == null)
                {
                    _areas = _serviceProvider.GetRequiredService<IRepoBase<Area>>();
                }
                return _areas;
            }
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
