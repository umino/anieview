using AnieView.Application.UseCases;

namespace AnieView.Tests.Application;

public class CalculateZoomUseCaseTests
{
    private readonly CalculateZoomUseCase _useCase = new();

    [Fact]
    public void CalculateFitScale_LandscapeImage_FitsWidth()
    {
        // 横長画像 (1000x500) を 800x600 の画面にフィット → 幅で制限される
        var scale = _useCase.CalculateFitScale(1000, 500, 800, 600, 0);

        Assert.Equal(0.8, scale); // 800 / 1000
    }

    [Fact]
    public void CalculateFitScale_PortraitImage_FitsHeight()
    {
        // 縦長画像 (500x1000) を 800x600 の画面にフィット → 高さで制限される
        var scale = _useCase.CalculateFitScale(500, 1000, 800, 600, 0);

        Assert.Equal(0.6, scale); // 600 / 1000
    }

    [Fact]
    public void CalculateFitScale_Rotated90_SwapsWidthHeight()
    {
        // 横長画像 (1000x500) を90度回転 → 実質 500x1000 として計算される
        var scaleRotated = _useCase.CalculateFitScale(1000, 500, 800, 600, 90);
        var scaleTransposed = _useCase.CalculateFitScale(500, 1000, 800, 600, 0);

        Assert.Equal(scaleTransposed, scaleRotated);
    }

    [Fact]
    public void CalculateFitScale_Rotated180_NoSwap()
    {
        // 180度回転は幅と高さが入れ替わらない
        var scaleNormal = _useCase.CalculateFitScale(1000, 500, 800, 600, 0);
        var scaleRotated = _useCase.CalculateFitScale(1000, 500, 800, 600, 180);

        Assert.Equal(scaleNormal, scaleRotated);
    }

    [Fact]
    public void CalculateFractionalZoom_ScreenHalf()
    {
        // 1920px幅画面の1/2に960px画像 (96dpi) を収める → 100%
        var zoom = _useCase.CalculateFractionalZoom(960, 96.0, 1920, 2);

        Assert.Equal(100.0, zoom);
    }

    [Fact]
    public void CalculateFractionalZoom_HighDpi()
    {
        // 192dpi (2x) の 960px 画像 → DIP width は 480
        // 画面幅 1920 / 2 = 960 ターゲット → 960 / 480 * 100 = 200%
        var zoom = _useCase.CalculateFractionalZoom(960, 192.0, 1920, 2);

        Assert.Equal(200.0, zoom);
    }
}
