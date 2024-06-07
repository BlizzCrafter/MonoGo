using Microsoft.Xna.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class LinearGravityModifier : IModifier
    {
        public class LinearGravityModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(LinearGravityModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new LinearGravityModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public Axis Direction { get; set; }
        public float Strength { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var vector = Direction * (Strength * elapsedSeconds);

            while (iterator.HasNext) {
                var particle = iterator.Next();
                particle->Velocity = new Vector2(
                    particle->Velocity.X + vector.X * particle->Mass,
                    particle->Velocity.Y + vector.Y * particle->Mass);
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