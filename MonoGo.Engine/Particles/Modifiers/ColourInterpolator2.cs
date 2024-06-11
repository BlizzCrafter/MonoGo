namespace MonoGo.Engine.Particles.Modifiers
{
    /// <summary>
    /// Defines a modifier which interpolates the colour of a particle over the course of its lifetime.
    /// </summary>
    public class ColourInterpolator2 : IModifier 
    {
        /// <summary>
        /// Gets or sets the initial colour of particles when they are released.
        /// </summary>
        public HSL InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the final colour of particles when they are retired.
        /// </summary>
        public HSL FinalColour { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = new HSL(FinalColour.H - InitialColour.H,
                                   FinalColour.S - InitialColour.S,
                                   FinalColour.L - InitialColour.L);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Colour = new HSL(
                    InitialColour.H + delta.H * particle->Age,
                    InitialColour.S + delta.S * particle->Age,
                    InitialColour.L + delta.L * particle->Age);
            }
        }
    }
}