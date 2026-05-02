namespace AiNewsFeed.DTOs;

public class FeedRefreshResponseDto
{
    public int NewArticlesAdded { get; set; }
    public int FailedSources { get; set; }
    public List<string> FailedSourceNames { get; set; } = new();
    public int DisabledSources { get; set; }
}