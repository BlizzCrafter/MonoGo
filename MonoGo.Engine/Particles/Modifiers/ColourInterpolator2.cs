using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public sealed class ColourInterpolator2 : IModifier 
    {
        public class ColourInterpolator2Converter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ColourInterpolator2));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new ColourInterpolator2();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public HSL InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public HSL FinalColour { get; set; }

        public unsafe void Update(float elapsedseconds, ParticleBuffer.ParticleIterator iterator) {
            var delta = new HSL(FinalColour.H - InitialColour.H,
                                   FinalColour.S - InitialColour.S,
                                   FinalColour.L - InitialColour.L);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Colour = new HSL(
                    InitialColour.H + delta.H*particle->Age,
                    InitialColour.S + delta.S*particle->Age,
                    InitialColour.L + delta.L*particle->Age);
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