using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

public interface IImageService
{
    Task<ImageData?> LoadImageAsync(string filePath);
    Task<ImageData> CreateEmptyImageAsync(int width, int height);
    ImageData? CropImage(ImageData source, int x, int y, int width, int height);
}
