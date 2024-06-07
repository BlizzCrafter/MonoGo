using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.UI.Entities.TextValidators;
using MonoGo.Samples.Misc;
using System;

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

        private Player _player;
        private ParticleFX _particleFX;
        private bool _particlesFollowPlayer = true;

        private RichParagraph ActiveParticles_Paragraph;

        public ParticlesDemo(Layer layer) : base(layer)
        {
            _player = new Player(layer, new Vector2(400, 300))
            {
                Depth = 0
            };

            _particleFX = new ParticleFX(layer, _player.GetComponent<PositionComponent>(), GameMgr.WindowManager.CanvasCenter)
            {
                Depth = 1
            };
            layer.DepthSorting = true;
            layer.ReorderEntity(_player, 0);
            layer.ReorderEntity(_particleFX, 1);
        }

        public override void Update()
        {
            base.Update();

            if (ActiveParticles_Paragraph != null)
            {
                ActiveParticles_Paragraph.Text = "Active Particles:{{YELLOW}}" + _particleFX.ParticleEffect.ActiveParticles + "{{DEFAULT}}";
            }

            if (Input.CheckButtonPress(ToggleFollowEntityButton))
            {
                _particlesFollowPlayer = !_particlesFollowPlayer;
            }

            if (_particlesFollowPlayer)
            {
                foreach (var entity in Layer.GetEntityList<ParticleFX>())
                {
                    if (entity.TryGetComponent(out PositionComponent posComponent))
                    {
                        posComponent.Position = _player.GetComponent<PositionComponent>().Position;
                    }
                }
            }
            else
            {
                foreach (var entity in Layer.GetEntityList<ParticleFX>())
                {
                    if (entity.TryGetComponent(out PositionComponent posComponent))
                    {
                        posComponent.Position = posComponent.StartingPosition;
                    }
                }
            }

            if (Input.CheckButtonPress(ToggleInsideButton))
            {
                _particleFX.Inside();
            }

            if (Input.CheckButtonPress(ToggleVisibilityButton))
            {
                foreach (var entity in Layer.GetEntityList<ParticleFX>())
                {
                    entity.Visible = !entity.Visible;
                }
            }

            if (Input.CheckButtonPress(ToggleEnabledButton))
            {
                foreach (var entity in Layer.GetEntityList<ParticleFX>())
                {
                    entity.Enabled = !entity.Enabled;
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
                    _particleFX.OffsetX(value);
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
                    _particleFX.OffsetY(value);
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
                    _particleFX.Speed(value);
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
        }
    }
}
