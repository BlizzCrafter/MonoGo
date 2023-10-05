using Microsoft.Xna.Framework;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.SceneSystem;

namespace Monofoxe.Extended.Engine
{
    public static class RenderMgr
    {
        public static Surface SceneSurface { get; set; }
        public static Surface GUISurface { get; set; }

        public static Matrix GUITransformMatrix = Matrix.Identity;

        public static void Initialize()
        {
            var screenWidth = GameMgr.WindowManager.CanvasWidth;
            var screenHeight = GameMgr.WindowManager.CanvasHeight;

            SceneSurface = new Surface(screenWidth, screenHeight);
            GUISurface = new Surface(screenWidth, screenHeight);

            SceneMgr.OnPreDraw += SceneMgr_OnPreDraw;
            SceneMgr.OnPostDraw += SceneMgr_OnPostDraw;
            SceneMgr.OnPreDrawGUI += SceneMgr_OnPreDrawGUI;
            SceneMgr.OnPostDrawGUI += SceneMgr_OnPostDrawGUI;
        }

        private static void SceneMgr_OnPreDraw()
        {
            Surface.SetTarget(SceneSurface, GraphicsMgr.VertexBatch.View);
            GraphicsMgr.Device.Clear(GraphicsMgr.CurrentCamera.BackgroundColor);
        }

        private static void SceneMgr_OnPreDrawGUI()
        {
            Surface.SetTarget(GUISurface);
            GraphicsMgr.Device.Clear(Color.Transparent);
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

            if (GUITransformMatrix == Matrix.Identity) GUISurface.Draw();
            else
            {
                GraphicsMgr.VertexBatch.PushViewMatrix(GUITransformMatrix);
                GUISurface.Draw();
                GraphicsMgr.VertexBatch.PopViewMatrix();
            }
        }

        public static void Destroy()
        {
            SceneSurface.Dispose();
            GUISurface.Dispose();
        }
    }
}
