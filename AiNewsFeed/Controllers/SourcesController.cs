using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiNewsFeed.Data;
using AiNewsFeed.DTOs;

namespace AiNewsFeed.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SourcesController : ControllerBase
{
    private readonly AppDbContext _db;

    public SourcesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/sources
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeedSourceResponseDto>>> GetSources()
    {
        var sources = await _db.FeedSources
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new FeedSourceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                FeedUrl = s.FeedUrl,
                IsActive = s.IsActive,
                LastFetchedAt = s.LastFetchedAt,
                LastFetchStatus = s.LastFetchStatus,
                LastErrorMessage = s.LastErrorMessage,
                ConsecutiveFailures = s.ConsecutiveFailures
            })
            .ToListAsync();

        return Ok(sources);
    }
}