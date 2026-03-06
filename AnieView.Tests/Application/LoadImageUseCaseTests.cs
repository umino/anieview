using AnieView.Application.UseCases;
using AnieView.Core.Models;
using AnieView.Tests.Stubs;

namespace AnieView.Tests.Application;

public class LoadImageUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ValidPath_CallsImageService()
    {
        var stub = new StubImageService
        {
            LoadImageResult = new ImageData(new byte[4], 100, 100)
        };
        var useCase = new LoadImageUseCase(stub);

        var result = await useCase.ExecuteAsync(@"C:\test.jpg");

        Assert.NotNull(result);
        Assert.Equal(1, stub.LoadImageCallCount);
        Assert.Equal(@"C:\test.jpg", stub.LastLoadedFilePath);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyPath_ReturnsNull()
    {
        var stub = new StubImageService();
        var useCase = new LoadImageUseCase(stub);

        var result = await useCase.ExecuteAsync("");

        Assert.Null(result);
        Assert.Equal(0, stub.LoadImageCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhitespacePath_ReturnsNull()
    {
        var stub = new StubImageService();
        var useCase = new LoadImageUseCase(stub);

        var result = await useCase.ExecuteAsync("   ");

        Assert.Null(result);
        Assert.Equal(0, stub.LoadImageCallCount);
    }
}
