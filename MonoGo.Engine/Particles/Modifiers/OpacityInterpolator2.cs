﻿namespace MonoGo.Engine.Particles.Modifiers 
{
    public class OpacityInterpolator2 : IModifier
    {
        public string Key { get; set; }

        public float InitialOpacity { get; set; }
        public float FinalOpacity { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = FinalOpacity - InitialOpacity;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Opacity = delta * particle->Age + InitialOpacity;
            }
        }
    }
}