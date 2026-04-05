using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;
using AnieView.Application.UseCases;

namespace AnieView.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IWindowService _windowService;
    private readonly LoadImageUseCase _loadImageUseCase;
    private readonly NavigateImageUseCase _navigateImageUseCase;
    private readonly CalculateZoomUseCase _calculateZoomUseCase;
    private readonly IScreenInfoService _screenInfoService;
    private readonly CreateEmptyImageUseCase _createEmptyImageUseCase;
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScaleX))]
    [NotifyPropertyChangedFor(nameof(ScaleY))]
    private ImageData? _displayImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScaleX))]
    [NotifyPropertyChangedFor(nameof(ScaleY))]
    private double _zoomPercentage = 100.0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScaleX))]
    [NotifyPropertyChangedFor(nameof(ScaleY))]
    private bool _isMaximizedToScreen = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScaleX))]
    [NotifyPropertyChangedFor(nameof(ScaleY))]
    private int _rotationAngle = 0;

    [ObservableProperty]
    private string _windowTitle = "AnieView";

    public double ScaleX
    {
        get
        {
            if (IsMaximizedToScreen && DisplayImage != null)
            {
                return _calculateZoomUseCase.CalculateFitScale(
                    DisplayImage.Width * 96.0 / DisplayImage.DpiX,
                    DisplayImage.Height * 96.0 / DisplayImage.DpiY,
                    _screenInfoService.WorkAreaWidth,
                    _screenInfoService.WorkAreaHeight,
                    RotationAngle);
            }

            return ZoomPercentage / 100.0;
        }
    }

    public double ScaleY => ScaleX;

    private ImageFile? _currentImageFile;

    public MainViewModel(
        IWindowService windowService,
        LoadImageUseCase loadImageUseCase,
        NavigateImageUseCase navigateImageUseCase,
        CalculateZoomUseCase calculateZoomUseCase,
        IScreenInfoService screenInfoService,
        CreateEmptyImageUseCase createEmptyImageUseCase,
        ISettingsService settingsService,
        INavigationService navigationService)
    {
        _windowService = windowService;
        _loadImageUseCase = loadImageUseCase;
        _navigateImageUseCase = navigateImageUseCase;
        _calculateZoomUseCase = calculateZoomUseCase;
        _screenInfoService = screenInfoService;
        _createEmptyImageUseCase = createEmptyImageUseCase;
        _settingsService = settingsService;
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
        
        var imageData = await _loadImageUseCase.ExecuteAsync(_currentImageFile.FilePath);
        DisplayImage = imageData;

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
            ? _navigateImageUseCase.GetNextPath(_currentImageFile.FilePath)
            : _navigateImageUseCase.GetPreviousPath(_currentImageFile.FilePath);

        if (nextPath != null)
        {
            var nextImage = new ImageFile(nextPath)
            {
                ZoomPercentage = ZoomPercentage,
                RotationAngle = RotationAngle
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
    private void ToggleFitToScreen()
    {
        IsMaximizedToScreen = !IsMaximizedToScreen;
    }

    [RelayCommand]
    private void ResizeToFraction(string parameter)
    {
        if (DisplayImage == null || string.IsNullOrEmpty(parameter)) return;

        char type = parameter[0]; // 'W' or 'H'
        if (!int.TryParse(parameter.Substring(1), out int n)) return;

        double screenDim = type == 'W' ? _screenInfoService.WorkAreaWidth : _screenInfoService.WorkAreaHeight;
        int pixelDim = type == 'W' ? DisplayImage.Width : DisplayImage.Height;
        double dpi = type == 'W' ? DisplayImage.DpiX : DisplayImage.DpiY;

        ZoomPercentage = _calculateZoomUseCase.CalculateFractionalZoom(pixelDim, dpi, screenDim, n);
    }

    [RelayCommand]
    private void Duplicate()
    {
        if (_currentImageFile == null) return;
        _windowService.OpenDuplicateWindow(_currentImageFile.FilePath, ZoomPercentage, RotationAngle);
    }

    [RelayCommand]
    private void BringAllToFront()
    {
        _windowService.BringAllWindowsToForeground();
    }

    [RelayCommand]
    private void ToggleSortOrder()
    {
        // ソート順を切り替え
        var newOrder = _settingsService.SortOrder == SortOrder.FileName 
            ? SortOrder.LastModified 
            : SortOrder.FileName;

        _settingsService.SortOrder = newOrder;
        _navigationService.SortOrder = newOrder;
        _settingsService.Save();

        // ユーザーへのフィードバック表示
        var orderName = newOrder == SortOrder.FileName ? "ファイル名順" : "更新日時順";
        WindowTitle = $"AnieView - Sort: {orderName}";
    }

    [RelayCommand]
    private async Task EmptyImage()
    {
        var imageData = await _createEmptyImageUseCase.ExecuteAsync();
        DisplayImage = imageData;
        _currentImageFile = null;

        // 表示状態をリセット
        ZoomPercentage = 100.0;
        RotationAngle = 0;
        WindowTitle = $"AnieView - Empty Image ({imageData.Width}x{imageData.Height})";
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
