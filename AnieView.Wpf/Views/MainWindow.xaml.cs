using AnieView.Wpf.ViewModels;
using AnieView.Application.UseCases;
using System.ComponentModel;

namespace AnieView.Wpf.Views;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;
    private readonly CalculateWindowPlacementUseCase _placementUseCase;

    public MainWindow(MainViewModel viewModel, CalculateWindowPlacementUseCase placementUseCase)
    {
        _placementUseCase = placementUseCase;
        InitializeComponent();
        DataContext = viewModel;
        
        // サイズ変更時の制御
        this.SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // はみ出し防止（クランプ）
        var (left, top) = _placementUseCase.ClampToScreen(
            this.Left, this.Top, 
            this.ActualWidth, this.ActualHeight, 
            SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);

        if (this.Left != left) this.Left = left;
        if (this.Top != top) this.Top = top;
    }

    /// <summary>
    /// 手動リサイズ後に SizeToContent が Manual になるため、コンテンツ追従リサイズを復元する。
    /// </summary>
    private void ResetSizeToContent()
    {
        this.Width = double.NaN;
        this.Height = double.NaN;
        this.SizeToContent = SizeToContent.WidthAndHeight;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        // Map keys to commands based on requirements
        switch (e.Key)
        {
            case Key.Right:
                ViewModel.NavigateCommand.Execute("1");
                ResetSizeToContent();
                break;
            case Key.Space:
                ViewModel.NavigateCommand.Execute(isShift ? "1": "-1");
                ResetSizeToContent();
                break;
            case Key.Left:
            case Key.Back:
                ViewModel.NavigateCommand.Execute("-1");
                ResetSizeToContent();
                break;
            case Key.OemPlus: // + (Plus) on Japanese keyboard
                ViewModel.ZoomInCommand.Execute(null);
                ResetSizeToContent();
                break;
            case Key.OemMinus: // -
                ViewModel.ZoomOutCommand.Execute(null);
                ResetSizeToContent();
                break;
            case Key.R:
                ViewModel.RotateCommand.Execute(null);
                ResetSizeToContent();
                break;
            case Key.F:
                ViewModel.ToggleFitToScreenCommand.Execute(null);
                ResetSizeToContent();
                break;
            case Key.D:
                ViewModel.DuplicateCommand.Execute(null);
                break;
            case Key.S:
                ViewModel.ToggleSortOrderCommand.Execute(null);
                break;
            case Key.Escape:
                e.Handled = true; // イベントが他のwindowに伝播しないようにする
                Close();
                break;
            case Key.Q:
                if (isShift)
                {
                    System.Windows.Application.Current.Shutdown();
                }
                break;
            case Key.B:
                ViewModel.BringAllToFrontCommand.Execute(null);
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
                ResetSizeToContent();
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
