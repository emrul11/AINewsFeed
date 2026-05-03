using AiNewsFeed.Data;
using AiNewsFeed.DTOs;
using AiNewsFeed.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AiNewsFeed.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ArticlesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/articles?source=&unread=true&search=&model=kimi-k2.6&company=anthropic&priority=high&page=1&pageSize=50
    [HttpGet]
    public async Task<ActionResult<PagedArticleResponseDto>> GetArticles(
        [FromQuery] string? source,
        [FromQuery] bool? unread,
        [FromQuery] string? search,
        [FromQuery] string? model,           // NEW: Filter by mentioned model
        [FromQuery] string? company,         // NEW: Filter by mentioned company
        [FromQuery] SourcePriority? priority, // NEW: Filter by source priority
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Min(100, Math.Max(1, pageSize));

        var query = _db.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(a => a.Source == source);

        if (unread == true)
            query = query.Where(a => !a.IsRead);

        // NEW: Model filter (JSON contains)
        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(a => a.MentionedModels != null && a.MentionedModels.Contains(model));

        // NEW: Company filter
        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(a => a.MentionedCompanies != null && a.MentionedCompanies.Contains(company));

        // NEW: Priority filter (join with FeedSources)
        if (priority.HasValue)
        {
            var highPrioritySources = await _db.FeedSources
                .Where(f => f.Priority >= priority.Value)
                .Select(f => f.Name)
                .ToListAsync();
            query = query.Where(a => highPrioritySources.Contains(a.Source));
        }

        // IMPROVED: Full-text search using SearchVector
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                a.SearchVector != null && a.SearchVector.Contains(term));
        }

        var total = await query.CountAsync();

        var articles = await query
            .OrderByDescending(a => a.PublishedAt)
            .ThenByDescending(a => a.FetchedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                Content = a.Content,
                Url = a.Url,
                Source = a.Source,
                Author = a.Author,
                PublishedAt = a.PublishedAt,
                FetchedAt = a.FetchedAt,
                IsRead = a.IsRead,
                IsDeleted = a.IsDeleted,
                MentionedModels = a.MentionedModels,
                MentionedCompanies = a.MentionedCompanies
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());

        return Ok(new PagedArticleResponseDto
        {
            Items = articles,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    // GET /api/articles/models — List all tracked models with counts
    [HttpGet("models")]
    public async Task<ActionResult<Dictionary<string, int>>> GetModelCounts()
    {
        var articles = await _db.Articles
            .Where(a => !a.IsDeleted && a.MentionedModels != null)
            .ToListAsync();

        var counts = new Dictionary<string, int>();
        foreach (var article in articles)
        {
            var models = JsonSerializer.Deserialize<List<string>>(article.MentionedModels!) ?? new();
            foreach (var model in models)
            {
                counts[model] = counts.GetValueOrDefault(model) + 1;
            }
        }

        return Ok(counts.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    // GET /api/articles/companies — List all tracked companies with counts
    [HttpGet("companies")]
    public async Task<ActionResult<Dictionary<string, int>>> GetCompanyCounts()
    {
        var articles = await _db.Articles
            .Where(a => !a.IsDeleted && a.MentionedCompanies != null)
            .ToListAsync();

        var counts = new Dictionary<string, int>();
        foreach (var article in articles)
        {
            var companies = JsonSerializer.Deserialize<List<string>>(article.MentionedCompanies!) ?? new();
            foreach (var company in companies)
            {
                counts[company] = counts.GetValueOrDefault(company) + 1;
            }
        }

        return Ok(counts.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> UpdateReadStatus(int id, [FromBody] UpdateReadStatusRequestDto dto)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null || article.IsDeleted) return NotFound();

        article.IsRead = dto.IsRead;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<ActionResult<object>> MarkAllRead()
    {
        var count = await _db.Articles
            .Where(a => !a.IsDeleted && !a.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.IsRead, true));

        return Ok(new { markedCount = count });
    }
}