using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Zenkoi.DAL.Queries
{
	public class QueryBuilder<T> where T : class
	{
		private readonly QueryOptions<T> _options = new QueryOptions<T>();

		public QueryBuilder<T> WithPredicate(Expression<Func<T, bool>> predicate)
		{
			if (_options.Predicate == null)
			{
				_options.Predicate = predicate;
			}
			else
			{
				_options.Predicate = _options.Predicate.AndAlso(predicate);
			}
			return this;
		}

		public QueryBuilder<T> WithTracking(bool tracked)
		{
			_options.Tracked = tracked;
			return this;
		}

		public QueryBuilder<T> WithLockForUpdate(bool lockForUpdate)
		{
			_options.LockForUpdate = lockForUpdate;
			return this;
		}

		public QueryBuilder<T> WithOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
		{
			_options.OrderBy = orderBy;
			return this;
		}

		public QueryBuilder<T> WithInclude(params Expression<Func<T, object>>[] includeProperty)
		{
			_options.IncludeProperties.AddRange(includeProperty);
			return this;
		}

		public QueryBuilder<T> WithInclude(params string[] includeStrings)
		{
			_options.IncludeStrings.AddRange(includeStrings);
			return this;
		}

		public QueryBuilder<T> WithThenInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression)
		{
			_options.ThenIncludeExpressions.Add(includeExpression);
			return this;
		}

		public QueryOptions<T> Build()
		{
			return _options;
		}
	}
}
