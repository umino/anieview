using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Infrastructure.Services;

public class WpfSaveImageService : ISaveImageService
{
    // ShowDialog はメッセージポンプを回すため、キーリピートによる再入を防ぐ
    private bool _isSaving = false;

    public Task SaveAsync(ImageData imageData, (int X, int Y, int Width, int Height)? cropRect)
    {
        if (_isSaving) return Task.CompletedTask;

        var bitmap = imageData.RawNativeImage as BitmapSource;
        if (bitmap == null) return Task.CompletedTask;

        var dialog = new SaveFileDialog
        {
            Title = "名前を付けて保存",
            Filter = "PNG画像|*.png|JPEG画像|*.jpg|WebP画像|*.webp|BMP画像|*.bmp",
            DefaultExt = ".png",
            FilterIndex = 1
        };

        _isSaving = true;
        try
        {
            if (dialog.ShowDialog() != true) return Task.CompletedTask;

            BitmapSource source = bitmap;
            if (cropRect.HasValue)
            {
                var (cx, cy, cw, ch) = cropRect.Value;
                cx = Math.Max(0, cx);
                cy = Math.Max(0, cy);
                cw = Math.Min(cw, bitmap.PixelWidth - cx);
                ch = Math.Min(ch, bitmap.PixelHeight - cy);

                if (cw > 0 && ch > 0)
                    source = new CroppedBitmap(bitmap, new System.Windows.Int32Rect(cx, cy, cw, ch));
            }

            var ext = Path.GetExtension(dialog.FileName).ToLower();
            if (ext == ".webp")
                SaveAsWebp(source, dialog.FileName);
            else
                SaveWithWpfEncoder(source, dialog.FileName, ext);
        }
        finally
        {
            // Win32コモンダイアログが閉じる際にWM_KEYDOWNがメインウィンドウへ再配信される。
            // Input優先度のキーイベントより低いBackground優先度でリセットを遅延させ、
            // 残留キーイベントを _isSaving=true の状態でブロックする。
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                new Action(() => _isSaving = false),
                System.Windows.Threading.DispatcherPriority.Background);
        }

        return Task.CompletedTask;
    }

    private static void SaveWithWpfEncoder(BitmapSource source, string filePath, string ext)
    {
        BitmapEncoder encoder = ext switch
        {
            ".jpg" or ".jpeg" => new JpegBitmapEncoder { QualityLevel = 95 },
            ".bmp" => new BmpBitmapEncoder(),
            _ => new PngBitmapEncoder()
        };

        encoder.Frames.Add(BitmapFrame.Create(source));
        using var stream = new FileStream(filePath, FileMode.Create);
        encoder.Save(stream);
    }

    private static void SaveAsWebp(BitmapSource source, string filePath)
    {
        // BitmapSource → PNG メモリストリーム → ImageSharp → WebP
        using var pngStream = new MemoryStream();
        var pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(source));
        pngEncoder.Save(pngStream);
        pngStream.Position = 0;

        using var img = SixLabors.ImageSharp.Image.Load(pngStream);
        using var outStream = new FileStream(filePath, FileMode.Create);
        img.SaveAsWebp(outStream, new WebpEncoder { Quality = 90 });
    }
}
