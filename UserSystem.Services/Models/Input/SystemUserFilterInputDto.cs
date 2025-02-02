using UserSystem.Services.Models.Common;

namespace UserSystem.Services.Models.Input
{
    public class SystemUserFilterInputDto : PaginatedListInputDto
    {
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
