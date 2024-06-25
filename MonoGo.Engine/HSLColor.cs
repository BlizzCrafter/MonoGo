using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGo.Engine
{
    /// <summary>
    /// An immutable data structure representing a 24bit colour composed of separate hue, saturation and lightness channels.
    /// </summary>
    public struct HSLColor : IEquatable<HSLColor>
    {
        public class HSLConverter : JsonConverter<HSLColor>
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HSLColor);
            }

            public override HSLColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, HSLColor value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        /// <summary>
        /// Gets the value of the hue channel in degrees.
        /// </summary>
        public float H;

        /// <summary>
        /// Gets the value of the saturation channel.
        /// </summary>
        public float S;

        /// <summary>
        /// Gets the value of the lightness channel.
        /// </summary>
        public float L;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSLColor"/> structure.
        /// </summary>
        public HSLColor(Color color) : this()
        {
            this = color.ToHsl();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSLColor"/> structure.
        /// </summary>
        /// <param name="h">The value of the hue channel.</param>
        /// <param name="s">The value of the saturation channel.</param>
        /// <param name="l">The value of the lightness channel.</param>
        public HSLColor(float h, float s, float l) : this()
        {
            // normalize the hue
            H = NormalizeHue(h);
            S = MathHelper.Clamp(s, 0f, 1f);
            L = MathHelper.Clamp(l, 0f, 1f);
        }

        private static float NormalizeHue(float h)
        {
            if (h < 0) return h + 360 * ((int)(h / 360) + 1);
            return h % 360;
        }

        /// <summary>
        /// Copies the individual channels of the colour to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public void CopyTo(out HSLColor destination)
        {
            destination = new HSLColor(H, S, L);
        }

        /// <summary>
        /// Destructures the colour, exposing the individual channels.
        /// </summary>
        public void Destructure(out float h, out float s, out float l)
        {
            h = H;
            s = S;
            l = L;
        }

        /// <summary>
        /// Exposes the individual channels of the colour to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual channels of the colour.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback"/> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float, float> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(H, S, L);
        }

        /// <summary>
        /// Exposes the individual channels of the colour to the specified mapping function and returns the
        /// result;
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the colour channels to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map"/> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, float, T> map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(H, S, L);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HSLColor)
                return Equals((HSLColor)obj);

            return base.Equals(obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="HSLColor"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="HSLColor"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="HSLColor"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(HSLColor value)
        {
            return H.Equals(value.H) &&
                   S.Equals(value.S) &&
                   L.Equals(value.L);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return H.GetHashCode() ^
                   S.GetHashCode() ^
                   L.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}°;{1:P0};{2:P0}",
                H.ToString("F1"), S.ToString("F1"), L.ToString("F1"));
        }

        public static HSLColor Parse(string s)
        {
            var hsl = s.Split(';');
            var hue = float.Parse(hsl[0].TrimEnd('°'));
            var sat = float.Parse(hsl[1]);
            var lig = float.Parse(hsl[2]);

            return new HSLColor(hue, sat, lig);
        }

        public static Color OppositeColorRGB(Color input)
        {
            HSLColor hslColor = new Color(
                input.R,
                input.G,
                input.B,
                (byte)255).ToHsl();
            float h = (hslColor.H + 180) % 360;
            return ColorHelper.ToColor(h, hslColor.S, hslColor.L);
        }

        public static Color InvertedColorRGB(Color input)
        {
            HSLColor hslColor = new Color(
                input.R,
                input.G,
                input.B,
                (byte)255).ToHsl();
            float h = 360 - hslColor.H;
            return ColorHelper.ToColor(h, hslColor.S, hslColor.L);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HSLColor"/> is equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(HSLColor x, HSLColor y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The lvalue.</param>
        /// <param name="y">The rvalue.</param>
        /// <returns>
        ///     <c>true</c> if the lvalue <see cref="HSLColor"/> is not equal to the rvalue; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(HSLColor x, HSLColor y)
        {
            return !x.Equals(y);
        }

        public static HSLColor operator -(HSLColor a, HSLColor b)
        {
            return new HSLColor(a.H - b.H, a.S - b.S, a.L - b.L);
        }

        public static HSLColor Lerp(HSLColor c1, HSLColor c2, float t)
        {
            // loop around if c2.H < c1.H
            var h2 = c2.H >= c1.H ? c2.H : c2.H + 360;
            return new HSLColor(
                c1.H + t * (h2 - c1.H),
                c1.S + t * (c2.S - c1.S),
                c1.L + t * (c2.L - c2.L));
        }
    }
}