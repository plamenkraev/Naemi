using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using LumexUI.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NaemZZ.Core.Interfaces;
using NaemZZ.Desktop.Logging;
using NaemZZ.Desktop.Services;
using NaemZZ.Infrastructure;
using NaemZZ.Infrastructure.Data;
using R3;
using Tjt.Modularity;

namespace NaemZZ.Desktop;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                _host!.Services.GetRequiredService<ILogger<App>>().LogError(ex, "Unhandled exception");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            _host!.Services.GetRequiredService<ILogger<App>>().LogError(e.Exception, "Unhandled exception");
            e.SetObserved();
        };

        WpfProviderInitializer.SetDefaultObservableSystem(ex => Trace.WriteLine($"R3 UnhandledException:{ex}"));


        base.OnStartup(e);

        // Bulgarian locale by default, without mutating shared NumberFormatInfo process-wide;
        // call sites that need the legacy "dd/MM/yyyy" date format apply it explicitly.
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("bg-BG");
        Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;

        RegisterGlobalExceptionHandlers();

        var appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NaemZZ");
        Directory.CreateDirectory(appDataDirectory);
        var databasePath = Path.Combine(appDataDirectory, "dbZZ.db");
        var logDirectory = Path.Combine(appDataDirectory, "logs");

        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddProvider(new FileLoggerProvider(logDirectory));
            })
            .ConfigureServices(services =>
            {
                services.AddWpfBlazorWebView();
#if DEBUG
                services.AddBlazorWebViewDeveloperTools();
#endif
                services.AddLumexServices();

                services.AddNaemZZInfrastructure(databasePath);
                services.AddSingleton<IContractDocumentService, ContractDocumentService>();

                services.AddTransient<MainWindow>();

                var application = ModuleEngineFactory.Create<HostModule>(services);
            })
            .Build();

        await _host.StartAsync();

        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Договори за наем starting up. Database: {DatabasePath}", databasePath);

        var dbFactory = _host.Services.GetRequiredService<IDbContextFactory<NaemZZDbContext>>();
        var initResult = await DatabaseInitializer.InitializeAsync(dbFactory, databasePath, logger);

        if (!initResult.Success)
        {
            logger.LogError("Database initialization failed: {Error}", initResult.ErrorMessage);
            MessageBox.Show(
                $"Грешка при зареждане на базата данни:\n{initResult.ErrorMessage}\n\nПриложението ще бъде затворено.",
                "Договори за наем", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    private void RegisterGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += (_, args) =>
        {
            LogUnhandledException(args.Exception, "DispatcherUnhandledException");
            MessageBox.Show(
                $"Възникна неочаквана грешка:\n{args.Exception.Message}\n\nПодробности са записани в лог файла.",
                "Договори за наем", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            LogUnhandledException(args.ExceptionObject as Exception, "AppDomain.UnhandledException");

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");
            args.SetObserved();
        };
    }

    private void LogUnhandledException(Exception? exception, string source)
    {
        var logger = _host?.Services.GetService<ILogger<App>>();
        logger?.LogError(exception, "Unhandled exception from {Source}", source);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}