namespace MonoGo.Engine.Particles.Modifiers 
{
    public class OpacityFastFadeModifier : IModifier
    {
        public string Key { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Opacity = 1.0f - particle->Age;
            }
        }
    }
}