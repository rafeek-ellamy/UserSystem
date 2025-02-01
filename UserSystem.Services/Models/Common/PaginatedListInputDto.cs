namespace UserSystem.Services.Models.Common
{
    public class PaginatedListInputDto
    {
        public virtual int PageIndex { get; set; } = 1;
        public virtual int PageSize { get; set; } = 10;
    }
}
