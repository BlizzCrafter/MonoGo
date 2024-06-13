using System.Linq;
using Microsoft.Xna.Framework;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Samples.Misc
{
    public class ParticleEditorEntity : Entity
    {
        public PositionComponent PositionComponent { get; set; }
        public ParticleEffect ParticleEffect { get; set; }

        public ParticleEditorEntity(
            Layer layer,
            Vector2 position,
            ParticleEffect particleEffect) 
            : base(layer)
        {
            PositionComponent = AddComponent(new PositionComponent(position));
            ParticleEffect = particleEffect;
        }

        public void OffsetX(float value)
        {
            ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(value, x.Offset.Y));
        }

        public void OffsetY(float value)
        {
            ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(x.Offset.X, value));
        }

        public void Speed(float value)
        {
            ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Speed = value);
        }

        public void ToggleInside()
        {
            ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Inside = !x.Inside);
        }
    }
}
