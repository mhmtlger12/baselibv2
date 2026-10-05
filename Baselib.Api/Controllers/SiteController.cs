using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace Baselib.Api.Controllers;

[ApiController, Route("api/site"), AllowAnonymous]
public sealed class SiteController(ISiteContentService content) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    { var result = await content.GetAsync(ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("scores")]
    public async Task<IActionResult> Scores(string category, int programId, string? period, string? institution, string? city, CancellationToken ct, int page = 1)
    { var result = await content.ScoresAsync(category, programId, period, institution, city, page, ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("comments")]
    public async Task<IActionResult> Comments(string page, CancellationToken ct)
    { var result = await content.CommentsAsync(page, ct); return StatusCode(result.StatusCode, result); }
    [HttpPost("contact"), EnableRateLimiting(RateLimitPolicies.SiteSubmission)]
    public async Task<IActionResult> Contact(SendContactDto input, CancellationToken ct)
    { var result = await content.SendContactAsync(input, ct); return StatusCode(result.StatusCode, result); }
    [HttpPost("comments"), EnableRateLimiting(RateLimitPolicies.SiteSubmission)]
    public async Task<IActionResult> Comment(SendCommentDto input, CancellationToken ct)
    { var result = await content.SendCommentAsync(input, ct); return StatusCode(result.StatusCode, result); }
}
