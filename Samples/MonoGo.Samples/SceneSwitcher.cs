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
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.Utils;
using MonoGo.Engine.UI.Defs;
using System.IO;

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
        Paragraph _FPS_Paragraph;

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
            UISystem.Clear();

            var panelInvisibleStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "panel_invisible.json"));
            var panelNoPaddingStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "panel_nopadding.json"));

            /*#region PostFX Panel

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
                OnClick = (Control btn) =>
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
                OnClick = (Control btn) => { RenderMgr.PostProcessing = !RenderMgr.PostProcessing; }
            });

            #region Color Grading

            _postFXPanel.AddChild(new Button(
                "Color Grading", ButtonSkin.Default, Anchor.AutoCenter, new Vector2(300, 50))
            {
                ToggleMode = true,
                Checked = RenderMgr.ColorGradingFX,
                OnClick = (Control btn) => { RenderMgr.ColorGradingFX = !RenderMgr.ColorGradingFX; }
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
                    OnClick = (Control btn) => { ColorGrading.PreviousLUT(); image.Texture = ColorGrading.CurrentLUT[0].Texture; }
                };
                leftButton.ButtonParagraph.SetAnchorAndOffset(Anchor.AutoInlineNoBreak, Vector2.Zero);
                leftButton.AddChild(new Icon(IconType.None, Anchor.Center)
                {
                    Texture = Engine.UI.Resources.Instance.ArrowLeft
                }, true);
                var rightButton = new Button(
                    "", ButtonSkin.Alternative, Anchor.AutoInlineNoBreak, new Vector2(64, 64), new Vector2(10, 0))
                {
                    OnClick = (Control btn) => { ColorGrading.NextLUT(); image.Texture = ColorGrading.CurrentLUT[0].Texture; }
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
                OnClick = (Control btn) => { RenderMgr.BloomFX = !RenderMgr.BloomFX; }
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
                    OnClick = (Control btn) => { Bloom.PreviousPreset(); }
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
                    OnClick = (Control btn) => { Bloom.NextPreset(); }
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
                        OnValueChange = (Control control) => { Bloom.Threshold = MathF.Min(((Slider)control).Value / 100f, 0.99f); }
                    };
                    panel.AddChild(slider);
                }
                {
                    panel.AddChild(new Header("Streak"));
                    var slider = new Slider(0, 30)
                    {
                        Value = (int)((100 * Bloom.StreakLength) / Bloom.StreakLength),
                        OnValueChange = (Control control) => { Bloom.StreakLength = MathF.Min((((Slider)control).Value / 100f) * 10f, 3f); }
                    };
                    panel.AddChild(slider);
                }
            }

            #endregion Bloom

            #endregion PostFX Panel*/

            #region Bottom Panel

            var sceneDescription = Description;
            var hasDescription = CurrentFactory.Description != string.Empty;
            if (hasDescription) sceneDescription = CurrentFactory.Description;

            int panelHeight = 250;
            var isUIDemo = false;
            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                isUIDemo = true;
                panelHeight = 65;
            }

            Panel bottomPanel = new(isUIDemo ? panelInvisibleStyle : panelNoPaddingStyle)
            {
                Anchor = Anchor.BottomCenter,
                Identifier = "BottomPanel"
            };
            bottomPanel.Size.SetPixels((int)GameMgr.WindowManager.CanvasSize.X, panelHeight);
            UISystem.Root.AddChild(bottomPanel);

            _previousExampleButton = new Button($"<- ({_prevSceneButton}) Back")
            {
                Anchor = Anchor.CenterLeft
            };
            _previousExampleButton.Size.SetPixels(250, (int)bottomPanel.Size.Y.Value);
            _previousExampleButton.Events.OnClick = (Control btn) => { PreviousScene(); };
            bottomPanel.AddChild(_previousExampleButton);

            if (CurrentScene != null && !isUIDemo)
            {
                //Scene Name
                {
                    Panel descriptionPanel = new(panelInvisibleStyle)
                    {
                        Anchor = Anchor.AutoInlineLTR,
                        Identifier = "DescriptionPanel"
                    };
                    descriptionPanel.StyleSheet.Default.Padding = new Sides(5, 5, -5, 0);
                    descriptionPanel.Size.X.SetPixels((int)bottomPanel.Size.X.Value - 500);

                    _FPS_Paragraph = new Paragraph("")
                    {
                        Anchor = Anchor.TopRight
                    };
                    _FPS_Paragraph.Offset.Y.SetPixels(30);
                    descriptionPanel.AddChild(_FPS_Paragraph);

                    var title = new Title(CurrentScene.Name);
                    title.Offset.Y.SetPixels(-40);

                    descriptionPanel.AddChild(title);
                    descriptionPanel.AddChild(new HorizontalLine());
                    descriptionPanel.AddChild(new Paragraph(sceneDescription));
                    descriptionPanel.OverflowMode = OverflowMode.HideOverflow;
                    bottomPanel.AddChild(descriptionPanel);
                }
            }

            _nextExampleButton = new Button($"({_nextSceneButton}) Next ->")
            {
                Anchor = Anchor.CenterRight
            };
            _nextExampleButton.Size.SetPixels(250, (int)bottomPanel.Size.Y.Value);
            _nextExampleButton.Events.OnClick = (Control btn) => { NextScene(); };
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

            /*_postFXPanelAnimation.Update();
            if (_postFXPanelAnimation.Running)
            {
                _postFXPanel.Offset = new Vector2(
                    _postFXPanelOffsetX * (float)_postFXPanelAnimation.Progress, 0);
                _postFXButton.Offset = new Vector2(_postFXPanel.Offset.X -_postFXPanelOffsetX, 0);
            }*/

            if (Input.CheckButtonPress(_toggleUIButton))
			{
                UISystem.Root.Visible = !UISystem.Root.Visible;
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

            if (_FPS_Paragraph != null) _FPS_Paragraph.Text = "FPS: ${FC:FFFF00}" + GameMgr.FPS + "${RESET}";

            UISystem.Update();
        }

		public override void Draw()
        {
            base.Draw();

            UISystem.Draw();
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
