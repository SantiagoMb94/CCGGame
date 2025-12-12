using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CCGGame.Converters
{
    /// <summary>
    /// Convierte el tipo de carta en un ícono textual simple.
    /// </summary>
    public class TypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var type = value as string ?? string.Empty;
            return type switch
            {
                "Warrior" => "⚔️",
                "Mage" => "✨",
                "Ranger" => "🏹",
                "Beast" => "🐾",
                "Dragon" => "🐉",
                "Mech" => "🤖",
                "Elemental" => "🪨",
                "Demon" => "👹",
                "Priest" => "⛪",
                "Shaman" => "🌩️",
                "Human" => "🛡️",
                "Spell" => "🪄",
                _ => "🃏"
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

