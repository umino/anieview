using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;
using AnieView.Core.Services;
using AnieView.Infrastructure.Services;

namespace AnieView.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IImageService _imageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private BitmapSource? _displayImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScaleX))]
    [NotifyPropertyChangedFor(nameof(ScaleY))]
    private double _zoomPercentage = 100.0;

    [ObservableProperty]
    private int _rotationAngle = 0;

    [ObservableProperty]
    private string _windowTitle = "AnieView";

    public double ScaleX => ZoomPercentage / 100.0;
    public double ScaleY => ZoomPercentage / 100.0;

    private ImageFile? _currentImageFile;

    public MainViewModel() : this(new WpfImageService(), new NavigationService()) { }

    public MainViewModel(IImageService imageService, INavigationService navigationService)
    {
        _imageService = imageService;
        _navigationService = navigationService;
    }

    public async Task LoadInitialImage(string filePath)
    {
        _currentImageFile = new ImageFile(filePath);
        await LoadCurrentImage();
    }

    private async Task LoadCurrentImage()
    {
        if (_currentImageFile == null) return;
        
        var result = await _imageService.LoadImageAsync(_currentImageFile.FilePath);
        DisplayImage = result as BitmapSource;

        if (DisplayImage == null)
        {
            WindowTitle = $"AnieView - Failed to load: {_currentImageFile.FileName}";
            return;
        }

        RotationAngle = _currentImageFile.RotationAngle;
        ZoomPercentage = _currentImageFile.ZoomPercentage;
        WindowTitle = $"AnieView - {_currentImageFile.FileName} ({ZoomPercentage:F0}%)";
    }

    [RelayCommand]
    private void Navigate(string directionStr)
    {
        if (_currentImageFile == null || !int.TryParse(directionStr, out int direction)) return;

        string? nextPath = direction > 0 
            ? _navigationService.GetNextFile(_currentImageFile.FilePath)
            : _navigationService.GetPreviousFile(_currentImageFile.FilePath);

        if (nextPath != null)
        {
            var nextImage = new ImageFile(nextPath)
            {
                ZoomPercentage = _zoomPercentage,
                RotationAngle = _rotationAngle
            };
            _currentImageFile = nextImage;
            _ = LoadCurrentImage();
        }
    }

    [RelayCommand]
    private void ZoomIn() => ZoomPercentage += 10;

    [RelayCommand]
    private void ZoomOut() => ZoomPercentage = Math.Max(10, ZoomPercentage - 10);

    [RelayCommand]
    private void Rotate() => RotationAngle = (RotationAngle + 90) % 360;

    [RelayCommand]
    private void Duplicate()
    {
        if (_currentImageFile == null) return;
        var newWindow = new Views.MainWindow();
        if (newWindow.DataContext is MainViewModel vm)
        {
            vm.LoadInitialImage(_currentImageFile.FilePath).ConfigureAwait(false);
            vm.ZoomPercentage = _zoomPercentage;
            vm.RotationAngle = _rotationAngle;
        }
        newWindow.Show();
    }

    partial void OnZoomPercentageChanged(double value)
    {
        if (_currentImageFile != null) _currentImageFile.ZoomPercentage = value;
        WindowTitle = $"AnieView - {_currentImageFile?.FileName} ({value:F0}%)";
    }

    partial void OnRotationAngleChanged(int value)
    {
        if (_currentImageFile != null) _currentImageFile.RotationAngle = value;
    }
}
