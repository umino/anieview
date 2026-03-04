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

                // Use Cases
                services.AddTransient<LoadImageUseCase>();
                services.AddTransient<NavigateImageUseCase>();
                services.AddTransient<CalculateZoomUseCase>();

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
