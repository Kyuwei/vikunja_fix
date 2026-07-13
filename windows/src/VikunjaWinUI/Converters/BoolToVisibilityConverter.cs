using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace VikunjaWinUI.Converters;

/// <summary>
/// Maps a bool to <see cref="Visibility"/> for x:Bind, which — unlike classic
/// Binding — performs no implicit conversion. Pass ConverterParameter="invert"
/// to collapse on true instead of false.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is bool b && b;
        if (parameter is string s && string.Equals(s, "invert", StringComparison.OrdinalIgnoreCase))
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var visible = value is Visibility v && v == Visibility.Visible;
        if (parameter is string s && string.Equals(s, "invert", StringComparison.OrdinalIgnoreCase))
        {
            visible = !visible;
        }

        return visible;
    }
}
