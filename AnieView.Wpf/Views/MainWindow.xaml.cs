using System.Windows;
using System.Windows.Input;
using AnieView.Wpf.ViewModels;

namespace AnieView.Wpf.Views;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        SizeToContent = SizeToContent.WidthAndHeight;

        // Check for command line arguments (image path)
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            var fullPath = System.IO.Path.GetFullPath(args[1]);
            _ = ViewModel.LoadInitialImage(fullPath);
        }
        else
        {
            ViewModel.EmptyImageCommand.Execute(null);
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        // Map keys to commands based on requirements
        switch (e.Key)
        {
            case Key.Right:
            case Key.Space:
                ViewModel.NavigateCommand.Execute("1");
                break;
            case Key.Left:
            case Key.Back:
                ViewModel.NavigateCommand.Execute("-1");
                break;
            case Key.Oem1: // ; (Semicolon) on Japanese keyboard
                ViewModel.ZoomInCommand.Execute(null);
                break;
            case Key.OemMinus: // -
                ViewModel.ZoomOutCommand.Execute(null);
                break;
            case Key.R:
                ViewModel.RotateCommand.Execute(null);
                break;
            case Key.F:
                ViewModel.ToggleFitToScreenCommand.Execute(null);
                break;
            case Key.D:
                ViewModel.DuplicateCommand.Execute(null);
                break;
            case Key.Escape:
                Close();
                break;
            
            // Fractional Resizing (2, 3, 4, 5, 6)
            case Key.D2:
            case Key.D3:
            case Key.D4:
            case Key.D5:
            case Key.D6:
                int n = (int)e.Key - (int)Key.D0;
                string param = $"{(isShift ? 'H' : 'W')}{n}";
                ViewModel.ResizeToFractionCommand.Execute(param);
                break;
        }
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                _ = ViewModel.LoadInitialImage(files[0]);
            }
        }
    }
}
