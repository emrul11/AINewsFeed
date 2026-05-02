namespace AiNewsFeed.DTOs;

internal class FeedFetchResult
{
    public string SourceName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int NewArticlesAdded { get; set; }
    public string? ErrorMessage { get; set; }
}