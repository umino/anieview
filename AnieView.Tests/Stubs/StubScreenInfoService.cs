using AnieView.Core.Interfaces;

namespace AnieView.Tests.Stubs;

public class StubScreenInfoService : IScreenInfoService
{
    public double WorkAreaWidth { get; set; } = 1920;
    public double WorkAreaHeight { get; set; } = 1040;
    public double PrimaryScreenWidth { get; set; } = 1920;
    public double PrimaryScreenHeight { get; set; } = 1080;
}
