﻿namespace MonoGo.Engine.Particles.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public class ColourInterpolator2 : IModifier 
    {
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public HSLColor InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public HSLColor FinalColour { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = new HSLColor(FinalColour.H - InitialColour.H,
                                   FinalColour.S - InitialColour.S,
                                   FinalColour.L - InitialColour.L);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Colour = new HSLColor(
                    InitialColour.H + delta.H * particle->Age,
                    InitialColour.S + delta.S * particle->Age,
                    InitialColour.L + delta.L * particle->Age);
            }
        }
    }
}