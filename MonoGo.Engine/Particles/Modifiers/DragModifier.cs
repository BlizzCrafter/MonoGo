using Microsoft.Xna.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers {
    public class DragModifier : IModifier
    {
        public class DragModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(DragModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new DragModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public float DragCoefficient { get; set; } = 0.47f;
        public float Density { get; set; } = .5f;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            while (iterator.HasNext) {
                var particle = iterator.Next();
                var drag = -DragCoefficient * Density * particle->Mass * elapsedSeconds;

                particle->Velocity = new Vector2(
                    particle->Velocity.X + particle->Velocity.X * drag,
                    particle->Velocity.Y + particle->Velocity.Y * drag);
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