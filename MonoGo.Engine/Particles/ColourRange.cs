using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MonoGo.Engine.Particles
{
    public struct ColourRange
    {
        public class ColourRangeConverter : JsonConverter<ColourRange>
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ColourRange);
            }

            public override ColourRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, ColourRange value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        public ColourRange(Colour min, Colour max) {
            Min = min;
            Max = max;
        }

        public readonly Colour Min;
        public readonly Colour Max;

        public static ColourRange Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                var noBrackets = value.Substring(1, value.Length - 2);
                var colors = noBrackets.Split(';');
                var c1 = Colour.Parse($"{colors[0]};{colors[1]};{colors[2]}");
                var c2 = Colour.Parse($"{colors[3]};{colors[4]};{colors[5]}");
                return new ColourRange(c1, c2);
            }
            catch
            {
                throw new FormatException(
                    "ColorRange should be formatted like: [HUE°, SAT, LUM;HUE°, SAT, LUM], but was " +
                    value);
            }
        }

        public override string ToString()
        {
            return "[" + Min + ';' + Max + ']';
        }

        public static implicit operator ColourRange(Colour value) {
            return new ColourRange(value, value);
        }
    }
}