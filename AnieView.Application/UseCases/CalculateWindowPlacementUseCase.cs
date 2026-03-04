using System;

namespace AnieView.Application.UseCases;

public class CalculateWindowPlacementUseCase
{
    /// <summary>
    /// ウィンドウが画面（作業領域）からはみ出さないように調整された座標を計算する。
    /// </summary>
    public (double Left, double Top) ClampToScreen(
        double currentLeft, double currentTop, 
        double windowWidth, double windowHeight, 
        double workAreaWidth, double workAreaHeight)
    {
        double newLeft = currentLeft;
        double newTop = currentTop;

        // 右端がはみ出している場合は左にずらす
        if (newLeft + windowWidth > workAreaWidth)
        {
            newLeft = workAreaWidth - windowWidth;
        }

        // 下端がはみ出している場合は上にずらす
        if (newTop + windowHeight > workAreaHeight)
        {
            newTop = workAreaHeight - windowHeight;
        }

        // 左端・上端がはみ出している（マイナス）場合は 0 に固定
        newLeft = Math.Max(0, newLeft);
        newTop = Math.Max(0, newTop);

        return (newLeft, newTop);
    }

    /// <summary>
    /// ウィンドウ画面中央に配置するための座標を計算する。
    /// </summary>
    public (double Left, double Top) GetCenteredPosition(
        double windowWidth, double windowHeight, 
        double workAreaWidth, double workAreaHeight)
    {
        double left = (workAreaWidth - windowWidth) / 2.0;
        double top = (workAreaHeight - windowHeight) / 2.0;

        return (Math.Max(0, left), Math.Max(0, top));
    }
}
