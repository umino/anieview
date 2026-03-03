using System;
using System.IO;
using System.Windows.Media.Imaging;
using AnieView.Core.Interfaces;

namespace AnieView.Infrastructure.Services;

public class WpfImageService : IImageService
{
    public async Task<object?> LoadImageAsync(string filePath)
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
                return bitmap;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
            return null;
        }
    }
}
