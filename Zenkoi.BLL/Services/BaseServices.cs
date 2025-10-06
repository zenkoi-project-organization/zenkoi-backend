using AutoMapper;
using Zenkoi.BLL.Helpers.Fillters;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.UnitOfWork;


namespace Zenkoi.BLL.Services
{
	public abstract class BaseServices<TEntity, TDetailDto> where TEntity : class
	{
		protected readonly IUnitOfWork _unitOfWork;
		protected readonly IMapper _mapper;

		public BaseServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<TEntity> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<TEntity>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<TEntity>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<PaginatedList<TDetailDto>> GetPagedData(IQueryable<TEntity> query, int pageIndex, int pageSize)
		{
			var paginatedEntities = await PaginatedList<TEntity>.CreateAsync(query, pageIndex, pageSize);
			var resultDto = _mapper.Map<List<TDetailDto>>(paginatedEntities);

			return new PaginatedList<TDetailDto>(resultDto, paginatedEntities.TotalItems, pageIndex, pageSize);
		}
	}
}
