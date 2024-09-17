using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;
using System.Diagnostics;

namespace MonoGo.Samples
{
	public class GameController : Entity
	{
		public Camera2D MainCamera = new(new Vector2(1600, 900));

		public static RasterizerState DefaultRasterizer;
		public static RasterizerState WireframeRasterizer;

        private readonly Stopwatch _updateStopwatch = new();
        private readonly Stopwatch _drawStopwatch = new();
		private double _elapsedUpdate, _elapsedDraw; 

		public static RandomExt Random = new();

		public GameController() : base(SceneMgr.DefaultLayer)
		{
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60; // Fixing framerate on 60.

			MainCamera.BackgroundColor = new Color(38, 38, 38);

			GameMgr.WindowManager.CanvasSize = MainCamera.Size;
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;

			GraphicsMgr.VertexBatch.SamplerState = SamplerState.PointWrap; // Will make textures repeat without interpolation.

            DefaultRasterizer = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false
            };

            WireframeRasterizer = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.WireFrame,
                MultiSampleAntiAlias = false
            };

            GraphicsMgr.VertexBatch.RasterizerState = DefaultRasterizer;

			var cameraController = new CameraController(MainCamera);
            var switcher = new SceneSwitcher(cameraController);

			// Enabling applying postprocessing effects to separate layers.
			// Note that this will create an additional surface.
			MainCamera.PostprocessingMode = PostprocessingMode.CameraAndLayers;

			// Setting a default Font to avoid crashes when skipping the samples backwards.
            Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Default");

            SceneMgr.OnPreUpdate += OnPreUpdate;
            SceneMgr.OnPostUpdate += OnPostUpdate;
            SceneMgr.OnPreDraw += OnPreDraw; // You can do the same for individual layers or scenes.
			SceneMgr.OnPostDraw += OnPostDraw;
		}

        private void OnPreUpdate() => _updateStopwatch.Start();

        private void OnPostUpdate()
        {
            _updateStopwatch.Stop();
            _elapsedUpdate = _updateStopwatch.Elapsed.TotalMilliseconds;
            _updateStopwatch.Reset();
        }

        private void OnPreDraw() => _drawStopwatch.Start();

		private void OnPostDraw()
		{
			_drawStopwatch.Stop();
            _elapsedDraw = _drawStopwatch.Elapsed.TotalMilliseconds;
            GameMgr.WindowManager.WindowTitle = $"update: {_elapsedUpdate:0.00}ms | draw: {_elapsedDraw:0.00}ms";
			_drawStopwatch.Reset();
		}

		public override void Destroy()
		{
			base.Destroy();

			SceneMgr.OnPreUpdate -= OnPreUpdate;
			SceneMgr.OnPostUpdate -= OnPostUpdate;
            SceneMgr.OnPreDraw -= OnPreDraw;
            SceneMgr.OnPostDraw -= OnPostDraw;
        }
	}
}
