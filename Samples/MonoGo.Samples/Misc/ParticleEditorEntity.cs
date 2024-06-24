using System.Linq;
using Microsoft.Xna.Framework;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.UI.Entities.TextValidators;

namespace MonoGo.Samples.Misc
{
    public class ParticleEditorEntity : Entity, IHaveGUI
    {
        public ParticleEffectComponent ParticleEffectComponent { get; set; }

        private RichParagraph _activeParticlesParagraph;

        public ParticleEditorEntity(
            Layer layer,
            ParticleEffectComponent particleEffectComponent) 
            : base(layer)
        {
            Visible = false;

            ParticleEffectComponent = particleEffectComponent;
        }

        public override void Update()
        {
            base.Update();

            if (_activeParticlesParagraph != null)
            {
                _activeParticlesParagraph.Text = 
                    "Active Particles:{{YELLOW}}" + ParticleEffectComponent.ParticleEffect.ActiveParticles + "{{DEFAULT}}";
            }
        }

        public void OffsetX(float value)
        {
            var player = Layer.FindEntity<Player>();
            var particleEffectComponent = player.GetComponent<ParticleEffectComponent>();
            particleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(value, x.Offset.Y));
        }

        public void OffsetY(float value)
        {
            var player = Layer.FindEntity<Player>();
            var particleEffectComponent = player.GetComponent<ParticleEffectComponent>();
            particleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(x.Offset.X, value));
        }

        public void Speed(float value)
        {
            var player = Layer.FindEntity<Player>();
            var particleEffectComponent = player.GetComponent<ParticleEffectComponent>();
            particleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Speed = value);
        }

        public void ToggleInside()
        {
            var player = Layer.FindEntity<Player>();
            var particleEffectComponent = player.GetComponent<ParticleEffectComponent>();
            particleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Inside = !x.Inside);
        }

        public void CreateUI()
        { 
            var topPanel = new Panel(new Vector2(0, 60), PanelSkin.None, Anchor.TopCenter);
            _activeParticlesParagraph = new RichParagraph("", Anchor.Center);
            topPanel.ClickThrough = true;
            topPanel.AddChild(_activeParticlesParagraph);

            UserInterface.Active.AddUIEntity(topPanel);

            var bottomPanel = UserInterface.Active.Root.Find("BottomPanel", true);
            bottomPanel.Size = new Vector2(0, 180);

            var descriptionPanel = UserInterface.Active.Root.Find("DescriptionPanel", true);
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Offest:X=0";
                textInput.OnValueChange += (EntityUI entityUI) =>
                {
                    if (float.TryParse(entityUI.GetValue().ToString(), out float value))
                    {
                        OffsetX(value); 
                    }
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Offest:Y=0";
                textInput.OnValueChange += (EntityUI entityUI) =>
                {
                    if (float.TryParse(entityUI.GetValue().ToString(), out float value))
                    {
                        OffsetY(value);
                    }
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
            {
                var textInput = new TextInput(false, new Vector2(170, 50), anchor: Anchor.AutoInline, skin: PanelSkin.ListBackground);
                textInput.PlaceholderText = "Speed=1";
                textInput.OnValueChange += (EntityUI entityUI) =>
                {
                    if (float.TryParse(entityUI.GetValue().ToString(), out float value))
                    {
                        Speed(value);
                    }
                };
                textInput.Validators.Add(new NumbersOnly(true));
                descriptionPanel.AddChild(textInput);
            }
        }
    }
}
