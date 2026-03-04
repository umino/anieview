using System;
using System.IO;
using System.Windows.Media.Imaging;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Infrastructure.Services;

public class WpfImageService : IImageService
{
    public async Task<ImageData?> LoadImageAsync(string filePath)
    {
        try
        {
            return await Task.Run(() =>
            {
                var fullPath = System.IO.Path.GetFullPath(filePath);
                if (!System.IO.File.Exists(fullPath)) return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new System.Uri(fullPath, System.UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                // 簡易化のため、ピクセルバッファ抽出は省略し RawNativeImage に BitmapSource を入れる
                // 本来は Clean Architecture 的にはピクセルバッファを PixelBuffer に入れるべきだが、パフォーマンスを考慮
                return new ImageData(
                    pixelBuffer: Array.Empty<byte>(), 
                    width: bitmap.PixelWidth, 
                    height: bitmap.PixelHeight, 
                    dpiX: bitmap.DpiX, 
                    dpiY: bitmap.DpiY, 
                    rawNativeImage: bitmap);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
            return null;
        }
    }

    public async Task<ImageData> CreateEmptyImageAsync(int width, int height)
    {
        return await Task.Run(() =>
        {
            const double dpi = 96.0;
            var pixelFormat = System.Windows.Media.PixelFormats.Pbgra32;
            var writeable = new WriteableBitmap(width, height, dpi, dpi, pixelFormat, null);

            int bytesPerPixel = pixelFormat.BitsPerPixel / 8;
            int stride = width * bytesPerPixel;
            var pixels = new byte[height * stride];

            // 白で塗りつぶす（不透明）
            for (int i = 0; i < pixels.Length; i += bytesPerPixel)
            {
                pixels[i + 0] = 255; // B
                pixels[i + 1] = 255; // G
                pixels[i + 2] = 255; // R
                pixels[i + 3] = 255; // A
            }

            writeable.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, stride, 0);
            writeable.Freeze();

            return new ImageData(
                pixelBuffer: pixels,
                width: width,
                height: height,
                dpiX: dpi,
                dpiY: dpi,
                rawNativeImage: writeable);
        });
    }
}
