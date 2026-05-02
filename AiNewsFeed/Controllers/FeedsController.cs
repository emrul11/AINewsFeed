using Microsoft.AspNetCore.Mvc;
using AiNewsFeed.DTOs;
using AiNewsFeed.Services;

namespace AiNewsFeed.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedsController : ControllerBase
{
    private readonly IFeedFetcherService _feedService;

    public FeedsController(IFeedFetcherService feedService)
    {
        _feedService = feedService;
    }

    // POST /api/feeds/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult<FeedRefreshResponseDto>> Refresh()
    {
        var result = await _feedService.RefreshFeedsAsync();
        return Ok(result);
    }
}