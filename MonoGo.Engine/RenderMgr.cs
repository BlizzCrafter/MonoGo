using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;

namespace MonoGo.Engine
{
    public static class RenderMgr
    {
        public static bool PostProcessing { get; set; } = false;
        public static bool ColorGradingFX { get; set; } = true;
        public static bool BloomFX { get; set; } = true;

        public static Surface SceneSurface { get; set; }
        public static Surface GUISurface { get; set; }

        public static void Init()
        {
            UpdateResolution();

            ColorGrading.Init();
            Bloom.Init();

            SceneMgr.OnPreDraw += SceneMgr_OnPreDraw;
            SceneMgr.OnPostDraw += SceneMgr_OnPostDraw;
            SceneMgr.OnPreDrawGUI += SceneMgr_OnPreDrawGUI;
            SceneMgr.OnPostDrawGUI += SceneMgr_OnPostDrawGUI;
        }

        private static void SceneMgr_OnPreDraw()
        {
            UpdateResolution();

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

            if (PostProcessing && (ColorGradingFX || BloomFX))
            {
                ColorGrading.Process();
                Bloom.Process();
                DrawPostFXScene();
            }
            else SceneSurface.Draw();

            GUISurface.Draw();

            // Drawing the ui cursor here so that it is outside of the gui surface, because
            // the mouse cursor shouldn't be included in the gui render target.
            UISystem.DrawCursor();
        }

        private static void DrawPostFXScene()
        {
            GraphicsMgr.VertexBatch.BlendState = BlendState.Additive;

            if (ColorGradingFX) ColorGrading.Surface.Draw();
            else SceneSurface.Draw();

            if (BloomFX) Bloom.Surface.Draw();

            GraphicsMgr.VertexBatch.BlendState = BlendState.AlphaBlend;
        }

        private static void UpdateResolution()
        {
            if (SceneSurface == null || SceneSurface.Size != GameMgr.WindowManager.CanvasSize)
            {
                SceneSurface = new Surface(GameMgr.WindowManager.CanvasSize);
            }

            if (GUISurface == null || GUISurface.Size != GameMgr.WindowManager.CanvasSize)
            {
                GUISurface = new Surface(GameMgr.WindowManager.CanvasSize);
            }
        }

        public static void Dispose()
        {
            SceneSurface.Dispose();
            GUISurface.Dispose();
            ColorGrading.Dispose();
            Bloom.Dispose();
        }
    }
}
