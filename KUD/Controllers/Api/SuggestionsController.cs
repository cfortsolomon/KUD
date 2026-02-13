using Microsoft.AspNetCore.Mvc;
using KUD.DTOs;
using KUD.Services;

namespace KUD.Controllers.Api;


[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class SuggestionsController : ControllerBase
{
    private readonly ISuggestionService _suggestions;

    public SuggestionsController(ISuggestionService suggestions) => _suggestions = suggestions;

    /// <summary>Submit an anonymous suggestion. No identity stored or required.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SuggestionResponse), 201)]
    [ProducesResponseType(400)]
    [Route("submit")]
    public async Task<IActionResult> Submit([FromBody] CreateSuggestionRequest request)
    {
        var suggestion = await _suggestions.SubmitAsync(request);
        return StatusCode(201, suggestion);
    }

    /// <summary>Get all suggestions. Visible to everyone. Supports pagination and category filter.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SuggestionResponse>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null)
    {
        var result = await _suggestions.GetAllAsync(page, pageSize, category);
        return Ok(result);
    }

    /// <summary>Get a single suggestion by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SuggestionResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var suggestion = await _suggestions.GetByIdAsync(id);
        if (suggestion is null)
            return NotFound(new { error = "Suggestion not found." });

        return Ok(suggestion);
    }
}
