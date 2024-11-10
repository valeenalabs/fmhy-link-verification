using AngleSharp;
using Flurl.Http;
using WikiLinkVerification.Web.Models;
using WikiLinkVerification.Web.Services.Interfaces;

namespace WikiLinkVerification.Web.Services;

public class UrlVerificationService : IUrlVerificationService
{
    private readonly IBrowsingContext _browsingContext;

    public UrlVerificationService(IBrowsingContext browsingContext)
    {
        _browsingContext = browsingContext;
    }

    public async Task<UrlVerificationResult> VerifyUrlAsync(string url)
    {
        var result = new UrlVerificationResult();

        try
        {
            var response = await SendRequestAsync(url);
            
            result.StatusCode = response.StatusCode;
            result.IsAccessible = response.StatusCode is >= 200 and <= 299;
            result.FinalUrl = response.ResponseMessage.RequestMessage?.RequestUri?.ToString() ?? url;
            
            
            // If successful, get content from AngleSharp
            if (result.IsAccessible)
            {
                var html = await response.GetStringAsync();
                
                // Parse with AngleSharp
                
                var document = await _browsingContext.OpenAsync(r => r.Content(html));
                result.UseHttps = response.ResponseMessage.RequestMessage?.RequestUri?.Scheme == "https";
                result.ContentType = document.ContentType;
                result.Description = document.QuerySelector("meta[name='description']")?.GetAttribute("content");
                result.Title = document.Title;
                result.Language = document.QuerySelector("html")?.GetAttribute("lang") 
                    ?? document.QuerySelector("meta[name='language']")?.GetAttribute("content");

                result.Keywords = document.QuerySelector("meta[name='keywords']")?.GetAttribute("content");

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return result;
    }

    private async Task<IFlurlResponse> SendRequestAsync(string url)
    {
        return await url
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithAutoRedirect(true)
            .WithSettings(settings =>
            {
                settings.Redirects.MaxAutoRedirects = 3;
                settings.Redirects.AllowSecureToInsecure = false;
                settings.Redirects.ForwardHeaders = true;
            })
            .AllowAnyHttpStatus()
            .GetAsync();
    }
}