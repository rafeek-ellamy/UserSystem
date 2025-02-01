namespace Configurations.GenericApiResponse
{
    public class PagedApiResponse<T> : ApiResponse<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PagedApiResponse(bool success, T items, int totalCount, int pageIndex, int pageSize) : base(success, items, string.Empty)
        {
            PageIndex = pageIndex;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
