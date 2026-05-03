using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiNewsFeed.Models;

public class Article
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

    // NEW: Extracted model/company mentions for fast filtering
    public string? MentionedModels { get; set; }  // JSON array: ["kimi-k2.6", "claude-opus-4.6"]
    public string? MentionedCompanies { get; set; } // JSON array: ["moonshot", "anthropic", "openai"]

    // NEW: For PostgreSQL full-text search (or keep as regular string for SQLite)
    public string? SearchVector { get; set; }

    // NEW: Content hash for deduplication
    public string? ContentHash { get; set; }

    // NEW: Original source type for provenance
    public FeedSourceType SourceType { get; set; }
}