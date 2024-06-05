using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.UI;
using MonoGo.Samples.Misc;
using System;

namespace MonoGo.Samples.Demos
{
    public class ParticlesDemo : Entity, IHaveGUI
    {
        public static readonly string Description =
            "Move > {{YELLOW}}WASD{{DEFAULT}}" + Environment.NewLine +
            "Particles > {{L_GREEN}}Update{{DEFAULT}}:{{YELLOW}}" + ToggleEnabledButton + " {{L_GREEN}}Draw{{DEFAULT}}:{{YELLOW}}" + ToggleVisibilityButton + " {{DEFAULT}}" + "{{L_GREEN}}Follow{{DEFAULT}}:{{YELLOW}}" + ToggleFollowEntityButton + " {{DEFAULT}}";

        public const Buttons ToggleVisibilityButton = Buttons.N;
        public const Buttons ToggleEnabledButton = Buttons.M;
        public const Buttons ToggleFollowEntityButton = Buttons.F;

        private Player _player;
        private ParticleFX _particleFX;

        private RichParagraph ActiveParticles_Paragraph;

        public ParticlesDemo(Layer layer) : base(layer)
        {
            _player = new Player(layer, new Vector2(400, 300))
            {
                Depth = 0
            };

            _particleFX = new ParticleFX(layer, GameMgr.WindowManager.CanvasCenter)
            {
                Depth = 1,
                FollowEntity = _player
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
                foreach (var entity in Layer.GetEntityList<ParticleFX>())
                {
                    if (entity.FollowEntity == null)
                    {
                        entity.FollowEntity = _player;
                    }
                    else entity.FollowEntity = null;
                }
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
            Panel descriptionPanel = new(new Vector2(0, 60), PanelSkin.None, Anchor.TopCenter);

            ActiveParticles_Paragraph = new RichParagraph("", Anchor.Center);
            descriptionPanel.AddChild(ActiveParticles_Paragraph);

            UserInterface.Active.AddUIEntity(descriptionPanel);
        }
    }
}
