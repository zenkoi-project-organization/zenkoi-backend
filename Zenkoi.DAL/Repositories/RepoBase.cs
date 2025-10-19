using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Zenkoi.DAL.EF;
using Zenkoi.DAL.Queries;

namespace Zenkoi.DAL.Repositories
{
	public class RepoBase<T> : IRepoBase<T> where T : class
	{
		private readonly ZenKoiContext _context;
		protected readonly DbSet<T> _dbSet;
		public RepoBase(ZenKoiContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}


		public async Task<T> CreateAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			return entity;
		}

		public async Task CreateAllAsync(List<T> entities)
		{
			await _dbSet.AddRangeAsync(entities);
		}

		public Task DeleteAsync(T entity)
		{
			if (_context.Entry<T>(entity).State == EntityState.Detached)
			{
				_dbSet.Attach(entity);
			}
			_dbSet.Remove(entity);

			return Task.CompletedTask;
		}

		public Task DeleteAllAsync(List<T> entities)
		{
			_dbSet.RemoveRange(entities);
			return Task.CompletedTask;
		}

		public IQueryable<T> Get(QueryOptions<T> options)
		{
			IQueryable<T> query = _dbSet;

			if (!options.Tracked)
			{
				query = query.AsNoTracking();
			}

			if (options.IncludeProperties?.Any() ?? false)
			{
				foreach (var includeProperty in options.IncludeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			if (options.ThenIncludeProperties?.Any() ?? false)
			{
				foreach (var thenIncludeProperty in options.ThenIncludeProperties)
				{
					query = ((IIncludableQueryable<T, object>)query).ThenInclude(thenIncludeProperty);
				}
			}

			if (options.Predicate != null)
			{
				query = query.Where(options.Predicate);
			}

			if (options.OrderBy != null)
			{
				query = options.OrderBy(query);
			}

			return query;
		}

		public Task UpdateAsync(T entity)
		{

			if (_context.Entry<T>(entity).State == EntityState.Detached)
			{
				_dbSet.Attach(entity);
			}
			_dbSet.Update(entity);

			return Task.CompletedTask;
		}

		public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options)
		{
			return await Get(options).ToListAsync();
		}

		public async Task<T> GetSingleAsync(QueryOptions<T> options)
		{
			return await Get(options).FirstOrDefaultAsync();
		}

		public async Task<bool> AnyAsync(QueryOptions<T> options)
		{
			if (options.Predicate != null)
			{
				var result = await _dbSet.AnyAsync(options.Predicate);
				return result;
			}
			return false;
		}
        public async Task<T?> GetByIdAsync(int? id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<bool> CheckExistAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }
    }
}
