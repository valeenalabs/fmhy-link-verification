using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WikiLinkVerification.Services;
using WikiLinkVerification.ViewModels;
using WikiLinkVerification.Views;

namespace WikiLinkVerification;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Create the service collection
        var services = new ServiceCollection();
        
        // Register the services
        services.AddSingleton<UrlVerificationService, UrlVerificationService>();
        services.AddSingleton<MainWindowViewModel>();
        
        // Build the service provider
        var serviceProvider  = services.BuildServiceProvider();
        
        var mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        var mainWindow = new MainWindow()
        {
            DataContext = mainWindowViewModel
        };
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = mainWindow;
        
        base.OnFrameworkInitializationCompleted();
    }
}