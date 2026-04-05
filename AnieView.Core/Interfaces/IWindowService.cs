namespace AnieView.Core.Interfaces;

public interface IWindowService
{
    /// <summary>
    /// 現在の画像を複製した新しいウィンドウを開く
    /// </summary>
    void OpenDuplicateWindow(string filePath, double zoomPercentage, int rotationAngle);

    /// <summary>
    /// アプリケーション内のすべてのウィンドウを前面に持ってくる
    /// </summary>
    void BringAllWindowsToForeground();
}
