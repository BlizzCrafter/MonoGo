using System.Linq;
using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Samples.Misc
{
    public class ParticleEditorEntity : Entity, IHaveGUI
    {
        public ParticleEffectComponent ParticleEffectComponent { get; set; }

        private Paragraph _activeParticlesParagraph;

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
                    "Active Particles:${FC:FFDB5F} " + ParticleEffectComponent.ParticleEffect.ActiveParticles + "${RESET}";
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
            var topPanel = new Panel(null!)
            {
                Anchor = Anchor.TopCenter,
                IgnoreInteractions = true
            };
            topPanel.Size.Y.SetPixels(60);
            UISystem.Add(topPanel);

            _activeParticlesParagraph = new Paragraph("")
            {
                Anchor = Anchor.Center
            };
            topPanel.AddChild(_activeParticlesParagraph);

            //var bottomPanel = UISystem.Find("BottomPanel");
            //bottomPanel.Size = new Vector2(0, 180);

            // left panel
            {
                var panel = new Panel(null!)
                {
                    Anchor = Anchor.AutoLTR
                };
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput();
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Control control) =>
                {
                    OffsetX((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label("Offset: X"));
            }
            // center panel
            {
                var panel = new Panel(null!)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Control control) =>
                {
                    OffsetY((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label("Offest:Y"));
            }
            // right panel
            {
                var panel = new Panel(null!)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Control control) =>
                {
                    Speed((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label("Speed"));
            }
        }

        public void OnCreated(IHaveGUI owner)
        {
        }
    }
}
