using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MonoGo.Engine
{
    public struct HSLRange
    {
        public class ColourRangeConverter : JsonConverter<HSLRange>
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HSLRange);
            }

            public override HSLRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, HSLRange value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        public HSLRange(HSLColor min, HSLColor max)
        {
            Min = min;
            Max = max;
        }

        public readonly HSLColor Min;
        public readonly HSLColor Max;

        public static HSLRange Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                var noBrackets = value.Substring(1, value.Length - 2);
                var colors = noBrackets.Split(';');
                var c1 = HSLColor.Parse($"{colors[0]};{colors[1]};{colors[2]}");
                var c2 = HSLColor.Parse($"{colors[3]};{colors[4]};{colors[5]}");
                return new HSLRange(c1, c2);
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

        public static implicit operator HSLRange(HSLColor value)
        {
            return new HSLRange(value, value);
        }
    }
}