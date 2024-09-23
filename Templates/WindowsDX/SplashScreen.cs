using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;
using System.Reflection;

namespace MGNamespace
{
    public class SplashScreen : Entity, IHaveGUI
    {
        private CameraController _cameraController;

        public SplashScreen(CameraController cameraController) : base(SceneMgr.DefaultLayer)
        {
            _cameraController = cameraController;
        }

        public void CreateUI()
        {
            var panel = new Panel(null!)
            {
                Anchor = Anchor.Center,
                AutoHeight = true,
                OverflowMode = OverflowMode.HideOverflow
            };
            UISystem.Add(panel);

            var welcomeText = new Paragraph(@$"MonoGo Engine ${{FC:FFDB5F}}v.{Assembly.GetAssembly(typeof(Entity)).GetName().Version}${{RESET}}");
            welcomeText.OverrideStyles.FontSize = 28;
            panel.AddChild(welcomeText);
        }
    }
}
