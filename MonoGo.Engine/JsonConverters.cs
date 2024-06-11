using System.Text.Json;
using static MonoGo.Engine.Axis;
using static MonoGo.Engine.HSL;
using static MonoGo.Engine.HSLRange;
using static MonoGo.Engine.Range;
using static MonoGo.Engine.RangeF;
using static MonoGo.Engine.Particles.Modifiers.ColourInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.DragModifier;
using static MonoGo.Engine.Particles.Modifiers.FollowPositionModifier;
using static MonoGo.Engine.Particles.Modifiers.HueInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.LinearGravityModifier;
using static MonoGo.Engine.Particles.Modifiers.NoModifier;
using static MonoGo.Engine.Particles.Modifiers.OpacityFastFadeModifier;
using static MonoGo.Engine.Particles.Modifiers.OpacityInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.RotationModifier;
using static MonoGo.Engine.Particles.Modifiers.ScaleInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.VelocityColourModifier;
using static MonoGo.Engine.Particles.Modifiers.VelocityHueModifier;
using static MonoGo.Engine.Particles.Modifiers.VortexModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.CircleContainerModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.RectContainerModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.RectLoopContainerModifier;
using static MonoGo.Engine.AdditionalConverters;
using System.Text.Json.Serialization;
using System;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace MonoGo.Engine
{
    internal static class JsonConverters
    {
        internal static JsonSerializerOptions SerializerOptions;

        internal static bool Initialized = false;

        internal static void Init()
        {
            if (Initialized) return;

            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true,
                Converters =
                {
                    new VectorConverter(),
                    new AxisConverter(),
                    new RangeConverter(),
                    new RangeFConverter(),
                    new HSLConverter(),
                    new ColourRangeConverter(),
                    new ColourInterpolator2Converter(),
                    new BaseTypeJsonConverter<Profile>(),
                    new BaseTypeJsonConverter<IModifier>()
                    new HueInterpolator2Converter(),
                    new LinearGravityModifierConverter(),
                    new NoModifierConverter(),
                    new OpacityFastFadeModifierConverter(),
                    new OpacityInterpolator2Converter(),
                    new RotationModifierConverter(),
                    new ScaleInterpolator2Converter(),
                    new VelocityColourModifierConverter(),
                    new VelocityHueModifierConverter(),
                    new VortexModifierConverter(),
                    new CircleContainerModifierConverter(),
                    new RectContainerModifierConverter(),
                    new RectLoopContainerModifierConverter()
                }
            };
            Initialized = true;
        }
    }

    public class AdditionalConverters
    {
        public class VectorConverter : JsonConverter<Vector2>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Vector2));
            }

            public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(WriteVector2(value));
            }

            public static Vector2 Parse(string value)
            {
                string trimed = value.TrimStart('(').TrimEnd(')');
                string[] xy = trimed.Split(';');

                float x = float.Parse(xy[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                float y = float.Parse(xy[1], NumberStyles.Float, CultureInfo.InvariantCulture);

                return new Vector2(x, y);
            }

            public string WriteVector2(Vector2 value)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    $"({value.X.ToString(CultureInfo.InvariantCulture)}; {value.Y.ToString(CultureInfo.InvariantCulture)})");
            }
        }
        public class BaseTypeJsonConverter<T> : JsonConverter<T>
        {
            private readonly Dictionary<string, Type> _baseTypes;

            public BaseTypeJsonConverter()
            {
                var typeList = typeof(T).GetTypeInfo().Assembly.DefinedTypes
                .Where(type => typeof(T).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);

                _baseTypes = typeList.ToDictionary(t => t.Name, t => t.AsType());
            }

            public override bool CanConvert(Type typeToConvert)
            {                
                return _baseTypes.ContainsValue(typeToConvert) || typeof(T) == typeToConvert;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                var name = reader.GetString();

                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    var jObject = doc.RootElement;
                    if (_baseTypes.TryGetValue(name, out Type type))
                    {
                        var value = (T)JsonSerializer.Deserialize(jObject.GetRawText(), type, GetTempOptions(options));
                        reader.Read();
                        return value;
                    }
                }
                reader.Read();
                return default;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                string name = ((dynamic)value).ToString();
                string shortName = name.Split('.').Last();

                writer.WriteStartObject();
                writer.WritePropertyName(shortName);
                JsonSerializer.Serialize(writer, (dynamic)value, GetTempOptions(options));
                writer.WriteEndObject();
            }

            private JsonSerializerOptions GetTempOptions(JsonSerializerOptions options)
            {
                var tempOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };
                foreach (var converter in options.Converters)
                {
                    if (converter == this) continue;
                    tempOptions.Converters.Add(converter);
                }
                return tempOptions;
            }
        }
    }
}
