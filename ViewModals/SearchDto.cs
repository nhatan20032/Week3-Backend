namespace EFCorePracticeAPI.ViewModals
{
    public class SearchDto
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; } = string.Empty;
    }
}
