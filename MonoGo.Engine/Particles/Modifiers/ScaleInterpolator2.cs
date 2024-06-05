using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers 
{
    public class ScaleInterpolator2 : IModifier
    {
        public class ScaleInterpolator2Converter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ScaleInterpolator2));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new ScaleInterpolator2();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public Vector InitialScale { get; set; }
        public Vector FinalScale { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var delta = FinalScale - InitialScale;

            while (iterator.HasNext) {
                var particle = iterator.Next();
                particle->Scale = delta * particle->Age + InitialScale;
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