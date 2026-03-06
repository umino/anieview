using AnieView.Application.UseCases;
using AnieView.Tests.Stubs;

namespace AnieView.Tests.Application;

public class NavigateImageUseCaseTests
{
    [Fact]
    public void GetNextPath_ReturnsNextFile()
    {
        var stub = new StubNavigationService { NextFileResult = @"C:\next.jpg" };
        var useCase = new NavigateImageUseCase(stub);

        var result = useCase.GetNextPath(@"C:\current.jpg");

        Assert.Equal(@"C:\next.jpg", result);
    }

    [Fact]
    public void GetPreviousPath_ReturnsPreviousFile()
    {
        var stub = new StubNavigationService { PreviousFileResult = @"C:\prev.jpg" };
        var useCase = new NavigateImageUseCase(stub);

        var result = useCase.GetPreviousPath(@"C:\current.jpg");

        Assert.Equal(@"C:\prev.jpg", result);
    }

    [Fact]
    public void GetNextPath_NoNext_ReturnsNull()
    {
        var stub = new StubNavigationService { NextFileResult = null };
        var useCase = new NavigateImageUseCase(stub);

        var result = useCase.GetNextPath(@"C:\current.jpg");

        Assert.Null(result);
    }
}
