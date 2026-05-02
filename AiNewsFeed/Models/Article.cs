using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiNewsFeed.Models;

public class Article
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? Content { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Source { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Author { get; set; }

    [Required]
    public DateTime PublishedAt { get; set; }

    public DateTime FetchedAt { get; set; }

    public bool IsRead { get; set; } = false;

    public bool IsDeleted { get; set; } = false;
}