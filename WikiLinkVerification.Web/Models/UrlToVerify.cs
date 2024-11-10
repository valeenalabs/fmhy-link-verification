namespace WikiLinkVerification.Web.Models;

public class UrlToVerify
{
    // Basic properties
    public bool IsValid { get; set; }
    public string? UrlInput { get; set; }
    public UrlVerificationResult? VerificationResult { get; set; }
    
    // Manual verification properties
    public string? Category { get; set; }
    public string? Quality { get; set; }
    public string? AccessType { get; set; } = "Free";  // Default value
    public bool RequiresRegistration { get; set; }
    public string? Notes { get; set; }
    
    // Social links
    public string? DiscordLink { get; set; }
    public string? TelegramLink { get; set; }
    
    // Approval status
    public ApprovalStatus? ApprovalStatus { get; set; }
}

public enum ApprovalStatus
{
    Approved,
    Rejected,
    NeedsVerification
}