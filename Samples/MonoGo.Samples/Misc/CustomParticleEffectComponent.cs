using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Particles;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Samples.Misc
{
    public class CustomParticleEffectComponent : ParticleEffectComponent, IHaveGUI
    {
        private Paragraph _activeParticlesParagraph;

        public CustomParticleEffectComponent(ParticleEffect particleEffect, Vector2 position) 
            : base(particleEffect, position)
        {
        }

        public override void Update()
        {
            base.Update();

            if (_activeParticlesParagraph != null)
            {
                _activeParticlesParagraph.Text =
                    "Active Particles:${FC:FFDB5F} " + ParticleEffect.ActiveParticles + "${RESET}";
            }
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
        }
    }
}
