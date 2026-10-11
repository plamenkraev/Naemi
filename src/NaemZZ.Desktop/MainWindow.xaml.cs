using Microsoft.AspNetCore.Components.WebView.Wpf;
using System.Windows;

namespace NaemZZ.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        BlazorView.Services = services;
    }

    public async Task PrintContractAsync()
    {
        var webView = BlazorView.WebView;

        if (webView?.CoreWebView2 is null)
            return;

        var settings = webView.CoreWebView2.Environment.CreatePrintSettings();

        settings.ShouldPrintHeaderAndFooter = false;
        settings.ShouldPrintBackgrounds = true;

        await webView.CoreWebView2.PrintAsync(settings);
    }
}
