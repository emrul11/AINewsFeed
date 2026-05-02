using System.ComponentModel.DataAnnotations;

namespace AiNewsFeed.Models;

public class FeedSource
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FeedUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastFetchedAt { get; set; }

    [MaxLength(50)]
    public string? LastFetchStatus { get; set; }

    [MaxLength(1000)]
    public string? LastErrorMessage { get; set; }

    public int ConsecutiveFailures { get; set; } = 0;
}