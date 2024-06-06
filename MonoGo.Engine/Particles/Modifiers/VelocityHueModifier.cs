using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class VelocityHueModifier : IModifier
    {
        public class VelocityHueModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(VelocityHueModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new VelocityHueModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public float StationaryHue { get; set; }
        public float VelocityHue { get; set; }
        public float VelocityThreshold { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (iterator.HasNext) {
                var particle = iterator.Next();
                var velocity2 = particle->Velocity.LengthSq;

                float h;
                if (velocity2 >= velocityThreshold2) {
                    h = VelocityHue;
                }
                else {
                    var t = (float)Math.Sqrt(velocity2) / VelocityThreshold;
                    h = MathHelper.Lerp(StationaryHue, VelocityHue, t);
                }
                particle->Colour = new HSL(h, particle->Colour.S, particle->Colour.L);
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