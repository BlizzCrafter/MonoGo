using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.Particles;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;
using System.Linq;

namespace MonoGo.Samples.Misc
{
    public class ParticleEditorEntity : Entity, IHaveGUI
    {
        public ParticleEffectComponent ParticleEffectComponent { get; set; }

        public ParticleEditorEntity(Layer layer) : base(layer) 
        {
            Visible = false;
        }

        public void OffsetX(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(value, x.Offset.Y));
        }

        public void OffsetY(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(x.Offset.X, value));
        }

        public void Speed(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Speed = value);
        }

        public void ToggleInside()
        {
            var player = Layer.FindEntity<Player>();
            player.InsideParticles = !player.InsideParticles;

            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Inside = player.InsideParticles);
        }

        private void ToggleAttract()
        {
            var player = Layer.FindEntity<Player>();
            player.AttractParticles = !player.AttractParticles;

            CheckParticleAttraction(player);
        }

        private void CheckParticleAttraction(Player player)
        {
            if (player.AttractParticles)
            {
                ParticleEffectComponent.AttractParticlesTo(player.GetComponent<PositionComponent>());
            }
            else
            {
                ParticleEffectComponent.AttractParticlesTo(null);
            }
        }

        public void CreateUI()
        {
            var player = Layer.FindEntity<Player>();
            ParticleEffectComponent = player.GetComponent<CustomParticleEffectComponent>();
            CheckParticleAttraction(player);

            // Controls
            var firstPanel = new Panel(null!)
            {
                Anchor = Anchor.AutoLTR,
                AutoHeight = false,
                OverflowMode = OverflowMode.AllowOverflow
            };
            firstPanel.Size.X.SetPercents(100);
            firstPanel.Size.Y.SetPixels(64);
            SceneSwitcher.DescriptionPanel.AddChild(firstPanel);
            {
                var checkbox = new Checkbox("Enabled") { Anchor = Anchor.AutoLTR, Checked = ParticleEffectComponent.Enabled };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Control control) =>
                {
                    ParticleEffectComponent.Enabled = !ParticleEffectComponent.Enabled;
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox("Visible") { Anchor = Anchor.AutoInlineLTR, Checked = ParticleEffectComponent.Visible };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Control control) =>
                {
                    ParticleEffectComponent.Visible = !ParticleEffectComponent.Visible;
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox("Follow") { Anchor = Anchor.AutoInlineLTR, Checked = ParticleEffectComponent.FollowOwner };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Control control) =>
                {
                    ParticleEffectComponent.ToggleFollowOwner();
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox("Attract") { Anchor = Anchor.AutoInlineLTR, Checked = player.AttractParticles };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Control control) =>
                {
                    ToggleAttract();
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox("Inside") { Anchor = Anchor.AutoInlineLTR, Checked = player.InsideParticles };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Control control) =>
                {
                    ToggleInside();
                };
                firstPanel.AddChild(checkbox);
            }

            // OFFSET, SPEED
            // left panel
            {
                var panel = new Panel(null!)
                {
                    Anchor = Anchor.AutoLTR
                };
                panel.OverrideStyles.MarginBefore = Point.Zero;
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
    }
}
