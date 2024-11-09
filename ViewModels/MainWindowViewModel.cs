using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Flurl;
using Flurl.Http;
using ReactiveUI;
using WikiLinkVerification.Services;

namespace WikiLinkVerification.ViewModels;
public partial class MainWindowViewModel : ReactiveObject
{
    private string _urlInput = "https://fmhy.net";
    private string _validationStatus;
    private Dictionary<Url, UrlVerificationResult> _urlVerificationResults =
        new Dictionary<Url, UrlVerificationResult>();

    public string UrlInput
    {
        get => _urlInput;
        set => this.RaiseAndSetIfChanged(ref _urlInput, value);
    }

    public string ValidationStatus
    {
        get => _validationStatus;
        set => this.RaiseAndSetIfChanged(ref _validationStatus, value);
    }
    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }
    public MainWindowViewModel(UrlVerificationService urlVerificationService, string validationStatus)
    {
        _validationStatus = validationStatus;
        List<Url> urls = new List<Url>();

        this.WhenAnyValue(x => x.UrlInput)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(url => ValidateUrl(url));
        
        var canVerify = this.WhenAnyValue(
            x => x.UrlInput,
            url => !string.IsNullOrEmpty(url));

        VerifyCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                foreach (var url in urls)
                {
                    var result = await urlVerificationService.VerifyUrlAsync(url);
                    _urlVerificationResults.Add(url, result);
                }
            });
    }

    private void ValidateUrl(string url)
    {
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            ValidationStatus = "URL is well formed";
        }
        else
        {
            ValidationStatus = "URL is not well formed";
        }
    }
}
