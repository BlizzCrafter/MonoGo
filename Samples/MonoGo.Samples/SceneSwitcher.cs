using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Demos;
using MonoGo.Samples.Misc;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGo.Samples
{
	public class SceneSwitcher : Entity
	{
        public static readonly string Description =
            "Camera > {{L_GREEN}}Move{{DEFAULT}}: {{YELLOW}}" + CameraController.UpButton + " / " + CameraController.DownButton + " / " + CameraController.LeftButton + " / " + CameraController.RightButton + "{{DEFAULT}}" + Environment.NewLine +
            "Camera > {{L_GREEN}}Rotate{{DEFAULT}}: {{YELLOW}}" + CameraController.RotateLeftButton + " / " + CameraController.RotateRightButton + " {{L_GREEN}}Zoom{{DEFAULT}}: {{YELLOW}}" + CameraController.ZoomInButton + " / " + CameraController.ZoomOutButton + "{{DEFAULT}}" + Environment.NewLine +
            "Restart:{{YELLOW}}" + _restartButton + "{{DEFAULT}} GUI:{{YELLOW}}" + _toggleUIButton + "{{DEFAULT}} Fullscreen:{{YELLOW}}" + _toggleFullscreenButton + "{{DEFAULT}} Exit:{{YELLOW}}" + _exitButton;

        Button nextExampleButton;
        Button previousExampleButton;
        RichParagraph FPS_Paragraph;

        public List<SceneFactory> Factories = new List<SceneFactory>
		{
            new SceneFactory(typeof(ParticlesDemo), ParticlesDemo.Description)
            /*new SceneFactory(typeof(UIDemo)),
            new SceneFactory(typeof(ShapeDemo)),
            new SceneFactory(typeof(PrimitiveDemo), PrimitiveDemo.Description),
            new SceneFactory(typeof(SpriteDemo)),
            new SceneFactory(typeof(InputDemo), InputDemo.Description),
            new SceneFactory(typeof(ECDemo), ECDemo.Description),
            new SceneFactory(typeof(SceneSystemDemo), SceneSystemDemo.Description),
            new SceneFactory(typeof(UtilsDemo)),
            new SceneFactory(typeof(TiledDemo), TiledDemo.Description),
            new SceneFactory(typeof(VertexBatchDemo)),
            new SceneFactory(typeof(CoroutinesDemo)),
			new SceneFactory(typeof(CollisionsDemo)),*/
        };

		public int CurrentSceneID {get; private set;} = 0;
		public Scene CurrentScene => CurrentFactory.Scene;
		public SceneFactory CurrentFactory => Factories[CurrentSceneID];

        const Buttons _prevSceneButton = Buttons.F1;
        const Buttons _nextSceneButton = Buttons.F2;
		const Buttons _restartButton = Buttons.F3;
		const Buttons _toggleUIButton = Buttons.F4;
		const Buttons _toggleFullscreenButton = Buttons.F5;
        const Buttons _exitButton = Buttons.Escape;

        CameraController _cameraController;

        public SceneSwitcher(Layer layer, CameraController cameraController) : base(layer)
		{
			_cameraController = cameraController;
        }

        public void CreateUI()
        {
            UserInterface.Active.Clear();

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

            Panel bottomPanel = new Panel(new Vector2(0, panelHeight), isUIDemo ? PanelSkin.None : PanelSkin.Default, Anchor.BottomCenter);
            bottomPanel.Identifier = "BottomPanel";
            bottomPanel.Padding = Vector2.Zero;
            UserInterface.Active.AddUIEntity(bottomPanel);

            previousExampleButton = new Button($"<- ({_prevSceneButton}) Back", ButtonSkin.Default, Anchor.CenterLeft, new Vector2(280, 0));
            previousExampleButton.OnClick = (EntityUI btn) => { PreviousScene(); };
            bottomPanel.AddChild(previousExampleButton);

            if (CurrentScene != null && !isUIDemo)
            {
                //Scene Name
                {
                    Panel descriptionPanel = new Panel(new Vector2(700, 0), PanelSkin.None, Anchor.Center);
                    descriptionPanel.Identifier = "DescriptionPanel";
                    descriptionPanel.Padding = new Vector2(10, 10);

                    FPS_Paragraph = new RichParagraph("", Anchor.TopRight);
                    descriptionPanel.AddChild(FPS_Paragraph);

                    descriptionPanel.AddChild(new Header(CurrentScene.Name, offset: new Vector2(0, -40)));
                    descriptionPanel.AddChild(new HorizontalLine());
                    descriptionPanel.AddChild(new RichParagraph(sceneDescription));
                    descriptionPanel.PanelOverflowBehavior = PanelOverflowBehavior.Clipped;
                    bottomPanel.AddChild(descriptionPanel);
                }
            }

            nextExampleButton = new Button($"Next ({_nextSceneButton}) ->", ButtonSkin.Default, Anchor.CenterRight, new Vector2(280, 0));
            nextExampleButton.OnClick = (EntityUI btn) => { NextScene(); };
            nextExampleButton.Identifier = "next_btn";
            bottomPanel.AddChild(nextExampleButton);

            // Create other GUIs last so that we don't steal input focus their.
            CurrentScene?.GetEntityList<Entity>()
                .Where(x => x is IHaveGUI)
                .Select(x => x as IHaveGUI).FirstOrDefault()?.CreateUI();
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

            if (Input.CheckButtonPress(_exitButton))
            {
                GameMgr.ExitGame();
            }

            if (FPS_Paragraph != null) FPS_Paragraph.Text = "FPS: {{YELLOW}}" + GameMgr.FPS + "{{DEFAULT}}";
            
            UserInterface.Active.Update();
        }

		public override void Draw()
        {
            base.Draw();

            UserInterface.Active.Draw();
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
