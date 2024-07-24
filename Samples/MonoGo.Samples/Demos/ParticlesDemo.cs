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
        public static readonly string Description =
            "Move > ${FC:FFDB5F}WASD${RESET}" + Environment.NewLine +
            "Emitter > ${FC:96FF5F}Update${RESET}: ${FC:FFDB5F}" + ToggleEnabledButton + " ${FC:96FF5F}Draw${RESET}: ${FC:FFDB5F}" + ToggleVisibilityButton + " ${RESET}" + "${FC:96FF5F}Follow${RESET}: ${FC:FFDB5F}" + ToggleFollowButton + " ${RESET}" + "${FC:96FF5F}Attract${RESET}: ${FC:FFDB5F}" + ToggleAttractButton + " ${RESET}" + "${FC:96FF5F}Inside${RESET}: ${FC:FFDB5F}" + ToggleInsideButton + " ${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Move${RESET}: ${FC:FFDB5F}" + CameraController.UpButton + "${RESET} / ${FC:FFDB5F}" + CameraController.DownButton + "${RESET} / ${FC:FFDB5F}" + CameraController.LeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Rotate${RESET}: ${FC:FFDB5F}" + CameraController.RotateLeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RotateRightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Zoom${RESET}: ${FC:FFDB5F}" + CameraController.ZoomInButton + "${RESET} / ${FC:FFDB5F}" + CameraController.ZoomOutButton + "${RESET}" + Environment.NewLine;

        public const Buttons ToggleVisibilityButton = Buttons.N;
        public const Buttons ToggleEnabledButton = Buttons.M;
        public const Buttons ToggleInsideButton = Buttons.I;
        public const Buttons ToggleFollowButton = Buttons.F;
        public const Buttons ToggleAttractButton = Buttons.G;
        #region DEBUG
        public const Buttons SerializeButton = Buttons.K;
        public const Buttons DeserializeButton = Buttons.L;
        #endregion DEBUG

        private Player _player;
        private ParticleEditorEntity _particleEditorEntity;
        private bool _playerAttractsParticles = true;

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
            _particleEditorEntity = new ParticleEditorEntity(layer, cParticleEffect);

            CheckPlayerAttractsParticles();
        }

        public override void Update()
        {
            base.Update();

            if (Input.CheckButtonPress(ToggleFollowButton))
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.ToggleFollowOwner();
            }

            if (Input.CheckButtonPress(ToggleAttractButton))
            {
                _playerAttractsParticles = !_playerAttractsParticles;
                CheckPlayerAttractsParticles();
            }

            if (Input.CheckButtonPress(ToggleInsideButton))
            {
                _particleEditorEntity.ToggleInside();
            }

            if (Input.CheckButtonPress(ToggleVisibilityButton))
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.Visible = !cParticleEffect.Visible;
            }

            if (Input.CheckButtonPress(ToggleEnabledButton))
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.Enabled = !cParticleEffect.Enabled;
            }

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

        private void CheckPlayerAttractsParticles()
        {
            if (_playerAttractsParticles)
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.AttractParticlesTo(_player.GetComponent<PositionComponent>());
            }
            else
            {
                var cParticleEffect = _player.GetComponent<ParticleEffectComponent>();
                cParticleEffect.AttractParticlesTo(null);
            }
        }
    }
}
