using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

public interface ISaveImageService
{
    /// <summary>
    /// 「名前を付けて保存」ダイアログを開き、指定範囲（または全体）を保存する。
    /// cropRect が null の場合は画像全体を保存する。
    /// </summary>
    Task SaveAsync(ImageData imageData, (int X, int Y, int Width, int Height)? cropRect);
}
