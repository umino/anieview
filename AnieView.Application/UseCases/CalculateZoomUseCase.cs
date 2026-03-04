namespace AnieView.Application.UseCases;

public class CalculateZoomUseCase
{
    /// <summary>
    /// 画像を画面にフィットさせるためのスケール値を計算する
    /// </summary>
    public double CalculateFitScale(
        double imgWidth, double imgHeight, 
        double workAreaWidth, double workAreaHeight, 
        int rotationAngle)
    {
        double effectiveWidth = imgWidth;
        double effectiveHeight = imgHeight;

        // 回転が90度、270度の場合は幅と高さを入れ替えて計算
        if (rotationAngle % 180 != 0)
        {
            (effectiveWidth, effectiveHeight) = (effectiveHeight, effectiveWidth);
        }

        double scaleX = workAreaWidth / Math.Max(1.0, effectiveWidth);
        double scaleY = workAreaHeight / Math.Max(1.0, effectiveHeight);

        return Math.Min(scaleX, scaleY);
    }
}
