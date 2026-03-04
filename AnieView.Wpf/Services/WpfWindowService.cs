using AnieView.Core.Interfaces;
using AnieView.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AnieView.Wpf.Services;

public class WpfWindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    public WpfWindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void OpenDuplicateWindow(string filePath, double zoomPercentage, int rotationAngle)
    {
        var newWindow = _serviceProvider.GetRequiredService<Views.MainWindow>();
        if (newWindow.DataContext is MainViewModel vm)
        {
            // まず画像をロード
            _ = vm.LoadInitialImage(filePath);
            
            // パラメータを復元
            vm.ZoomPercentage = zoomPercentage;
            vm.RotationAngle = rotationAngle;
        }
        newWindow.Show();
    }
}
