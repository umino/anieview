using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using AnieView.Core.Models;

namespace AnieView.Wpf.Converters;

/// <summary>
/// ImageData モデルから WPF の BitmapSource へ変換するコンバーター。
/// これにより ViewModel が WPF の具体的な型（BitmapSource）を知る必要がなくなる。
/// </summary>
public class ImageDataToSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ImageData imageData) return null;

        // すでに WPF 用のオブジェクト（RawNativeImage）を持っている場合はそれを優先
        if (imageData.RawNativeImage is BitmapSource bitmapSource)
        {
            return bitmapSource;
        }

        // TODO: RawNativeImage がない場合は PixelBuffer から生成するロジックをここに書ける
        // 今回は Infrastructure で生成して保持しているため、基本的には上のルートを通る

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
