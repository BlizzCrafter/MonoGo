using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.SceneSystem;
using System.Linq;

namespace MonoGo.Samples.Misc
{
    public class ParticleSampleEntity : ParticleEffectEntity
    {
        public PositionComponent PositionComponent { get; set; }

        public ParticleSampleEntity(
            Layer layer, 
            ParticleEffect particleEffect,
            IMovable followable = default,
            Vector2 position = default) 
            : base(layer, particleEffect, followable, new PositionComponent(position))
        {
            PositionComponent = AddComponent(new PositionComponent(position));
        }

        public ParticleSampleEntity(
            Layer layer,
            string particleEffectFilePath,
            IMovable followable = default,
            Vector2 position = default)
            : base(layer, particleEffectFilePath, followable, new PositionComponent(position))
        {
            PositionComponent = AddComponent(new PositionComponent(position));
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
