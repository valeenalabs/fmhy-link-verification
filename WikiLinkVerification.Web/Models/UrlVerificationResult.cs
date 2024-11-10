namespace WikiLinkVerification.Web.Models;

public class UrlVerificationResult
{
    
    public string? ContentType { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? FinalUrl { get; set; } = string.Empty;

    public int StatusCode { get; set; }
    public bool IsAccessible { get; set; }
    public bool UseHttps { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Language { get; set; } = string.Empty;
    public string? Keywords { get; set; } = string.Empty; 
}