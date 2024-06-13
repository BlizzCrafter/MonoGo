using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class VortexModifier : IModifier
    {
        public string Key { get; set; }

        public Vector2 Position { get; set; }
        public float Mass { get; set; }
        public float MaxSpeed { get; set; }
        
        // Note: not the real-life one
        private const float GravConst = 100000f;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var diff = Position + particle->TriggerPos - particle->Position;

                var distance2 = diff.LengthSquared();

                var speedGain = GravConst * Mass / distance2 * elapsedSeconds;
                // normalize distances and multiply by speedGain
                particle->Velocity += diff.Axis() * speedGain;
            }
        }
    }
}