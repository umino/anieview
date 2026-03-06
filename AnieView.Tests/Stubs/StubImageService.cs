using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Tests.Stubs;

public class StubImageService : IImageService
{
    public ImageData? LoadImageResult { get; set; }
    public ImageData CreateEmptyImageResult { get; set; } = new ImageData(new byte[4], 1, 1);
    public int LoadImageCallCount { get; private set; }
    public int CreateEmptyImageCallCount { get; private set; }
    public string? LastLoadedFilePath { get; private set; }

    public Task<ImageData?> LoadImageAsync(string filePath)
    {
        LoadImageCallCount++;
        LastLoadedFilePath = filePath;
        return Task.FromResult(LoadImageResult);
    }

    public Task<ImageData> CreateEmptyImageAsync(int width, int height)
    {
        CreateEmptyImageCallCount++;
        return Task.FromResult(CreateEmptyImageResult);
    }
}
