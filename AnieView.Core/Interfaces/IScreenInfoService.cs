namespace AnieView.Core.Interfaces;

public interface IScreenInfoService
{
    double WorkAreaWidth { get; }
    double WorkAreaHeight { get; }
    double PrimaryScreenWidth { get; }
    double PrimaryScreenHeight { get; }
}
