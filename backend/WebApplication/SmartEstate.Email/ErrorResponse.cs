namespace SmartEstate.Email;

public class ErrorResponse
{
    public string Message { get; set; }
    public List<ErrorDetail> Errors { get; set; } = new();
    
    public record ErrorDetail(string Code, string Message, string? Field = null);
}