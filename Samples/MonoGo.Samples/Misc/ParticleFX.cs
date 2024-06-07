using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.Particles.Profiles;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using System;

namespace MonoGo.Samples.Misc
{
    public class ParticleFollowFX : Entity
    {
        public ParticleEffect ParticleEffect { get; set; }
        public PositionComponent PositionComponent { get; set; }

        public ParticleFollowFX(Layer layer, PositionComponent followComponent, Vector2 position = default) : base(layer)
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
                        Name = "Splash",
                        Modifiers = new[]
                        {
                            new FollowObjectModifier()
                            {
                                ObjectReference = followComponent,
                                Inside = false,
                                Speed = 1f,
                                Offset = Vector2.Zero
                            }
                        }
                    }
                }
            };
        }

        public override void Update()
        {
            base.Update();

            ParticleEffect.Update((float)GameMgr.ElapsedTime);
            ParticleEffect.Trigger(PositionComponent.Position);
        }

        public override void Draw()
        {
            base.Draw();

            ParticleEffect.Draw();
        }

        public void OffsetX(float value)
        {
            foreach (Emitter emitter in ParticleEffect.Emitters)
            {
                foreach (IModifier modifier in emitter.Modifiers)
                {
                    if (modifier is FollowObjectModifier)
                    {
                        var followModifier = modifier as FollowObjectModifier;
                        followModifier.Offset = new Vector2(value, followModifier.Offset.Y);
                    }
                }
            }
        }

        public void OffsetY(float value)
        {
            foreach (Emitter emitter in ParticleEffect.Emitters)
            {
                foreach (IModifier modifier in emitter.Modifiers)
                {
                    if (modifier is FollowObjectModifier)
                    {
                        var followModifier = modifier as FollowObjectModifier;
                        followModifier.Offset = new Vector2(followModifier.Offset.X, value);
                    }
                }
            }
        }

        public void Speed(float value)
        {
            foreach (Emitter emitter in ParticleEffect.Emitters)
            {
                foreach (IModifier modifier in emitter.Modifiers)
                {
                    if (modifier is FollowObjectModifier)
                    {
                        var followModifier = modifier as FollowObjectModifier;
                        followModifier.Speed = value;
                    }
                }
            }
        }

        public void Inside()
        {
            foreach (Emitter emitter in ParticleEffect.Emitters)
            {
                foreach (IModifier modifier in emitter.Modifiers)
                {
                    if (modifier is FollowObjectModifier)
                    {
                        var followModifier = modifier as FollowObjectModifier;
                        followModifier.Inside = !followModifier.Inside;
                    }
                }
            }
        }
    }
}
