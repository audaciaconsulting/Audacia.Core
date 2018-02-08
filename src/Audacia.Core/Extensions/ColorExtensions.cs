using System.Drawing;

namespace Audacia.Core.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns 6 digit hex representation of a .NET colour
        /// </summary>
        /// <param name="color">the colour to convert</param>
        /// <returns></returns>
        public static string ToHexString(this Color color)
        {
            return $"{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
        }
    }
}