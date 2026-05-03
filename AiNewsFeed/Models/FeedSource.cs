using System.ComponentModel.DataAnnotations;

namespace AiNewsFeed.Models;



public enum FeedSourceType
{
    Rss,
    NewsApi,
    RedditJson,
    HackerNews,
    ArXiv,
    GoogleNews,
    YouTube
}

public enum SourcePriority
{
    Low = 1,      // Aggregators, newsletters
    Medium = 2,   // General tech news
    High = 3,     // Research labs, official blogs
    Critical = 4  // Official model announcements
}

public class FeedSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FeedUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public FeedSourceType Type { get; set; } = FeedSourceType.Rss;
    public SourcePriority Priority { get; set; } = SourcePriority.Medium;

    // NEW: Only ingest articles matching these keywords (comma-separated). Empty = accept all.
    public string? RequiredKeywords { get; set; }

    // NEW: Always include articles from this source regardless of keywords (for official blogs)
    public bool AlwaysInclude { get; set; }

    public DateTime? LastFetchedAt { get; set; }
    public string? LastFetchStatus { get; set; }
    public string? LastErrorMessage { get; set; }
    public int ConsecutiveFailures { get; set; }
    public int FetchIntervalMinutes { get; set; } = 60;
}