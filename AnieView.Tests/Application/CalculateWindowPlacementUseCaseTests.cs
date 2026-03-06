using AnieView.Application.UseCases;

namespace AnieView.Tests.Application;

public class CalculateWindowPlacementUseCaseTests
{
    private readonly CalculateWindowPlacementUseCase _useCase = new();

    [Fact]
    public void ClampToScreen_WindowInsideScreen_NoChange()
    {
        var (left, top) = _useCase.ClampToScreen(100, 100, 400, 300, 1920, 1080);

        Assert.Equal(100, left);
        Assert.Equal(100, top);
    }

    [Fact]
    public void ClampToScreen_RightOverflow_ClampsLeft()
    {
        // ウィンドウの右端がはみ出し: 1800 + 400 = 2200 > 1920
        var (left, top) = _useCase.ClampToScreen(1800, 100, 400, 300, 1920, 1080);

        Assert.Equal(1520, left); // 1920 - 400
        Assert.Equal(100, top);
    }

    [Fact]
    public void ClampToScreen_BottomOverflow_ClampsTop()
    {
        var (left, top) = _useCase.ClampToScreen(100, 900, 400, 300, 1920, 1080);

        Assert.Equal(100, left);
        Assert.Equal(780, top); // 1080 - 300
    }

    [Fact]
    public void ClampToScreen_NegativePosition_ClampsToZero()
    {
        var (left, top) = _useCase.ClampToScreen(-50, -30, 400, 300, 1920, 1080);

        Assert.Equal(0, left);
        Assert.Equal(0, top);
    }

    [Fact]
    public void ClampToScreen_WindowLargerThanScreen_ClampsToZero()
    {
        // ウィンドウが画面より大きい場合 → 0,0 に固定
        var (left, top) = _useCase.ClampToScreen(100, 100, 2000, 1200, 1920, 1080);

        Assert.Equal(0, left);
        Assert.Equal(0, top);
    }

    [Fact]
    public void GetCenteredPosition_CentersCorrectly()
    {
        var (left, top) = _useCase.GetCenteredPosition(400, 300, 1920, 1080);

        Assert.Equal(760, left);   // (1920 - 400) / 2
        Assert.Equal(390, top);    // (1080 - 300) / 2
    }

    [Fact]
    public void GetCenteredPosition_WindowLargerThanScreen_ClampsToZero()
    {
        var (left, top) = _useCase.GetCenteredPosition(2000, 1200, 1920, 1080);

        Assert.Equal(0, left);    // (1920 - 2000) / 2 = -40 → 0
        Assert.Equal(0, top);     // (1080 - 1200) / 2 = -60 → 0
    }
}
