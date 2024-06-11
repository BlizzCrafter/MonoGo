using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class FollowPositionModifier : IModifier
    {
        [JsonIgnore]
        public IMovable ObjectReference { get; set; }
        public Vector2 Offset { get; set; }
        public float Speed { get; set; } = 1f;
        public bool Inside { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                Vector2 position = particle->Position;
                if (ObjectReference != null)
                {
                    var angle = new Angle(position, ObjectReference.Position + Offset);
                    if (Inside) angle *= -1;
                    position += (Vector2)angle * Speed * elapsedSeconds;
                }
                particle->Position = position;
            }
        }
    }
}
