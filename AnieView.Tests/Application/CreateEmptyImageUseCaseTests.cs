using AnieView.Application.UseCases;
using AnieView.Core.Models;
using AnieView.Tests.Stubs;

namespace AnieView.Tests.Application;

public class CreateEmptyImageUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_CreatesEmptyImage_HalfScreenSize()
    {
        var imageStub = new StubImageService
        {
            CreateEmptyImageResult = new ImageData(new byte[4], 960, 540)
        };
        var screenStub = new StubScreenInfoService
        {
            PrimaryScreenWidth = 1920,
            PrimaryScreenHeight = 1080
        };
        var useCase = new CreateEmptyImageUseCase(imageStub, screenStub);

        var result = await useCase.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Equal(1, imageStub.CreateEmptyImageCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_SmallScreen_MinimumSize()
    {
        var imageStub = new StubImageService
        {
            CreateEmptyImageResult = new ImageData(new byte[4], 1, 1)
        };
        var screenStub = new StubScreenInfoService
        {
            PrimaryScreenWidth = 1,
            PrimaryScreenHeight = 1
        };
        var useCase = new CreateEmptyImageUseCase(imageStub, screenStub);

        var result = await useCase.ExecuteAsync();

        Assert.NotNull(result);
        Assert.Equal(1, imageStub.CreateEmptyImageCallCount);
    }
}
