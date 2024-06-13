using System;
using System.IO;
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

namespace MonoGo.Samples.Demos
{
    public class ParticlesDemo : Entity, IHaveGUI
    {
        public static readonly string Description =
            "Move > {{YELLOW}}WASD{{DEFAULT}}" + Environment.NewLine +
            "{{L_GREEN}}Update{{DEFAULT}}:{{YELLOW}}" + ToggleEnabledButton + " {{L_GREEN}}Draw{{DEFAULT}}:{{YELLOW}}" + ToggleVisibilityButton + " {{DEFAULT}}" + "{{L_GREEN}}Follow{{DEFAULT}}:{{YELLOW}}" + ToggleFollowButton + " {{DEFAULT}}" + "{{L_GREEN}}Attract{{DEFAULT}}:{{YELLOW}}" + ToggleAttractButton + " {{DEFAULT}}" + "{{L_GREEN}}Inside{{DEFAULT}}:{{YELLOW}}" + ToggleInsideButton + " {{DEFAULT}}";

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

        private RichParagraph ActiveParticles_Paragraph;

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

            _player = new Player(layer, new Vector2(400, 300));
            _player.AddComponentToTop(
                new ParticleEffectComponent(particleEffect, GameMgr.WindowManager.CanvasCenter) { Depth = 1 });

            _particleEditorEntity = new ParticleEditorEntity(
                layer,
                _player.GetComponent<PositionComponent>().Position,
                particleEffect);

            CheckPlayerAttractsParticles();
        }

        public override void Update()
        {
            base.Update();

            if (ActiveParticles_Paragraph != null)
            {
                ActiveParticles_Paragraph.Text = "Active Particles:{{YELLOW}}" + _particleEditorEntity.ParticleEffect.ActiveParticles + "{{DEFAULT}}";
            }

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
                    _particleEditorEntity.OffsetX(value);
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
                    _particleEditorEntity.OffsetY(value);
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
                    _particleEditorEntity.Speed(value);
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
        }
    }
}
