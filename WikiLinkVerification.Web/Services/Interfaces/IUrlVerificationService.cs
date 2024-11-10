using WikiLinkVerification.Web.Models;

namespace WikiLinkVerification.Web.Services.Interfaces;

public interface IUrlVerificationService
{
    Task<UrlVerificationResult> VerifyUrlAsync(string url);
}