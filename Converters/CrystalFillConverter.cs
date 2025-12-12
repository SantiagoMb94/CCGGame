using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CCGGame.Converters
{
    /// <summary>
    /// Devuelve un color de cristal relleno o vacío según el maná disponible comparado con el índice (ConverterParameter).
    /// </summary>
    public class CrystalFillConverter : IValueConverter
    {
        public Color FilledColor { get; set; } = Color.FromArgb("#4dd3ff");
        public Color EmptyColor { get; set; } = Color.FromArgb("#2d2d3a");

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not int current) return EmptyColor;
            if (parameter == null || !int.TryParse(parameter.ToString(), out var index))
            {
                return EmptyColor;
            }

            return current >= index ? FilledColor : EmptyColor;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
