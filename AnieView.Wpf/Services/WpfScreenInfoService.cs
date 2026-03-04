using System.Windows;
using AnieView.Core.Interfaces;

namespace AnieView.Wpf.Services;

public class WpfScreenInfoService : IScreenInfoService
{
    public double WorkAreaWidth => SystemParameters.WorkArea.Width;
    public double WorkAreaHeight => SystemParameters.WorkArea.Height;
    public double PrimaryScreenWidth => SystemParameters.PrimaryScreenWidth;
    public double PrimaryScreenHeight => SystemParameters.PrimaryScreenHeight;
}
