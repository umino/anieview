namespace AnieView.Application.UseCases;

public class CalculateZoomUseCase
{
    /// <summary>
    /// 画像を画面にフィットさせるためのスケール値を計算する
    /// </summary>
    public double CalculateFitScale(
        double imgWidthDip, double imgHeightDip, 
        double workAreaWidth, double workAreaHeight, 
        int rotationAngle)
    {
        double effectiveWidth = imgWidthDip;
        double effectiveHeight = imgHeightDip;

        // 回転が90度、270度の場合は幅と高さを入れ替えて計算
        if (rotationAngle % 180 != 0)
        {
            (effectiveWidth, effectiveHeight) = (effectiveHeight, effectiveWidth);
        }

        double scaleX = workAreaWidth / Math.Max(1.0, effectiveWidth);
        double scaleY = workAreaHeight / Math.Max(1.0, effectiveHeight);

        return Math.Min(scaleX, scaleY);
    }

    /// <summary>
    /// 画面の 1/n に収まるズーム率 (%) を計算する
    /// </summary>
    public double CalculateFractionalZoom(
        int pixelDimension, double dpi, 
        double screenDimensionDip, int n)
    {
        // 実ピクセルをデバイス独立単位 (DIP) に変換
        double originalDimensionDip = pixelDimension * 96.0 / dpi;
        
        // ターゲットサイズ (画面の 1/n)
        double targetDimensionDip = screenDimensionDip / n;

        // ズーム率を計算
        return (targetDimensionDip / originalDimensionDip) * 100.0;
    }
}
