using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Application.UseCases;

public class LoadImageUseCase
{
    private readonly IImageService _imageService;

    public LoadImageUseCase(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<ImageData?> ExecuteAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return null;
        return await _imageService.LoadImageAsync(filePath);
    }
}
