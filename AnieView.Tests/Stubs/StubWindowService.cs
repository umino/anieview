using AnieView.Core.Interfaces;

namespace AnieView.Tests.Stubs;

public class StubWindowService : IWindowService
{
    public int OpenDuplicateWindowCallCount { get; private set; }
    public string? LastFilePath { get; private set; }
    public double LastZoomPercentage { get; private set; }
    public int LastRotationAngle { get; private set; }

    public void OpenDuplicateWindow(string filePath, double zoomPercentage, int rotationAngle)
    {
        OpenDuplicateWindowCallCount++;
        LastFilePath = filePath;
        LastZoomPercentage = zoomPercentage;
        LastRotationAngle = rotationAngle;
    }
}
