using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers
{
    public unsafe class VortexModifier : IModifier
    {
        public class VortexModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(VortexModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new VortexModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public Vector Position { get; set; }
        public float Mass { get; set; }
        public float MaxSpeed { get; set; }
        // Note: not the real-life one
        private const float GravConst = 100000f;

        public void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            while (iterator.HasNext) {
                var particle = iterator.Next();
                var diff = Position + particle->TriggerPos - particle->Position;
                
                var distance2 = diff.LengthSq;
                
                var speedGain = GravConst * Mass / distance2 * elapsedSeconds;
                // normalize distances and multiply by speedGain
                particle->Velocity += diff.Axis * speedGain;
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