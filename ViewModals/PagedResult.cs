namespace EFCorePracticeAPI.ViewModals
{

    public class LoginResult<T>
    {
        public T Data { get; set; } = default!;
        public TokenResult TokenResult { get; set; } = new TokenResult();
    }

    public class TokenResult
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedResultDto<T> where T : class
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public PaginationMeta Meta { get; set; } = new PaginationMeta();
    }

    public class PaginationMeta
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
