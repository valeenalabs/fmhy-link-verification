using Flurl;

namespace WikiLinkVerification.Services;

public class UrlVerificationResult
{
    public int StatusCode { get; set; }
    public bool IsAccessible { get; set; }
    public bool HasValidSSlCertificate { get; set; }
    public string? ContentType { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Title { get; set; } = string.Empty;
    public Url FinalUrl { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? Language { get; set; } = string.Empty;
    public string? Keywords { get; set; } = string.Empty;
    public bool IsMobileFriendly  { get; set; }
}