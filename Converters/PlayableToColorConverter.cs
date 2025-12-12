using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CCGGame.Converters
{
    /// <summary>
    /// Devuelve un color de borde dependiendo si la carta es jugable.
    /// </summary>
    public class PlayableToColorConverter : IValueConverter
    {
        public Color PlayableColor { get; set; } = Color.FromArgb("#5dffb2");
        public Color NonPlayableColor { get; set; } = Color.FromArgb("#6de2ff");

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool playable && playable)
                return PlayableColor;
            return NonPlayableColor;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
