using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class FollowPositionModifier : IModifier, IFollowable
    {
        public string Key { get; set; }

        [JsonIgnore]
        public IMovable Followable { get; set; }
        public Vector2 Offset { get; set; }
        public float Speed { get; set; } = 1f;
        public bool Inside { get; set; } = true;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                Vector2 position = particle->Position;
                if (Followable != null)
                {
                    var angle = new Angle(position, Followable.Position + Offset);
                    if (!Inside) angle *= -1;
                    position += (Vector2)angle * Speed * elapsedSeconds;
                }
                else
                {
                    var angle = new Angle(position, particle->TriggerPos + Offset);
                    if (!Inside) angle *= -1;
                    var distance = position - particle->TriggerPos + Offset;
                    var length = distance.Length();
                    if (length > 100f)
                    {
                        position += (Vector2)angle * Speed * elapsedSeconds;
                    }
                    else if (length > 1f)
                    {
                        var angle2 = new Angle(position, particle->TriggerPos);
                        var distance2 = position - particle->TriggerPos;
                        var length2 = distance.Length();
                        position += (Vector2)angle * Speed * elapsedSeconds;
                    }
                    else position = particle->TriggerPos;
                }
                particle->Position = position;
            }
        }
    }
}
