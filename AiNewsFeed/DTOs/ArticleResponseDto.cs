namespace AiNewsFeed.DTOs;

public class ArticleResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime FetchedAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsDeleted { get; set; }
}