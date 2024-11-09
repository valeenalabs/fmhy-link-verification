using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AngleSharp;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace WikiLinkVerification.Services;

public class UrlVerificationService
{
    public async Task<UrlVerificationResult> VerifyUrlAsync(Url url)
    {
        var result = new UrlVerificationResult();

        var client = new FlurlClient();
        client.Settings.AllowedHttpStatusRange = "*";
       client.Settings.Timeout = TimeSpan.FromSeconds(5);
        client.Settings.Redirects.Enabled = true;
        client.Settings.Redirects.MaxAutoRedirects = 5;
        client.Settings.Redirects.ForwardHeaders = true;
        client.Settings.Redirects.AllowSecureToInsecure = false;

        var browsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        
        try
        {
            var response = client.Request(url).GetAsync().Result;
            result.StatusCode = response.StatusCode;
            result.IsAccessible = response.StatusCode >= 200 & response.StatusCode <= 299;
            result.HasValidSSlCertificate = response.ResponseMessage.RequestMessage?.RequestUri?.Scheme == "https";
            result.ContentType = response.ResponseMessage.Content.Headers.ContentType?.MediaType;
            if (result.IsAccessible)
            {
                var data = await browsingContext.OpenAsync(url);
                result.Title = data.Title;
                result.Description = data.QuerySelector("meta[name='description']")?.GetAttribute("content");
                result.Keywords = data.QuerySelector("meta[name='keywords']")?.GetAttribute("content");
                result.Language = data.QuerySelector("meta[name='language']")?.GetAttribute("content");
                
            }
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return result;
    }
}