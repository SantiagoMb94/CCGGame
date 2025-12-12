using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CCGGame.Converters
{
    /// <summary>
    /// Convierte maná actual y máximo a progreso (0-1).
    /// </summary>
    public class ManaToProgressConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not int current) return 0d;
            int max = 10;
            if (parameter != null && int.TryParse(parameter.ToString(), out var parsedMax))
            {
                max = parsedMax;
            }

            if (max <= 0) return 0d;
            var progress = (double)current / max;
            return Math.Clamp(progress, 0d, 1d);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
