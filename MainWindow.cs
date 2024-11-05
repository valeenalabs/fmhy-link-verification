using System.Drawing;
using Microsoft.Extensions.Logging;
using Terminal.Gui;
using WikiLinkVerification.Services;

namespace WikiLinkVerification;

public sealed class MainWindow : Window
{
    private FrameView _inputFrameView;
    private FrameView _resultsFrameView;
    
    // Input Labels
    private Label? _inputHelperLabel;
    private TextField? _inputField;
    private Button? _verifyButton;
    
    // Results Labels
    private Label? _statusLabel;
    private Label? _sslLabel;
    private Label? _titleLabel;
    private Label? _descriptionLabel;
    private Label? _keywordsLabel;
    private Label? _languageLabel;
    private Label? _wordCountLabel;
    private Label? _contentTypeLabel;
    private Label? _redirectLabel;
    
    private readonly UrlVerificationService _verificationService;

    private bool _isVerifying;
    
    public MainWindow()
    {
        // Simple logger initialization for now
        var loggerFactory = LoggerFactory.Create(builder => 
            builder.AddConsole());
        var logger = loggerFactory.CreateLogger<UrlVerificationService>();
        
        _verificationService = new UrlVerificationService(logger);
        
        Title = "Wiki Link Verification";

        var menuBar = new MenuBar
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 1,
        };

        // Add menu items
        menuBar.Menus = new MenuBarItem[]
        {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Quit", "", () => Application.RequestStop())
            }),
            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About", "", () =>
                {
                    MessageBox.Query(
                        "About",
                        "Wiki Link Verification Tool\nVersion 1.0",
                        "Ok");
                })
            })
        };
        // Add the menu bar to the window
        Add(menuBar);
        var quitShortcut = new Shortcut(Key.Q.WithCtrl, "Exit", () => Application.RequestStop());
        var focusShortcut = new Shortcut(Key.F.WithCtrl, "Focus", () => _inputField!.SetFocus());
        var verificationShortcut = new Shortcut(Key.E.WithCtrl, "Verification", () => VerifyUrlAsync());
        var statusBar = new StatusBar(){ X = 0, Height = 1, Width = Dim.Fill()};
        statusBar.Add(quitShortcut, focusShortcut, verificationShortcut);
        Add(statusBar);

        _inputFrameView = new FrameView()
        {
            X = 0,
            Y = 2,
            Title = "URL Input",
            Width = Dim.Fill(),
            Height = Dim.Auto(DimAutoStyle.Auto)
        };
        
        InitializeInputLayoutContent();
        Add(_inputFrameView);

        _resultsFrameView = new FrameView()
        {
            Title = "Verification Results",
            X = 0,
            Y = Pos.Bottom(_inputFrameView) + 1,
            Width = Dim.Fill(),
            Height = Dim.Auto(),
           // Visible = false,
        };
        Add(_resultsFrameView);

        InitializeResultsLayoutContent();
    }

    private void InitializeResultsLayoutContent()
    {
        var labels = new[]
        {
            _statusLabel = new Label() { Y = 1, Text = "Status: "},
            _sslLabel = new Label() { Y = Pos.Bottom(_statusLabel), Text = "SSL" },
            _titleLabel = new Label() { Y = Pos.Bottom(_sslLabel), Text = "Title" },
            _descriptionLabel = new Label() { Y = Pos.Bottom(_titleLabel), Text = "Description" },
            _keywordsLabel = new Label() { Y = Pos.Bottom(_descriptionLabel), Text = "Keywords" },
            _languageLabel = new Label() { Y = Pos.Bottom(_keywordsLabel), Text = "Language" },
            _wordCountLabel = new Label() { Y = Pos.Bottom(_languageLabel), Text = "Word Count" },
            _contentTypeLabel = new Label() { Y = Pos.Bottom(_wordCountLabel), Text = "Content" },
            _redirectLabel = new Label() { Y = Pos.Bottom(_contentTypeLabel), Text = "Redirect" }
        };

        foreach (var label in labels)
        {
            label.X = 1;
            _resultsFrameView.Add(label);
        }
    }

    private void InitializeInputLayoutContent()
    {
        _inputHelperLabel = new Label()
        {
            Text = "Enter URL (CTRL + U to focus)",
            Y = 2,
            X = 1,
            Height = 1
        };

        _inputField = new TextField()
        {
            Text = "https://fmhy.net",
            Y = Pos.Bottom(_inputHelperLabel) + 1,
            X = 1,
            Width = Dim.Fill(),
            Height = 1
        };
        _verifyButton = new Button()
        {
            Text = "Verify",
            Y = Pos.Bottom(_inputField) + 2,
            X = 1,
            Height = 1,
        };
        _verifyButton.MouseClick += (o, e) => VerifyUrlAsync();

        _inputFrameView.Add(_inputHelperLabel, _inputField, _verifyButton);
        //_inputFrameView.SetNeedsDisplay();
    }

    private async void VerifyUrlAsync()
    {
        if (_isVerifying) return;

        try
        {
            _isVerifying = true;
            _verifyButton!.Text = "Verifying...";

            var url = _inputField!.Text.ToString();
            var result = await _verificationService.VerifyUrlAsync(url);

            // Update your results labels
            _statusLabel!.Text = $"Status: {(result.IsAccessible ? "Accessible" : "Not Accessible")}";
            _sslLabel!.Text = $"SSL: {(result.HasValidSsl ? "Valid" : "Not Valid")}";
            _titleLabel!.Text = $"Title: {result.Title ?? "Not Found"}";
            _descriptionLabel!.Text = $"Description: {result.Description ?? "Not Found"}";

            if (result.ErrorMessage != null)
                MessageBox.ErrorQuery("Error", result.ErrorMessage, "Ok");
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"An error occured: {ex.Message}", "Ok");
        }
        finally
        {
            _isVerifying = false;
            _verifyButton!.Text = "Verify";
        }
    }
}