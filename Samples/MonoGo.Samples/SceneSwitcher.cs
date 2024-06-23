using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Demos;
using MonoGo.Samples.Misc;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.Utils;

namespace MonoGo.Samples
{
	public class SceneSwitcher : Entity
	{
        public static readonly string Description =
            "Camera > {{L_GREEN}}Move{{DEFAULT}}: {{YELLOW}}" + CameraController.UpButton + " / " + CameraController.DownButton + " / " + CameraController.LeftButton + " / " + CameraController.RightButton + "{{DEFAULT}}" + Environment.NewLine +
            "Camera > {{L_GREEN}}Rotate{{DEFAULT}}: {{YELLOW}}" + CameraController.RotateLeftButton + " / " + CameraController.RotateRightButton + " {{L_GREEN}}Zoom{{DEFAULT}}: {{YELLOW}}" + CameraController.ZoomInButton + " / " + CameraController.ZoomOutButton + "{{DEFAULT}}" + Environment.NewLine +
            "Restart:{{YELLOW}}" + _restartButton + "{{DEFAULT}} GUI:{{YELLOW}}" + _toggleUIButton + "{{DEFAULT}} Fullscreen:{{YELLOW}}" + _toggleFullscreenButton + "{{DEFAULT}} Exit:{{YELLOW}}" + _exitButton;

        const Buttons _prevSceneButton = Buttons.Q;
        const Buttons _nextSceneButton = Buttons.E;
        const Buttons _restartButton = Buttons.F1;
        const Buttons _toggleUIButton = Buttons.F2;
        const Buttons _toggleFullscreenButton = Buttons.F3;
        const Buttons _exitButton = Buttons.Escape;


        Panel _postFXPanel;
        Button _postFXButton;
        Animation _postFXPanelAnimation;
        bool _postFXPanelVisible = false;
        readonly int _postFXPanelOffsetX = -302;

        Button _nextExampleButton;
        Button _previousExampleButton;
        RichParagraph _FPS_Paragraph;

        public List<SceneFactory> Factories = new()
        {
            new SceneFactory(typeof(UIDemo)),
            new SceneFactory(typeof(PrimitiveDemo), PrimitiveDemo.Description),
            new SceneFactory(typeof(ShapeDemo)),
            new SceneFactory(typeof(SpriteDemo)),
            new SceneFactory(typeof(ParticlesDemo), ParticlesDemo.Description),
            new SceneFactory(typeof(InputDemo), InputDemo.Description),
            new SceneFactory(typeof(ECDemo), ECDemo.Description),
            new SceneFactory(typeof(SceneSystemDemo), SceneSystemDemo.Description),
            new SceneFactory(typeof(UtilsDemo)),
            new SceneFactory(typeof(TiledDemo), TiledDemo.Description),
            new SceneFactory(typeof(VertexBatchDemo)),
            new SceneFactory(typeof(CoroutinesDemo)),
			new SceneFactory(typeof(CollisionsDemo)),
        };

		public int CurrentSceneID { get; private set; } = 0;
		public Scene CurrentScene => CurrentFactory.Scene;
		public SceneFactory CurrentFactory => Factories[CurrentSceneID];

        CameraController _cameraController;

        public SceneSwitcher(Layer layer, CameraController cameraController) : base(layer)
		{
			_cameraController = cameraController;
        }

        public void CreateUI()
        {
            UserInterface.Active.Clear();

            #region PostFX Panel

            _postFXPanel = new(new Vector2(-_postFXPanelOffsetX, GameMgr.WindowManager.CanvasSize.Y), PanelSkin.Default, Anchor.TopRight, new Vector2(_postFXPanelOffsetX, 0))
            {
                Identifier = "PostFXPanel",
                Padding = new Vector2(0, 5)
            };
            UserInterface.Active.AddUIEntity(_postFXPanel);

            _postFXPanelAnimation = new Animation()
            {
                Easing = Easing.EaseInBounce,
                Looping = false,
                Speed = 1,
                Invert = true
            };
            _postFXPanelAnimation.AnimationEndEvent += (e) => 
            {
                _postFXPanelAnimation.Invert = !_postFXPanelAnimation.Invert;
                if (_postFXPanelAnimation.Invert)
                {
                    _postFXPanelAnimation.Easing = Easing.EaseInBounce;
                }
                else
                {
                    _postFXPanelAnimation.Easing = Easing.EaseOutBounce;
                }
            };

            _postFXButton = new Button(
                "FX", ButtonSkin.Fancy, Anchor.TopRight, new Vector2(100, 50))
            {
                OnClick = (EntityUI btn) =>
                {
                    _postFXPanelVisible = !_postFXPanelVisible;
                    if (!_postFXPanelAnimation.Running) _postFXPanelAnimation.Start(false);
                }
            };
            UserInterface.Active.AddUIEntity(_postFXButton);

            _postFXPanel.AddChild(new Header("Post FX"));
            _postFXPanel.AddChild(new HorizontalLine());
            _postFXPanel.AddChild(new Button(
                "Post Processing", ButtonSkin.Default, Anchor.AutoCenter, new Vector2(300, 50))
            {
                ToggleMode = true,
                Checked = RenderMgr.PostProcessing,
                OnClick = (EntityUI btn) => { RenderMgr.PostProcessing = !RenderMgr.PostProcessing; }
            });

            #region Color Grading

            _postFXPanel.AddChild(new Button(
                "Color Grading", ButtonSkin.Default, Anchor.AutoCenter, new Vector2(300, 50))
            {
                ToggleMode = true,
                Checked = RenderMgr.ColorGradingFX,
                OnClick = (EntityUI btn) => { RenderMgr.ColorGradingFX = !RenderMgr.ColorGradingFX; }
            });
            {
                Panel panel = new(new Vector2(_postFXPanel.Size.X, 64), PanelSkin.None, Anchor.AutoInline)
                {
                    Padding = Vector2.One
                };
                _postFXPanel.AddChild(panel);

                var image = new Image(ColorGrading.CurrentLUT[0].Texture, new Vector2(64, 64), offset: new Vector2(10, 0), anchor: Anchor.AutoInlineNoBreak)
                {
                    Padding = new Vector2(100, 0)
                };
                var leftButton = new Button(
                    "", ButtonSkin.Alternative, Anchor.AutoInlineNoBreak, new Vector2(64, 64), new Vector2(44, 0))
                {
                    OnClick = (EntityUI btn) => { ColorGrading.PreviousLUT(); image.Texture = ColorGrading.CurrentLUT[0].Texture; }
                };
                leftButton.ButtonParagraph.SetAnchorAndOffset(Anchor.AutoInlineNoBreak, Vector2.Zero);
                leftButton.AddChild(new Icon(IconType.None, Anchor.Center)
                {
                    Texture = Engine.UI.Resources.Instance.ArrowLeft
                }, true);
                var rightButton = new Button(
                    "", ButtonSkin.Alternative, Anchor.AutoInlineNoBreak, new Vector2(64, 64), new Vector2(10, 0))
                {
                    OnClick = (EntityUI btn) => { ColorGrading.NextLUT(); image.Texture = ColorGrading.CurrentLUT[0].Texture; }
                };
                rightButton.ButtonParagraph.SetAnchorAndOffset(Anchor.Center, Vector2.Zero);
                rightButton.AddChild(new Icon(IconType.None, Anchor.Center)
                {
                    Texture = Engine.UI.Resources.Instance.ArrowRight
                }, true);
                panel.AddChild(leftButton);
                panel.AddChild(image);
                panel.AddChild(rightButton);
            }

            #endregion Color Grading Panel
            #region Bloom

            _postFXPanel.AddChild(new Button(
                "Bloom", ButtonSkin.Default, Anchor.AutoCenter, new Vector2(300, 50))
            {
                ToggleMode = true,
                Checked = RenderMgr.BloomFX,
                OnClick = (EntityUI btn) => { RenderMgr.BloomFX = !RenderMgr.BloomFX; }
            });
            {
                Panel panel = new(new Vector2(_postFXPanel.Size.X, 64), PanelSkin.None, Anchor.AutoInline)
                {
                    Padding = Vector2.One
                };
                _postFXPanel.AddChild(panel);

                var leftButton = new Button(
                    "", ButtonSkin.Alternative, Anchor.AutoInlineNoBreak, new Vector2(64, 64), new Vector2(44, 0))
                {
                    OnClick = (EntityUI btn) => { Bloom.PreviousPreset(); }
                };
                leftButton.ButtonParagraph.SetAnchorAndOffset(Anchor.AutoInlineNoBreak, Vector2.Zero);
                leftButton.AddChild(new Icon(IconType.None, Anchor.Center)
                {
                    Texture = Engine.UI.Resources.Instance.ArrowLeft
                }, true);

                var image = new Image(ResourceHub.GetResource<Sprite>("GUISprites", "White_Texture")[0].Texture, new Vector2(64, 64), offset: new Vector2(10, 0), anchor: Anchor.AutoInlineNoBreak)
                {
                    Padding = new Vector2(100, 0)
                };

                var rightButton = new Button(
                    "", ButtonSkin.Alternative, Anchor.AutoInlineNoBreak, new Vector2(64, 64), new Vector2(10, 0))
                {
                    OnClick = (EntityUI btn) => { Bloom.NextPreset(); }
                };
                rightButton.ButtonParagraph.SetAnchorAndOffset(Anchor.Center, Vector2.Zero);
                rightButton.AddChild(new Icon(IconType.None, Anchor.Center)
                {
                    Texture = Engine.UI.Resources.Instance.ArrowRight
                }, true);
                panel.AddChild(leftButton);
                panel.AddChild(image);
                panel.AddChild(rightButton);

                panel.AddChild(new Header("Threshold"));
                {
                    var slider = new Slider(0, 100)
                    {
                        Value = (int)(100 * Bloom.Threshold),
                        OnValueChange = (EntityUI entity) => { Bloom.Threshold = MathF.Min(((Slider)entity).Value / 100f, 0.99f); }
                    };
                    panel.AddChild(slider);
                }
                {
                    panel.AddChild(new Header("Streak"));
                    var slider = new Slider(0, 30)
                    {
                        Value = (int)((100 * Bloom.StreakLength) / Bloom.StreakLength),
                        OnValueChange = (EntityUI entity) => { Bloom.StreakLength = MathF.Min((((Slider)entity).Value / 100f) * 10f, 3f); }
                    };
                    panel.AddChild(slider);
                }
            }

            #endregion Bloom

            #endregion PostFX Panel
            #region Bottom Panel

            var sceneDescription = Description;
            var hasDescription = CurrentFactory.Description != string.Empty;
            if (hasDescription) sceneDescription = CurrentFactory.Description;

            int panelHeight = 150;
            var isUIDemo = false;
            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                isUIDemo = true;
                panelHeight = 65;
            }

            Panel bottomPanel = new(new Vector2(GameMgr.WindowManager.CanvasSize.X, panelHeight), isUIDemo ? PanelSkin.None : PanelSkin.Default, Anchor.BottomCenter)
            {
                Identifier = "BottomPanel",
                Padding = Vector2.Zero
            };
            UserInterface.Active.AddUIEntity(bottomPanel);

            _previousExampleButton = new Button($"<- ({_prevSceneButton}) Back", ButtonSkin.Default, Anchor.CenterLeft, new Vector2(250, 0))
            {
                OnClick = (EntityUI btn) => { PreviousScene(); }
            };
            bottomPanel.AddChild(_previousExampleButton);

            if (CurrentScene != null && !isUIDemo)
            {
                //Scene Name
                {
                    Panel descriptionPanel = new(new Vector2(bottomPanel.Size.X - 500, 0), PanelSkin.None, Anchor.Center)
                    {
                        Identifier = "DescriptionPanel",
                        Padding = new Vector2(10, 10)
                    };

                    _FPS_Paragraph = new RichParagraph("", Anchor.TopRight);
                    descriptionPanel.AddChild(_FPS_Paragraph);

                    descriptionPanel.AddChild(new Header(CurrentScene.Name, offset: new Vector2(0, -40)));
                    descriptionPanel.AddChild(new HorizontalLine());
                    descriptionPanel.AddChild(new RichParagraph(sceneDescription));
                    descriptionPanel.PanelOverflowBehavior = PanelOverflowBehavior.Clipped;
                    bottomPanel.AddChild(descriptionPanel);
                }
            }

            _nextExampleButton = new Button($"Next ({_nextSceneButton}) ->", ButtonSkin.Default, Anchor.CenterRight, new Vector2(250, 0));
            _nextExampleButton.OnClick = (EntityUI btn) => { NextScene(); };
            _nextExampleButton.Identifier = "next_btn";
            bottomPanel.AddChild(_nextExampleButton);

            #endregion Bottom Panel

            // Create other GUIs last so that we don't steal input focus their.
            CurrentScene?.GetEntityList<Entity>()
                .Where(x => x is IHaveGUI)
                .Select(x => x as IHaveGUI).ToList()
                .ForEach(x => x.CreateUI());
        }

		public override void Update()
		{
			base.Update();

            _postFXPanelAnimation.Update();
            if (_postFXPanelAnimation.Running)
            {
                _postFXPanel.Offset = new Vector2(
                    _postFXPanelOffsetX * (float)_postFXPanelAnimation.Progress, 0);
                _postFXButton.Offset = new Vector2(_postFXPanel.Offset.X -_postFXPanelOffsetX, 0);
            }


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

            if (_FPS_Paragraph != null) _FPS_Paragraph.Text = "FPS: {{YELLOW}}" + GameMgr.FPS + "{{DEFAULT}}";
            
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
