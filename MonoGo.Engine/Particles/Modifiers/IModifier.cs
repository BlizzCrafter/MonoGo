namespace MonoGo.Engine.Particles.Modifiers 
{
    public interface IModifier
    {
        /// <summary>
        /// The key of this Modifier.
        /// </summary>
        string Key { get; set; }

        void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);
    }
}