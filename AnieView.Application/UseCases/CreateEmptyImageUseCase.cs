using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Application.UseCases;

public class CreateEmptyImageUseCase
{
    private readonly IImageService _imageService;
    private readonly IScreenInfoService _screenInfoService;

    public CreateEmptyImageUseCase(IImageService imageService, IScreenInfoService screenInfoService)
    {
        _imageService = imageService;
        _screenInfoService = screenInfoService;
    }

    public async Task<ImageData> ExecuteAsync()
    {
        // 画面サイズの1/4（面積比）に相当する大きさの空画像を作成する
        // 面積比が1/4なので、幅と高さはそれぞれ画面の1/2にする
        int width = Math.Max(1, (int)(_screenInfoService.PrimaryScreenWidth / 2.0));
        int height = Math.Max(1, (int)(_screenInfoService.PrimaryScreenHeight / 2.0));

        return await _imageService.CreateEmptyImageAsync(width, height);
    }
}
