namespace AnieView.Core.Models;

public class ImageFile
{
    public string FilePath { get; }
    public string FileName => System.IO.Path.GetFileName(FilePath);
    public double ZoomPercentage { get; set; } = 100.0;
    public int RotationAngle { get; set; } = 0;

    public ImageFile(string filePath)
    {
        FilePath = filePath;
    }
}
