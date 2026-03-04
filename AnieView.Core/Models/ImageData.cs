namespace AnieView.Core.Models;

/// <summary>
/// フレームワーク（WPF等）に依存しない、純粋な画像データモデル
/// </summary>
public class ImageData
{
    public byte[] PixelBuffer { get; }
    public int Width { get; }
    public int Height { get; }
    public double DpiX { get; }
    public double DpiY { get; }

    // 原型（BitmapSourceなど）を保持するためのフィールド。Infrastructure層での変換用。
    // クリーンアーキテクチャでは、これを object で持つのはグレーだが、今回は実用性を考慮
    public object? RawNativeImage { get; }

    public ImageData(byte[] pixelBuffer, int width, int height, double dpiX = 96.0, double dpiY = 96.0, object? rawNativeImage = null)
    {
        PixelBuffer = pixelBuffer;
        Width = width;
        Height = height;
        DpiX = dpiX;
        DpiY = dpiY;
        RawNativeImage = rawNativeImage;
    }
}
