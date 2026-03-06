using AnieView.Core.Models;

namespace AnieView.Tests.Core;

public class ImageFileTests
{
    [Fact]
    public void Constructor_SetsFilePath()
    {
        var file = new ImageFile(@"C:\images\test.jpg");

        Assert.Equal(@"C:\images\test.jpg", file.FilePath);
    }

    [Fact]
    public void FileName_ReturnsFileNameOnly()
    {
        var file = new ImageFile(@"C:\images\test.jpg");

        Assert.Equal("test.jpg", file.FileName);
    }

    [Fact]
    public void DefaultZoomPercentage_Is100()
    {
        var file = new ImageFile(@"C:\test.png");

        Assert.Equal(100.0, file.ZoomPercentage);
    }

    [Fact]
    public void DefaultRotationAngle_Is0()
    {
        var file = new ImageFile(@"C:\test.png");

        Assert.Equal(0, file.RotationAngle);
    }

    [Fact]
    public void ZoomPercentage_CanBeModified()
    {
        var file = new ImageFile(@"C:\test.png");
        file.ZoomPercentage = 150.0;

        Assert.Equal(150.0, file.ZoomPercentage);
    }
}
