using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

public interface IImageService
{
    Task<object?> LoadImageAsync(string filePath);
}
