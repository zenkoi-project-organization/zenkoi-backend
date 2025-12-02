using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Zenkoi.DAL.Queries
{
	public class QueryOptions<T>
	{
		public Expression<Func<T, bool>>? Predicate { get; set; }
		public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
		public bool Tracked { get; set; } = true;
		public List<Expression<Func<T, object>>> IncludeProperties { get; set; } = new List<Expression<Func<T, object>>>();
		public List<string> IncludeStrings { get; set; } = new List<string>();
		public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> ThenIncludeExpressions { get; set; } = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>();

	}
}
