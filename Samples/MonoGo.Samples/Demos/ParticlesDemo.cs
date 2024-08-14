using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.Particles.Profiles;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Misc;
using System;
using System.IO;

namespace MonoGo.Samples.Demos
{
    public class ParticlesDemo : Entity
    {
        public static readonly string Description = "Move > ${FC:FFDB5F}WASD${RESET}";

        #region DEBUG
        public const Buttons SerializeButton = Buttons.K;
        public const Buttons DeserializeButton = Buttons.L;
        #endregion DEBUG

        private Player _player;

        public ParticlesDemo(Layer layer) : base(layer)
        {
            layer.DepthSorting = true;

            var particleEffect = new ParticleEffect
            {
                Name = "Potpourri",
                Emitters = new[]
                {
                    new Emitter(1000, TimeSpan.FromSeconds(4), new PointProfile())
                    {
                        Sprite = ResourceHub.GetResource<Sprite>("ParticleSprites", "Pixel"),
                        Loop = true,
                        Name = "Splash",
                        Modifiers = new[]
                        {
                            new FollowPositionModifier()
                            {
                                Inside = true,
                                Speed = 1f,
                                Offset = Vector2.Zero
                            }
                        }
                    }
                }
            };

            var cParticleEffect = new ParticleEffectComponent(particleEffect, GameMgr.WindowManager.CanvasCenter) 
            { 
                Depth = 1 
            };

            _player = new Player(layer, new Vector2(400, 300));
            _player.AddComponentToTop(cParticleEffect);
            cParticleEffect.AttractParticlesTo(_player.GetComponent<PositionComponent>());

            new ParticleEditorEntity(layer);
        }

        public override void Update()
        {
            base.Update();

            #region DEBUG
            if (Input.CheckButtonPress(SerializeButton))
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.Serialize(Path.Combine(AppContext.BaseDirectory, "Exports"));
            }

            if (Input.CheckButtonPress(DeserializeButton))
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.Deserialize(Path.Combine(AppContext.BaseDirectory, "Exports", "Potpourri"));
            }
            #endregion DEBUG
        }
    }
}
