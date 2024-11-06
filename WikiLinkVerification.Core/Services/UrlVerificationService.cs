using System.Text.RegularExpressions;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using WikiLinkVerification.Core.Models;

namespace WikiLinkVerification.Core.Services;

public class UrlVerificationService
{
    private readonly ILogger<UrlVerificationService> _logger;

    public UrlVerificationService(ILogger<UrlVerificationService> logger)
    {
        _logger = logger;
    }

    public bool IsValidUrlFormat(string url)
    {
        // Basic URL format validation
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
          && (uriResult.Scheme == Uri.UriSchemeHttp  || uriResult.Scheme == Uri.UriSchemeHttps);  
    }
    
    public async Task<UrlVerificationResult> VerifyUrlAsync(string url)
    {
        var result = new UrlVerificationResult();

        try
        {
            if (!IsValidUrlFormat(url))
            {
                // First validate URL format
                result.ErrorMessage = "Invalid URL format";
                return result;
            }

            // Make the request
            var response = await url
                .WithTimeout(TimeSpan.FromSeconds(10))
                .WithHeader("UserAgent", "WikiLinkVerification/1.0")
                .AllowAnyHttpStatus()
                .GetAsync();

            // Basic accessibility check
            result.StatusCode = response.StatusCode;
            result.IsAccessible = response.StatusCode >= 200 && response.StatusCode <= 300;

            // Get final URL (after redirects)
            result.FinalUrl = response.ResponseMessage.RequestMessage?.RequestUri?.ToString();

            // SSL Check
            result.HasValidSsl = response.ResponseMessage.RequestMessage?.RequestUri?.Scheme == "https";

            // Get content type
            result.ContentType = response.ResponseMessage.Content.Headers.ContentType?.MediaType;

            // If accessible, try to get title and description
            if (result.IsAccessible)
            {
                var html = await response.GetStringAsync();
                (result.Title, result.Description) = ExtractMetadata(html);
            }

        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "Error verifying URL: {Url}", url);
            result.ErrorMessage = ex.Message;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error verifying URL: {Url}", url);
            result.ErrorMessage = e.Message;
        }

        return result;
    }

    private (string? title, string? description) ExtractMetadata(string html)
    {
        try
        {
            // For MVP, we can use simple Regex patterns
            // Later we can use a proper HTML parser library
            
            var titleMatch = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
            
            var descMatch = Regex.Match(html, 
                @"<meta\s+name=""description""\s+content=""([^""]*)""\s*/*>",
                RegexOptions.IgnoreCase);
            
            return (titleMatch.Success ? titleMatch.Groups[1].Value : null, descMatch.Success ? descMatch.Groups[1].Value : null);
        }
        catch
        {
            return (null, null);
        }
    }
}