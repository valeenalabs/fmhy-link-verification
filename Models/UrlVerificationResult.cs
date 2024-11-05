namespace WikiLinkVerification.Models;

public class UrlVerificationResult
{
    public bool IsAccessible {get; set;}
    public bool HasValidSsl {get; set;}
    public int StatusCode {get; set;}
    public string? FinalUrl {get; set;}
    public string? Title {get; set;}
    public string? Description {get; set;}
    public string? ContentType {get; set;}
    public string? ErrorMessage {get; set;}
}