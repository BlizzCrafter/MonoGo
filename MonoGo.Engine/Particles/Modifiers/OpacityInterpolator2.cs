using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers 
{
    public class OpacityInterpolator2 : IModifier
    {
        public class OpacityInterpolator2Converter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(OpacityInterpolator2));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new OpacityInterpolator2();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public float InitialOpacity { get; set; }
        public float FinalOpacity { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var delta = FinalOpacity - InitialOpacity;

            while (iterator.HasNext) {
                var particle = iterator.Next();
                particle->Opacity = delta * particle->Age + InitialOpacity;
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