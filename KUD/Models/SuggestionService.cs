using Microsoft.EntityFrameworkCore;
using KUD.Data;
using KUD.Models;
using KUD.DTOs;


namespace KUD.Services;

public interface ISuggestionService
{
    Task<SuggestionResponse> SubmitAsync(CreateSuggestionRequest request);
    Task<PagedResult<SuggestionResponse>> GetAllAsync(int page, int pageSize, string? category);
    Task<SuggestionResponse?> GetByIdAsync(int id);
}

public class SuggestionService : ISuggestionService
{
    private readonly ApplicationDbContext _context;

    public SuggestionService(ApplicationDbContext context) => _context = context;

    public async Task<SuggestionResponse> SubmitAsync(CreateSuggestionRequest request)
    {
        var suggestion = new Suggestion
        {
            Message = request.Message.Trim(),
            Category = request.Category.Trim()
            // No worker ID, no name, nothing — pure message only
        };

        _context.Suggestions.Add(suggestion);
        await _context.SaveChangesAsync();

        return MapToResponse(suggestion);
    }

    public async Task<PagedResult<SuggestionResponse>> GetAllAsync(int page, int pageSize, string? category)
    {
        pageSize = Math.Clamp(pageSize, 1, 50);
        page = Math.Max(1, page);

        var query = _context.Suggestions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(s => s.Category.ToLower() == category.ToLower());

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SuggestionResponse
            {
                Id = s.Id,
                Message = s.Message,
                Category = s.Category,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<SuggestionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<SuggestionResponse?> GetByIdAsync(int id)
    {
        return await _context.Suggestions
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SuggestionResponse
            {
                Id = s.Id,
                Message = s.Message,
                Category = s.Category,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    private static SuggestionResponse MapToResponse(Suggestion s) => new()
    {
        Id = s.Id,
        Message = s.Message,
        Category = s.Category,
        CreatedAt = s.CreatedAt
    };
}
