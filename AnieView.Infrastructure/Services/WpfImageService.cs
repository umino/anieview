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
}
