 using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AnieView.Application.UseCases;
using AnieView.Core.Interfaces;
using AnieView.Infrastructure.Services;
using AnieView.Wpf.Services;
using AnieView.Wpf.ViewModels;
using AnieView.Wpf.Views;

namespace AnieView.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Core Services
                services.AddSingleton<INavigationService, NavigationService>();

                // Infrastructure Services
                services.AddSingleton<IImageService, WpfImageService>();

                // Window Services
                services.AddSingleton<IWindowService, WpfWindowService>();
                services.AddSingleton<IScreenInfoService, WpfScreenInfoService>();

                // Settings
                services.AddSingleton<ISettingsService, JsonSettingsService>();

                // Use Cases
                services.AddTransient<LoadImageUseCase>();
                services.AddTransient<NavigateImageUseCase>();
                services.AddTransient<CalculateZoomUseCase>();
                services.AddTransient<CreateEmptyImageUseCase>();
                services.AddTransient<CalculateWindowPlacementUseCase>();

                // ViewModels
                services.AddTransient<MainViewModel>();

                // Views
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        var viewModel = (MainViewModel)mainWindow.DataContext;

        // 設定をロードし、NavigationService に反映
        var settingsService = _host.Services.GetRequiredService<ISettingsService>();
        settingsService.Load();
        var navigationService = _host.Services.GetRequiredService<INavigationService>();
        navigationService.SortOrder = settingsService.SortOrder;

        // コマンドライン引数（画像パス）のチェック
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            var fullPath = System.IO.Path.GetFullPath(args[1]);
            _ = viewModel.LoadInitialImage(fullPath);
        }
        else
        {
            viewModel.EmptyImageCommand.Execute(null);
        }

        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
