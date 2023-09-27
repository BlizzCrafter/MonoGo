using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Extended.Engine;
using Monofoxe.Extended.Engine.Cameras;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.EC;
using Monofoxe.Extended.Engine.Resources;
using Monofoxe.Extended.Engine.SceneSystem;
using Monofoxe.Extended.Samples.Demos;
using Monofoxe.Extended.Samples.Misc;
using Monofoxe.Extended.UI;
using Monofoxe.Extended.UI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monofoxe.Extended.Samples
{
	public class SceneSwitcher : Entity
	{
        public static readonly string Description =
            "Camera > {{L_GREEN}}Move{{DEFAULT}}: {{YELLOW}}" + CameraController.UpButton + " / " + CameraController.DownButton + " / " + CameraController.LeftButton + " / " + CameraController.RightButton + "{{DEFAULT}}" + Environment.NewLine +
            "Camera > {{L_GREEN}}Rotate{{DEFAULT}}: {{YELLOW}}" + CameraController.RotateLeftButton + " / " + CameraController.RotateRightButton + " {{L_GREEN}}Zoom{{DEFAULT}}: {{YELLOW}}" + CameraController.ZoomInButton + " / " + CameraController.ZoomOutButton + "{{DEFAULT}}" + Environment.NewLine +
            "Restart > {{YELLOW}}" + _restartButton + "{{DEFAULT}}";

        SpriteBatch _spriteBatch;

        Button nextExampleButton;
        Button previousExampleButton;
        RichParagraph FPS_Paragraph;

        public List<SceneFactory> Factories = new List<SceneFactory>
		{
            new SceneFactory(typeof(UIDemo)),
			new SceneFactory(typeof(ShapeDemo)),
			new SceneFactory(typeof(PrimitiveDemo), PrimitiveDemo.Description),
			new SceneFactory(typeof(SpriteDemo)),
			new SceneFactory(typeof(InputDemo), InputDemo.Description),
			new SceneFactory(typeof(ECDemo), ECDemo.Description),
			new SceneFactory(typeof(SceneSystemDemo), SceneSystemDemo.Description),
			new SceneFactory(typeof(UtilsDemo)),
			new SceneFactory(typeof(TiledDemo), TiledDemo.Description),
            
            //Currently DEACTIVATED because of color bugs
			//
            //new SceneFactory(typeof(VertexBatchDemo)),
			//new SceneFactory(typeof(CoroutinesDemo)),
            //
		};

		public int CurrentSceneID {get; private set;} = 0;
		public Scene CurrentScene => CurrentFactory.Scene;
		public SceneFactory CurrentFactory => Factories[CurrentSceneID];
        public Surface CurrentSurface => CurrentScene?.GetEntityList<SurfaceEntity>().First().RenderTargetSurface;

        int _barHeight = 64;
		Color _barColor = Color.Black * 0.5f;
		Color _textColor = Color.White;

		Vector2 _indent = new Vector2(8, 4);

		const Buttons _nextSceneButton = Buttons.E;
		const Buttons _prevSceneButton = Buttons.Q;
		const Buttons _restartButton = Buttons.R;
		const Buttons _toggleUIButton = Buttons.T;
		const Buttons _toggleFullscreenButton = Buttons.F;

		CameraController _cameraController;

        public SceneSwitcher(Layer layer, CameraController cameraController) : base(layer)
		{
			_cameraController = cameraController;
            _spriteBatch = new SpriteBatch(GraphicsMgr.Device);

            CreateUI();
        }

        public void CreateUI()
        {
            UserInterface.Active.Clear();

            CurrentScene?.GetEntityList<Entity>()
                .Where(x => x is IGuiEntity)
                .Select(x => x as IGuiEntity).FirstOrDefault()?.CreateUI();

            var sceneDescription = Description;
            var hasDescription = CurrentFactory.Description != string.Empty;
            if (hasDescription) sceneDescription = CurrentFactory.Description;

            int panelHeight = 180;
            var isUIDemo = false;
            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                isUIDemo = true;
                panelHeight = 65;
            }

            Panel menuPanel = new Panel(new Vector2(0, panelHeight), isUIDemo ? PanelSkin.None : PanelSkin.Default, Anchor.BottomCenter);
            menuPanel.Padding = Vector2.Zero;
            UserInterface.Active.AddEntity(menuPanel);

            previousExampleButton = new Button($"<- ({_prevSceneButton}) Back", ButtonSkin.Default, Anchor.CenterLeft, new Vector2(280, 0));
            previousExampleButton.OnClick = (EntityUI btn) => { PreviousScene(); };
            menuPanel.AddChild(previousExampleButton);

            if (CurrentScene != null && !isUIDemo)
            {
                //Scene Name
                {
                    Panel descriptionPanel = new Panel(new Vector2(700, 0), PanelSkin.None, Anchor.Center);

                    FPS_Paragraph = new RichParagraph("", Anchor.TopRight);
                    descriptionPanel.AddChild(FPS_Paragraph);

                    descriptionPanel.AddChild(new Header(CurrentScene.Name, offset: new Vector2(0, -40)));
                    descriptionPanel.AddChild(new HorizontalLine());
                    descriptionPanel.AddChild(new RichParagraph(sceneDescription));
                    descriptionPanel.PanelOverflowBehavior = PanelOverflowBehavior.Clipped;
                    menuPanel.AddChild(descriptionPanel);
                }
            }

            nextExampleButton = new Button($"Next ({_nextSceneButton}) ->", ButtonSkin.Default, Anchor.CenterRight, new Vector2(280, 0));
            nextExampleButton.OnClick = (EntityUI btn) => { NextScene(); };
            nextExampleButton.Identifier = "next_btn";
            menuPanel.AddChild(nextExampleButton);
        }

		public override void Update()
		{
			base.Update();

			if (Input.CheckButtonPress(_toggleUIButton))
			{
                UserInterface.Active.Root.Visible = !UserInterface.Active.Root.Visible;
            }

			if (Input.CheckButtonPress(_restartButton))
			{
				RestartScene();
			}

			if (Input.CheckButtonPress(_nextSceneButton))
			{
				NextScene();
			}

			if (Input.CheckButtonPress(_prevSceneButton))
			{
				PreviousScene();
			}

			if (Input.CheckButtonPress(_toggleFullscreenButton))
			{
				GameMgr.WindowManager.ToggleFullScreen();
			}

            if (FPS_Paragraph != null) FPS_Paragraph.Text = "FPS: {{YELLOW}}" + GameMgr.Fps + "{{DEFAULT}}";
            UserInterface.Active.Update();
        }

		public override void Draw()
        {
            base.Draw();

            UserInterface.Active.Draw(_spriteBatch);

            CurrentSurface.Draw();

            if (UserInterface.Active.UseRenderTarget)
            {
                UserInterface.Active.DrawMainRenderTarget(_spriteBatch);
            }

            //var canvasSize = GameMgr.WindowManager.CanvasSize;

            //Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Arial");
            //Text.HorAlign = TextAlign.Left;
            //Text.VerAlign = TextAlign.Top;

            //// Description.
            //if (CurrentFactory.Description != "")
            //{
            //	var padding = 8;
            //	var textSize = Text.CurrentFont.MeasureString(CurrentFactory.Description);
            //	var origin = Vector2.UnitX * (canvasSize - (textSize + Vector2.One * padding * 2));
            //	GraphicsMgr.CurrentColor = _barColor;
            //	RectangleShape.Draw(origin, origin + textSize + Vector2.One * padding * 2, false);
            //	GraphicsMgr.CurrentColor = _textColor;
            //	Text.Draw(CurrentFactory.Description, Vector2.One * padding + origin);
            //}
            //// Description.


            //// Bottom bar.
            //GraphicsMgr.VertexBatch.PushViewMatrix();
            //GraphicsMgr.VertexBatch.View =
            //	Matrix.CreateTranslation(new Vector3(0, canvasSize.Y - _barHeight, 0)) * GraphicsMgr.VertexBatch.View;

            //GraphicsMgr.CurrentColor = _barColor;
            //RectangleShape.Draw(Vector2.Zero, canvasSize, false);

            //GraphicsMgr.CurrentColor = _textColor;
            //Text.Draw(
            //    "fps: " + GameMgr.Fps
            //    + " | Current scene: " + CurrentScene.Name
            //    + Environment.NewLine
            //    + _prevSceneButton + "/" + _nextSceneButton + " - change scene, "
            //    + _restartButton + " - restart current scene, "
            //    + _toggleUIButton + " - toggle UI, "
            //    + _toggleFullscreenButton + " - toggle fullscreen"

            //    + Environment.NewLine
            //    + CameraController.UpButton + "/"
            //    + CameraController.DownButton + "/"
            //    + CameraController.LeftButton + "/"
            //    + CameraController.RightButton + " - move camera, "
            //    + CameraController.ZoomInButton + "/" + CameraController.ZoomOutButton + " - zoom, "
            //    + CameraController.RotateLeftButton + "/" + CameraController.RotateRightButton + " - rotate"
            //    ,
            //    _indent
            //);

            //GraphicsMgr.VertexBatch.PopViewMatrix();
            //// Bottom bar.
        }


		public void NextScene()
        {
            CurrentFactory.DestroyScene();

			CurrentSceneID += 1;
			if (CurrentSceneID >= Factories.Count)
			{
				CurrentSceneID = 0;
			}

			CurrentFactory.CreateScene();

			_cameraController.Reset();

            CreateUI();
        }


		public void PreviousScene()
		{
            CurrentFactory.DestroyScene();

			CurrentSceneID -= 1;
			if (CurrentSceneID < 0)
			{
				CurrentSceneID = Factories.Count - 1;
			}

			CurrentFactory.CreateScene();

			_cameraController.Reset();

            CreateUI();
        }


		public void RestartScene()
		{
            CurrentFactory.RestartScene();
			_cameraController.Reset();

            CreateUI();
        }
    }
}
