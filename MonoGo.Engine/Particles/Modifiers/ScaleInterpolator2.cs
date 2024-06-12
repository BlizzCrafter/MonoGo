using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Modifiers 
{
    public class ScaleInterpolator2 : IModifier
    {
        public string Key { get; set; }

        public Vector2 InitialScale { get; set; }
        public Vector2 FinalScale { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = FinalScale - InitialScale;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Scale = delta * particle->Age + InitialScale;
            }
        }
    }
}