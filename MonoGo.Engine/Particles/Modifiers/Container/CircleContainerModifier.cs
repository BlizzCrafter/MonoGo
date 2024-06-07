using Microsoft.Xna.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers.Container
{
    public class CircleContainerModifier : IModifier
    {
        public class CircleContainerModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(CircleContainerModifier);
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new CircleContainerModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public float Radius { get; set; }
        public bool Inside { get; set; } = true;
        public float RestitutionCoefficient { get; set; } = 1;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var radiusSq = Radius*Radius;
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var localPos = particle->Position - particle->TriggerPos;

                var distSq = localPos.LengthSq();
                var normal = localPos.Axis().ToVector2();

                if (Inside)
                {
                    if (distSq < radiusSq) continue;

                    SetReflected(distSq, particle, normal);
                }
                else
                {
                    if (distSq > radiusSq) continue;
                    
                    SetReflected(distSq, particle, -normal);
                }
                
            }
        }

        private unsafe void SetReflected(float distSq, Particle* particle, Vector2 normal)
        {
            var dist = (float) Math.Sqrt(distSq);
            var d = dist - Radius; // how far outside the circle is the particle

            var twoRestDot = 2*RestitutionCoefficient*
                             Vector2.Dot(particle->Velocity, normal);
            particle->Velocity -= twoRestDot*normal;

            // exact computation requires sqrt or goniometrics
            particle->Position -= normal*d;
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