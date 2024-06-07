using Microsoft.Xna.Framework;
using MonoGo.Engine.Utils;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class FollowObjectModifier : IModifier
    {
        public class FollowObjectModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(FollowObjectModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new FollowObjectModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public dynamic ObjectReference { get; set; }
        public Vector2 Offset { get; set; }
        public float Speed { get; set; } = 1f;
        public bool Inside { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                Vector2 position = particle->Position;
                if (ObjectReference != null)
                {
                    Angle angle = new Angle(position, ObjectReference.Position + Offset);
                    if (Inside) angle *= -1;
                    position += angle.ToVector2() * Speed * elapsedSeconds;
                }
                particle->Position = position;
            }
        }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }
}
