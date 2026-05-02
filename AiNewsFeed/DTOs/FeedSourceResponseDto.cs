namespace AiNewsFeed.DTOs;

public class FeedSourceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FeedUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastFetchedAt { get; set; }
    public string? LastFetchStatus { get; set; }
    public string? LastErrorMessage { get; set; }
    public int ConsecutiveFailures { get; set; }
}