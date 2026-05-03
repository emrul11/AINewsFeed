namespace AiNewsFeed.DTOs
{
    public class PagedArticleResponseDto
    {
        public List<ArticleResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
