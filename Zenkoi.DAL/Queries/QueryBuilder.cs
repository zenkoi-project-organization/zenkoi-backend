using System.Linq.Expressions;

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
		public QueryBuilder<T> WithThenInclude(params Expression<Func<object, object>>[] thenIncludeProperty)
		{
			_options.ThenIncludeProperties.AddRange(thenIncludeProperty);
			return this;
		}

		public QueryOptions<T> Build()
		{
			return _options;
		}
	}
}
