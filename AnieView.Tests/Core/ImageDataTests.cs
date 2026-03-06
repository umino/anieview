using AnieView.Core.Models;

namespace AnieView.Tests.Core;

public class ImageDataTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var pixels = new byte[] { 1, 2, 3, 4 };
        var data = new ImageData(pixels, 100, 200, 96.0, 72.0);

        Assert.Equal(pixels, data.PixelBuffer);
        Assert.Equal(100, data.Width);
        Assert.Equal(200, data.Height);
        Assert.Equal(96.0, data.DpiX);
        Assert.Equal(72.0, data.DpiY);
    }

    [Fact]
    public void Constructor_DefaultDpi_Is96()
    {
        var data = new ImageData(new byte[4], 10, 10);

        Assert.Equal(96.0, data.DpiX);
        Assert.Equal(96.0, data.DpiY);
    }

    [Fact]
    public void Constructor_RawNativeImage_IsNull_ByDefault()
    {
        var data = new ImageData(new byte[4], 10, 10);

        Assert.Null(data.RawNativeImage);
    }

    [Fact]
    public void Constructor_RawNativeImage_CanBeSet()
    {
        var nativeObj = new object();
        var data = new ImageData(new byte[4], 10, 10, rawNativeImage: nativeObj);

        Assert.Same(nativeObj, data.RawNativeImage);
    }
}
