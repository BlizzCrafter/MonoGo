using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Profiles;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using System;

namespace MonoGo.Samples.Misc
{
    public class ParticleFX : Entity
    {
        public ParticleEffect ParticleEffect { get; set; }
        public PositionComponent PositionComponent { get; set; }

        public Entity FollowEntity { get; set; }

        public ParticleFX(Layer layer, Vector2 position = default) : base(layer)
        {
            PositionComponent = AddComponent(new PositionComponent(position));

            ParticleEffect = new ParticleEffect
            {
                Name = "Potpourri",
                Emitters = new[]
                {
                    new Emitter(1000, TimeSpan.FromSeconds(3), new PointProfile())
                    {
                        Sprite = ResourceHub.GetResource<Sprite>("ParticleSprites", "Pixel"),
                        Loop = true,
                        Name = "Splash"
                    }
                }
            };
        }

        public override void Update()
        {
            base.Update();

            var position = PositionComponent.Position;
            if (FollowEntity != null)
            {
                if (FollowEntity.TryGetComponent(out PositionComponent posComponent))
                {
                    position = posComponent.Position;
                }
                else throw new NullReferenceException($"{FollowEntity.GetType().Name} entity need to have a {nameof(PositionComponent)} attached to it.");
            }

            ParticleEffect.Update((float)GameMgr.ElapsedTime);
            ParticleEffect.Trigger(position);
        }

        public override void Draw()
        {
            base.Draw();

            ParticleEffect.Draw();
        }
    }
}
