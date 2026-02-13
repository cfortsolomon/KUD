using System.ComponentModel.DataAnnotations;

namespace KUD.DTOs;

public class CreateSuggestionRequest
{
    [Required, MinLength(10, ErrorMessage = "Suggestion must be at least 10 characters.")]
    [MaxLength(1000, ErrorMessage = "Suggestion cannot exceed 1000 characters.")]
    public string Message { get; set; } = string.Empty;

    public string Category { get; set; } = "General";
}

public class SuggestionResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
