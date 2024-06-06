using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers 
{
    public class HueInterpolator2 : IModifier
    {
        public class HueInterpolator2Converter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(HueInterpolator2));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new HueInterpolator2();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public float InitialHue { get; set; }
        public float FinalHue { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var delta = FinalHue - InitialHue;

            while (iterator.HasNext) {
                var particle = iterator.Next();
                particle->Colour = new HSL(
                    delta * particle->Age + InitialHue,
                    particle->Colour.S,
                    particle->Colour.L);
            }
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        public void UpdateReferences(ref object _object)
        {
        }
    }
}