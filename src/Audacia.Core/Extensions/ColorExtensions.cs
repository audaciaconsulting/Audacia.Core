using System.Drawing;

namespace Audacia.Core.Extensions;

/// <summary>Extension methods for the type <see cref="Color"/>.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Returns 6 digit hex representation of a .NET colour.
    /// </summary>
    /// <param name="color">the colour to convert.</param>
    /// <returns>Color as a hex string.</returns>
    public static string ToHexString(this Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}