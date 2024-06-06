using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGo.Engine
{
    public static class ColorHelper
    {
        public static Color ToColor(this HSL c)
        {
            return ToColor(c.H, c.S, c.L);
        }

        public static Color ToColor(float h, float s, float l)
        {
            if (s == 0)
                return new Color(l, l, l);

            h = h / 360f;
            var max = l < 0.5f ? l * (1 + s) : l + s - l * s;
            var min = 2f * l - max;

            return new Color(
                ComponentFromHue(min, max, h + 1f / 3f),
                ComponentFromHue(min, max, h),
                ComponentFromHue(min, max, h - 1f / 3f));
        }

        private static float ComponentFromHue(float m1, float m2, float h)
        {
            h = (h + 1f) % 1f;
            if (h * 6f < 1)
                return m1 + (m2 - m1) * 6f * h;
            if (h * 2 < 1)
                return m2;
            if (h * 3 < 2)
                return m1 + (m2 - m1) * (2f / 3f - h) * 6f;
            return m1;
        }

        public static HSL ToHsl(this Color c)
        {
            return ToHsl(c.R, c.B, c.G);
        }

        public static HSL ToHsl(float r, float b, float g)
        {
            r = r / 255f;
            b = b / 255f;
            g = g / 255f;

            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            var sum = max + min;

            var l = sum * 0.5f;

            if (chroma == 0) return new HSL(0f, 0f, l);
            float h;
            if (r == max)
                h = (60 * (g - b) / chroma + 360) % 360;
            else if (g == max)
                h = 60 * (b - r) / chroma + 120f;
            else
                h = 60 * (r - g) / chroma + 240f;

            var s = l <= 0.5f ? chroma / sum : chroma / (2f - sum);

            return new HSL(h, s, l);
        }

        public static HSL InverseColour(this HSL colour)
        {
            return new HSL(360 - colour.H, colour.S, colour.L);
        }

        public static HSL OppositeColour(this HSL colour)
        {
            return new HSL((colour.H + 180) % 360, colour.S, colour.L);
        }

        /// <summary>
		/// Converts Color to its hex value.
		/// </summary>
		public static string ToHex(this Color color)
        {
            var r = $"{color.R:x2}";
            var g = $"{color.G:x2}";
            var b = $"{color.B:x2}";
            if (color.A == 0)
            {
                return $"#{r}{g}{b}";
            }
            var a = $"{color.A:x2}";
            return $"#{r}{g}{b}{a}";
        }


        /// <summary>
        /// Converts #RRGGBB or #RRGGBBAA hex value to Color.
        /// </summary>
        public static Color HexToColor(string colorStr)
        {
            colorStr = colorStr.Replace("#", "");

            var channels = new byte[colorStr.Length / 2];

            for (var i = 0; i < channels.Length; i += 1)
            {
                channels[i] = Convert.ToByte(colorStr.Substring(i * 2, 2), 16);
            }

            if (channels.Length == 3)
            {
                // #RRGGBB
                return new Color(channels[0], channels[1], channels[2]);
            }
            else
            {
                // #RRGGBBAA
                return new Color(channels[0], channels[1], channels[2], channels[3]);
            }
        }

        private static readonly Dictionary<string, Color> _colorsByName = typeof(Color)
            .GetRuntimeProperties()
            .Where(p => p.PropertyType == typeof(Color))
            .ToDictionary(p => p.Name, p => (Color)p.GetValue(null), StringComparer.OrdinalIgnoreCase);

        public static Color NameToColor(string name)
        {
            Color color;

            if (_colorsByName.TryGetValue(name, out color))
            {
                return color;
            }

            throw new InvalidOperationException($"{name} is not a valid color");
        }
    }
}
