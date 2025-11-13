using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.DTOs
{
	public class PagingDTO<T> where T : class
	{
		public int PageIndex { get; set; }
		public int TotalPages { get; set; }
		public int TotalItems { get; set; }
		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;
		public PaginatedList<T> Data { get; set; }

		public PagingDTO(PaginatedList<T> pagedEntity)
		{
			PageIndex = pagedEntity.PageIndex;
			TotalPages = pagedEntity.TotalPages;
			TotalItems = pagedEntity.TotalItems;
			Data = pagedEntity;
		}
	}
}
