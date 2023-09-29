using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.SceneSystem;

namespace Monofoxe.Extended.Engine
{
    public static class RenderMgr
    {
        public static Surface SceneSurface { get; set; }
        public static Surface GUISurface { get; set; }

        public static void Initialize()
        {
            var screenWidth = GameMgr.WindowManager.CanvasWidth;
            var screenHeight = GameMgr.WindowManager.CanvasHeight;

            SceneSurface = new Surface(screenWidth, screenHeight);
            GUISurface = new Surface(screenWidth, screenHeight);

            SceneMgr.OnPreDraw += SceneMgr_OnPreDraw;
            SceneMgr.OnPostDraw += SceneMgr_OnPostDraw;
            SceneMgr.OnPostDrawGUI += SceneMgr_OnPostDrawGUI;
        }

        private static void SceneMgr_OnPreDraw()
        {
            Surface.SetTarget(SceneSurface, GraphicsMgr.VertexBatch.View);
            GraphicsMgr.Device.Clear(GraphicsMgr.CurrentCamera.BackgroundColor);
        }

        private static void SceneMgr_OnPostDraw()
        {
            if (!Surface.SurfaceStackEmpty)
            {
                Surface.ResetTarget();
            }
        }

        private static void SceneMgr_OnPostDrawGUI()
        {
            if (!Surface.SurfaceStackEmpty)
            {
                Surface.ResetTarget();
            }

            SceneSurface.Draw();
            GUISurface.Draw();
        }

        public static void Destroy()
        {
            SceneSurface.Dispose();
            GUISurface.Dispose();
        }
    }
}
