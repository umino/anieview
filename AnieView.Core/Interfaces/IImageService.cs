using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

public interface IImageService
{
    Task<ImageData?> LoadImageAsync(string filePath);
    Task<ImageData> CreateEmptyImageAsync(int width, int height);
}
