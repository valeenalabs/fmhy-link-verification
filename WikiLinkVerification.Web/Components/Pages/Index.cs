using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WikiLinkVerification.Web.Models;
using WikiLinkVerification.Web.Services.Interfaces;

namespace WikiLinkVerification.Web.Components.Pages;

public partial class Index
{
    private HashSet<UrlVerificationResult> _expandedResults = new();
    private bool _isVerifying;
    private IJSObjectReference? _jsModule;
    private ElementReference _textAreaRef;

    private string _urlInput = string.Empty;
    private List<UrlVerificationResult> _verificationResults = new();

    [Inject] public required IUrlVerificationService UrlVerificationService { get; set; }

    [Inject] public required IJSRuntime JS { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./js/textarea.js");
        }
    }

    private async Task OnTextAreaInput()
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("autoResizeTextArea", _textAreaRef);
        }
    }

    private void ToggleResult(UrlVerificationResult result)
    {
        if (!_expandedResults.Add(result))
        {
            _expandedResults.Remove(result);
        }
    }

    private async Task VerifyUrls()
    {
        if (string.IsNullOrWhiteSpace(_urlInput)) return;

        _isVerifying = true;
        _verificationResults.Clear();
        _expandedResults.Clear();

        var urls = _urlInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(u => u.Trim())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .ToList();

        foreach (var url in urls)
        {
            try
            {
                var result = await UrlVerificationService.VerifyUrlAsync(url);
                _verificationResults.Add(result);
            }
            catch (Exception ex)
            {
                var errorResult = new UrlVerificationResult
                {
                    FinalUrl = url,
                    IsAccessible = false,
                    ErrorMessage = ex.Message
                };
                _verificationResults.Add(errorResult);
            }
        }

        _isVerifying = false;
    }

    private void ClearAll()
    {
        _urlInput = string.Empty;
        _verificationResults.Clear();
        _expandedResults.Clear();
    }
}