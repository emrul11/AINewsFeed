using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiNewsFeed.Data;
using AiNewsFeed.DTOs;

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

    // GET /api/articles?source=&unread=true&search=&page=1&pageSize=50
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleResponseDto>>> GetArticles(
        [FromQuery] string? source,
        [FromQuery] bool? unread,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Min(100, Math.Max(1, pageSize));

        var query = _db.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(source))
        {
            query = query.Where(a => a.Source == source);
        }

        if (unread == true)
        {
            query = query.Where(a => !a.IsRead);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                (a.Title != null && a.Title.ToLower().Contains(term)) ||
                (a.Summary != null && a.Summary.ToLower().Contains(term)) ||
                (a.Author != null && a.Author.ToLower().Contains(term)));
        }

        var total = await query.CountAsync();
        var articles = await query
            .OrderByDescending(a => a.PublishedAt)
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
                IsDeleted = a.IsDeleted
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(articles);
    }

    // PATCH /api/articles/{id}/read
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> UpdateReadStatus(int id, [FromBody] UpdateReadStatusRequestDto dto)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null || article.IsDeleted)
        {
            return NotFound();
        }

        article.IsRead = dto.IsRead;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // PATCH /api/articles/read-all
    [HttpPatch("read-all")]
    public async Task<ActionResult<object>> MarkAllRead()
    {
        var unreadArticles = await _db.Articles
            .Where(a => !a.IsDeleted && !a.IsRead)
            .ToListAsync();

        foreach (var article in unreadArticles)
        {
            article.IsRead = true;
        }

        await _db.SaveChangesAsync();

        return Ok(new { markedCount = unreadArticles.Count });
    }
}