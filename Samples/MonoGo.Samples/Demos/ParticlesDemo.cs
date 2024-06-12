using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.Particles.Profiles;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.UI.Entities.TextValidators;
using MonoGo.Samples.Misc;
using System;
using System.IO;

namespace MonoGo.Samples.Demos
{
    public class ParticlesDemo : Entity, IHaveGUI
    {
        public static readonly string Description =
            "Move > {{YELLOW}}WASD{{DEFAULT}}" + Environment.NewLine +
            "Particles > {{L_GREEN}}Update{{DEFAULT}}:{{YELLOW}}" + ToggleEnabledButton + " {{L_GREEN}}Draw{{DEFAULT}}:{{YELLOW}}" + ToggleVisibilityButton + " {{DEFAULT}}" + "{{L_GREEN}}Follow{{DEFAULT}}:{{YELLOW}}" + ToggleFollowEntityButton + " {{DEFAULT}}" + "{{L_GREEN}}Inside{{DEFAULT}}:{{YELLOW}}" + ToggleInsideButton + " {{DEFAULT}}";

        public const Buttons ToggleVisibilityButton = Buttons.N;
        public const Buttons ToggleEnabledButton = Buttons.M;
        public const Buttons ToggleFollowEntityButton = Buttons.F;
        public const Buttons ToggleInsideButton = Buttons.I;
        #region DEBUG
        public const Buttons SerializeButton = Buttons.K;
        public const Buttons DeserializeButton = Buttons.L;
        #endregion DEBUG

        private Player _player;
        private ParticleSampleEntity _particleSampleEntity;
        private bool _particlesFollowPlayer = true;

        private RichParagraph ActiveParticles_Paragraph;

        public ParticlesDemo(Layer layer) : base(layer)
        {
            var particleEffect = new ParticleEffect
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
                            new FollowPositionModifier()
                            {
                                Inside = false,
                                Speed = 1f,
                                Offset = Vector2.Zero
                            }
                        }
                    }
                }
            };

            _player = new Player(layer, new Vector2(400, 300))
            {
                Depth = 0
            };
            _particleSampleEntity = new ParticleSampleEntity(
                layer,
                particleEffect, 
                _player.GetComponent<PositionComponent>(), 
                GameMgr.WindowManager.CanvasCenter)
            {
                Depth = 1
            };

            layer.DepthSorting = true;
            layer.ReorderEntity(_player, 0);
            layer.ReorderEntity(_particleSampleEntity, 1);

            CheckParticleEffectFollowsPlayer();
        }

        public override void Update()
        {
            base.Update();

            if (ActiveParticles_Paragraph != null)
            {
                ActiveParticles_Paragraph.Text = "Active Particles:{{YELLOW}}" + _particleSampleEntity.ParticleEffect.ActiveParticles + "{{DEFAULT}}";
            }

            if (Input.CheckButtonPress(ToggleFollowEntityButton))
            {
                _particlesFollowPlayer = !_particlesFollowPlayer;
                CheckParticleEffectFollowsPlayer();
            }            

            if (Input.CheckButtonPress(ToggleInsideButton))
            {
                _particleSampleEntity.ToggleInside();
            }

            if (Input.CheckButtonPress(ToggleVisibilityButton))
            {
                foreach (var entity in Layer.GetEntityList<ParticleSampleEntity>())
                {
                    entity.Visible = !entity.Visible;
                }
            }

            if (Input.CheckButtonPress(ToggleEnabledButton))
            {
                foreach (var entity in Layer.GetEntityList<ParticleSampleEntity>())
                {
                    entity.Enabled = !entity.Enabled;
                }
            }

            #region DEBUG
            if (Input.CheckButtonPress(SerializeButton))
            {
                _particleSampleEntity.Serialize(Path.Combine(AppContext.BaseDirectory, "Exports"));
            }

            if (Input.CheckButtonPress(DeserializeButton))
            {
                _particleSampleEntity.Deserialize(Path.Combine(AppContext.BaseDirectory, "Exports", "Potpourri"));
            }
            #endregion DEBUG
        }

        private void CheckParticleEffectFollowsPlayer()
        {
            if (_particlesFollowPlayer)
            {
                foreach (var entity in Layer.GetEntityList<ParticleEffectEntity>())
                {
                    entity.Movable = _player.GetComponent<PositionComponent>();
                }
            }
            else
            {
                foreach (var entity in Layer.GetEntityList<ParticleEffectEntity>())
                {
                    entity.Movable = _particleSampleEntity.GetComponent<PositionComponent>();
                }
            }
        }

        public void CreateUI()
        {
            Panel topPanel = new(new Vector2(0, 60), PanelSkin.None, Anchor.TopCenter);

            ActiveParticles_Paragraph = new RichParagraph("", Anchor.Center);
            topPanel.AddChild(ActiveParticles_Paragraph);

            UserInterface.Active.AddUIEntity(topPanel);

            var descriptionPanel = UserInterface.Active.Root.Find("DescriptionPanel", true);
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Offest:X=0";
                textInput.OnValueChange += (EntityUI entityUI) => 
                {
                    float.TryParse(entityUI.GetValue().ToString(), out float value);
                    _particleSampleEntity.OffsetX(value);
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Offest:Y=0";
                textInput.OnValueChange += (EntityUI entityUI) => 
                {
                    float.TryParse(entityUI.GetValue().ToString(), out float value);
                    _particleSampleEntity.OffsetY(value);
                }; 
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Speed=1";
                textInput.OnValueChange += (EntityUI entityUI) => 
                {
                    float.TryParse(entityUI.GetValue().ToString(), out float value);
                    _particleSampleEntity.Speed(value);
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
        }
    }
}
