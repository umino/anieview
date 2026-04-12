using AnieView.Wpf.ViewModels;
using AnieView.Application.UseCases;

namespace AnieView.Wpf.Views;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;
    private readonly CalculateWindowPlacementUseCase _placementUseCase;

    private Point _selectionStart;
    private bool _isDrawing = false;

    public MainWindow(MainViewModel viewModel, CalculateWindowPlacementUseCase placementUseCase)
    {
        _placementUseCase = placementUseCase;
        InitializeComponent();
        DataContext = viewModel;

        this.SizeChanged += MainWindow_SizeChanged;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsClippingMode))
                OnClippingModeChanged(viewModel.IsClippingMode);
        };
    }

    private void OnClippingModeChanged(bool isClipping)
    {
        SelectionCanvas.IsHitTestVisible = isClipping;
        this.Cursor = isClipping ? Cursors.Cross : Cursors.Arrow;
        if (!isClipping)
        {
            SelectionRect.Visibility = Visibility.Collapsed;
            _isDrawing = false;
            ResetSizeToContent();
        }
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
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
        bool isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        // クリッピングモード中は Esc でキャンセル
        if (ViewModel.IsClippingMode && e.Key == Key.Escape)
        {
            e.Handled = true;
            ViewModel.CancelClipping();
            return;
        }

        switch (e.Key)
        {
            case Key.Y when isCtrl:
                ViewModel.ToggleClippingModeCommand.Execute(null);
                break;
            case Key.Right:
                ViewModel.NavigateCommand.Execute("1");
                ResetSizeToContent();
                break;
            case Key.Space:
                ViewModel.NavigateCommand.Execute(isShift ? "1" : "-1");
                ResetSizeToContent();
                break;
            case Key.Left:
            case Key.Back:
                ViewModel.NavigateCommand.Execute("-1");
                ResetSizeToContent();
                break;
            case Key.OemPlus:
                ViewModel.ZoomInCommand.Execute(null);
                ResetSizeToContent();
                break;
            case Key.OemMinus:
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
                _ = ViewModel.SaveAsCommand.ExecuteAsync(null);
                break;
            case Key.O:
                ViewModel.ToggleSortOrderCommand.Execute(null);
                break;
            case Key.B:
                ViewModel.BringAllToFrontCommand.Execute(null);
                break;
            case Key.Escape:
                e.Handled = true;
                Close();
                break;
            case Key.Q:
                if (isShift)
                    System.Windows.Application.Current.Shutdown();
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

    // -------------------------------------------------------
    // 矩形クリッピング マウスハンドラ
    // -------------------------------------------------------

    private void SelectionCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!ViewModel.IsClippingMode) return;
        _selectionStart = e.GetPosition(SelectionCanvas);
        _isDrawing = true;
        SelectionCanvas.CaptureMouse();

        Canvas.SetLeft(SelectionRect, _selectionStart.X);
        Canvas.SetTop(SelectionRect, _selectionStart.Y);
        SelectionRect.Width = 0;
        SelectionRect.Height = 0;
        SelectionRect.Visibility = Visibility.Visible;
    }

    private void SelectionCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDrawing) return;
        var current = e.GetPosition(SelectionCanvas);

        double left = Math.Min(_selectionStart.X, current.X);
        double top = Math.Min(_selectionStart.Y, current.Y);
        double width = Math.Abs(current.X - _selectionStart.X);
        double height = Math.Abs(current.Y - _selectionStart.Y);

        Canvas.SetLeft(SelectionRect, left);
        Canvas.SetTop(SelectionRect, top);
        SelectionRect.Width = width;
        SelectionRect.Height = height;
    }

    private void SelectionCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDrawing) return;
        _isDrawing = false;
        SelectionCanvas.ReleaseMouseCapture();

        double rectLeft = Canvas.GetLeft(SelectionRect);
        double rectTop = Canvas.GetTop(SelectionRect);
        double rectWidth = SelectionRect.Width;
        double rectHeight = SelectionRect.Height;

        // 小さすぎる選択はキャンセル
        if (rectWidth < 2 || rectHeight < 2)
        {
            ViewModel.CancelClipping();
            return;
        }

        var pixelRect = CanvasRectToImagePixels(rectLeft, rectTop, rectWidth, rectHeight);
        if (pixelRect.HasValue)
        {
            var (px, py, pw, ph) = pixelRect.Value;
            if (pw > 0 && ph > 0)
                ViewModel.SetCropRect(px, py, pw, ph);
            else
                ViewModel.CancelClipping();
        }
        else
        {
            ViewModel.CancelClipping();
        }
    }

    /// <summary>
    /// Canvas 座標系の矩形をメイン画像のピクセル座標に変換する。
    /// TransformToAncestor で Window 経由にて Image ローカル座標へマッピングし、DPI を考慮してピクセル変換する。
    /// </summary>
    private (int X, int Y, int Width, int Height)? CanvasRectToImagePixels(
        double left, double top, double width, double height)
    {
        if (ViewModel.DisplayImage == null) return null;

        try
        {
            // Canvas → Window → Image ローカル座標
            var canvasToWindow = SelectionCanvas.TransformToAncestor(this);
            var imageToWindow = MainImage.TransformToAncestor(this);
            var windowToImage = imageToWindow.Inverse;

            Point tlCanvas = new(left, top);
            Point brCanvas = new(left + width, top + height);

            Point tlWindow = canvasToWindow.Transform(tlCanvas);
            Point brWindow = canvasToWindow.Transform(brCanvas);

            Point tlImage = windowToImage.Transform(tlWindow);
            Point brImage = windowToImage.Transform(brWindow);

            // Image ローカル DIP → ピクセル
            var img = ViewModel.DisplayImage;
            double scaleX = img.DpiX / 96.0;
            double scaleY = img.DpiY / 96.0;

            int x = (int)(tlImage.X * scaleX);
            int y = (int)(tlImage.Y * scaleY);
            int x2 = (int)(brImage.X * scaleX);
            int y2 = (int)(brImage.Y * scaleY);

            // クランプ
            x = Math.Max(0, Math.Min(x, img.Width));
            y = Math.Max(0, Math.Min(y, img.Height));
            x2 = Math.Max(0, Math.Min(x2, img.Width));
            y2 = Math.Max(0, Math.Min(y2, img.Height));

            return (x, y, x2 - x, y2 - y);
        }
        catch
        {
            return null;
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
