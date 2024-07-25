using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Demos;
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
	public class SceneSwitcher : Entity, IHaveGUI
	{
        public static readonly string Description =
            "Camera > ${FC:96FF5F}Move${RESET}: ${FC:FFDB5F}" + CameraController.UpButton + "${RESET} / ${FC:FFDB5F}" + CameraController.DownButton + "${RESET} / ${FC:FFDB5F}" + CameraController.LeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Rotate${RESET}: ${FC:FFDB5F}" + CameraController.RotateLeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RotateRightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Zoom${RESET}: ${FC:FFDB5F}" + CameraController.ZoomInButton + "${RESET} / ${FC:FFDB5F}" + CameraController.ZoomOutButton + "${RESET}" + Environment.NewLine +
            "Demo > Restart: ${FC:FFDB5F}" + _restartButton + "${RESET} GUI: ${FC:FFDB5F}" + _toggleUIButton + "${RESET} Fullscreen: ${FC:FFDB5F}" + _toggleFullscreenButton + "${RESET} Exit: ${FC:FFDB5F}" + _exitButton;

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

        public SceneSwitcher(CameraController cameraController) : base(SceneMgr.DefaultLayer)
		{
			_cameraController = cameraController;
        }

        public void CreateUI()
        {
            if (CurrentScene == null) CurrentFactory.CreateScene();

            var panelImageStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "panel_image.json"));
            var listPanelCentered = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "list_panel_centered.json"));
            var listItemCentered = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "list_item_centered.json"));

            #region PostFX Panel

            _postFXPanel = new()
            {
                Identifier = "PostFXPanel",
                Anchor = Anchor.TopRight
            };
            _postFXPanel.Size.SetPixels(-_postFXPanelOffsetX, (int)GameMgr.WindowManager.CanvasSize.Y);
            _postFXPanel.Offset.X.SetPixels(_postFXPanelOffsetX);
            _postFXPanel.StyleSheet.Default.Padding = new Sides(0, 0, 5, 5);
            UISystem.Add(_postFXPanel);

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

            _postFXButton = new Button("FX")
            {
                Anchor = Anchor.TopRight
            };
            _postFXButton.Size.SetPixels(100, 50);
            _postFXButton.Events.OnClick = (Control control) =>
            {
                _postFXPanelVisible = !_postFXPanelVisible;
                if (!_postFXPanelAnimation.Running) _postFXPanelAnimation.Start(false);
            };
            UISystem.Add(_postFXButton);

            _postFXPanel.AddChild(new Title("Post FX") { Anchor = Anchor.AutoCenter });
            _postFXPanel.AddChild(new HorizontalLine());
            var postFXEnableButton = new Button("Post Processing")
            {
                Anchor = Anchor.AutoCenter,
                ToggleCheckOnClick = true,
                Checked = RenderMgr.PostProcessing
            };
            postFXEnableButton.Size.SetPixels(300, 50);
            postFXEnableButton.Events.OnClick = (Control control) =>
            {
                RenderMgr.PostProcessing = !RenderMgr.PostProcessing;
            };
            _postFXPanel.AddChild(postFXEnableButton);

            #region Color Grading
            
            var colorGradingEnableButton = new Button("Color Grading")
            {
                Anchor = Anchor.AutoCenter,
                ToggleCheckOnClick = true,
                Checked = RenderMgr.ColorGradingFX
            };
            colorGradingEnableButton.Size.SetPixels(300, 50);
            colorGradingEnableButton.Events.OnClick = (Control control) =>
            {
                RenderMgr.ColorGradingFX = !RenderMgr.ColorGradingFX;
            };
            _postFXPanel.AddChild(colorGradingEnableButton);
            
            {
                Panel panel = new(null!)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                //panel.StyleSheet.Default.Padding = new Sides(1, 1, 1, 1);
                panel.Size.SetPixels((int)_postFXPanel.Size.X.Value, 64);
                _postFXPanel.AddChild(panel);

                var logo = new Panel(panelImageStyle.DeepCopy())
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                var logoTexture = ColorGrading.CurrentLUT[0].Texture;
                logo.StyleSheet.Default.Icon.Texture = logoTexture;
                logo.Size.SetPixels(64, 64);
                logo.StyleSheet.Default.Icon.SourceRect = new Rectangle(0, 0, 64, 64);
                logo.Offset.X.SetPixels(10);

                var leftButton = new Button("P")
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                leftButton.Size.SetPixels(64, 64);
                leftButton.Offset.X.SetPixels(44);
                leftButton.Events.OnClick = (Control control) =>
                {
                    ColorGrading.PreviousLUT(); 
                    logo.StyleSheet.Default.Icon.Texture = ColorGrading.CurrentLUT[0].Texture;
                };

                var rightButton = new Button("N")
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                rightButton.Size.SetPixels(64, 64);
                rightButton.Offset.X.SetPixels(10);
                rightButton.Events.OnClick = (Control control) =>
                {
                    ColorGrading.NextLUT(); 
                    logo.StyleSheet.Default.Icon.Texture = ColorGrading.CurrentLUT[0].Texture;
                };

                panel.AddChild(leftButton);
                panel.AddChild(logo);
                panel.AddChild(rightButton);
            }

            #endregion Color Grading Panel
            #region Bloom

            var bloomEnableButton = new Button("Bloom")
            {
                Anchor = Anchor.AutoCenter,
                ToggleCheckOnClick = true,
                Checked = RenderMgr.BloomFX
            };
            bloomEnableButton.Size.SetPixels(300, 50);
            bloomEnableButton.Events.OnClick = (Control control) =>
            {
                RenderMgr.BloomFX = !RenderMgr.BloomFX;
            };
            _postFXPanel.AddChild(bloomEnableButton);

            {
                Panel panel = new(null!)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                //panel.StyleSheet.Default.Padding = new Sides(1, 1, 1, 1);
                panel.Size.SetPixels((int)_postFXPanel.Size.X.Value, 64);
                _postFXPanel.AddChild(panel);

                var logo = new Panel(panelImageStyle.DeepCopy())
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                var logoTexture = ResourceHub.GetResource<Sprite>("ParticleSprites", "Pixel")[0].Texture;
                logo.StyleSheet.Default.Icon.Texture = logoTexture;
                logo.Size.SetPixels(64, 64);
                logo.StyleSheet.Default.Icon.SourceRect = new Rectangle(0, 0, 64, 64);
                logo.Offset.X.SetPixels(10);

                var leftButton = new Button("P")
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                leftButton.Size.SetPixels(64, 64);
                leftButton.Offset.X.SetPixels(44);
                leftButton.Events.OnClick = (Control control) =>
                {
                    Bloom.PreviousPreset();
                };

                var rightButton = new Button("N")
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                rightButton.Size.SetPixels(64, 64);
                rightButton.Offset.X.SetPixels(10);
                rightButton.Events.OnClick = (Control control) =>
                {
                    Bloom.NextPreset();
                };

                panel.AddChild(leftButton);
                panel.AddChild(logo);
                panel.AddChild(rightButton);

                panel.AddChild(new Title("Threshold") { Anchor = Anchor.AutoCenter });
                {
                    var slider = new Slider()
                    {
                        MinValue = 0,
                        MaxValue = 100,
                        Value = (int)(100 * Bloom.Threshold)
                    };
                    slider.Events.OnValueChanged = (Control control) =>
                    {
                        Bloom.Threshold = MathF.Min(((Slider)control).Value / 100f, 0.99f);
                    };
                    panel.AddChild(slider);
                }
                {
                    panel.AddChild(new Title("Streak") { Anchor = Anchor.AutoCenter });
                    var slider = new Slider()
                    {
                        MinValue = 0,
                        MaxValue = 30,
                        Value = (int)((10 * Bloom.StreakLength) / Bloom.StreakLength)
                    };
                    slider.Events.OnValueChanged = (Control control) =>
                    {
                        Bloom.StreakLength = MathF.Min((((Slider)control).Value / 100f) * 10f, 3f);
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

            int panelHeight = 250;
            var isUIDemo = false;
            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                isUIDemo = true;
                panelHeight = 65;
            }

            Panel bottomPanel = new(isUIDemo ? null! : UISystem.DefaultStylesheets.Panels.DeepCopy())
            {
                Anchor = Anchor.BottomCenter,
                Identifier = "BottomPanel"
            };
            if (!isUIDemo) bottomPanel.StyleSheet.Default.Padding = Sides.Zero;
            bottomPanel.Size.SetPixels((int)GameMgr.WindowManager.CanvasSize.X, panelHeight);
            UISystem.Add(bottomPanel);

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
                    Panel descriptionPanel = new(null!)
                    {
                        Anchor = Anchor.AutoInlineLTR,
                        Identifier = "DescriptionPanel"
                    };
                    descriptionPanel.Size.X.SetPixels((int)bottomPanel.Size.X.Value - 500);

                    _FPS_Paragraph = new Paragraph("")
                    {
                        Anchor = Anchor.TopRight
                    };
                    _FPS_Paragraph.Offset.Y.SetPixels(30);
                    descriptionPanel.AddChild(_FPS_Paragraph);

                    var title = new Title(CurrentScene.Name) { Anchor = Anchor.AutoCenter };
                    title.Offset.Y.SetPixels(-40);

                    descriptionPanel.AddChild(title);
                    descriptionPanel.AddChild(new HorizontalLine());
                    descriptionPanel.AddChild(new Paragraph(sceneDescription));
                    descriptionPanel.OverflowMode = OverflowMode.HideOverflow;
                    bottomPanel.AddChild(descriptionPanel);
                }
            }
            else if (isUIDemo)
            {
                UISystem.Root.Walk(
                    x =>
                    {
                        if (x.Identifier == "Top Panel")
                        {
                            DropDown themeDropDown = new(listPanelCentered, listItemCentered)
                            {
                                Identifier = "Theme Switcher",
                                Anchor = Anchor.TopCenter,
                                AllowDeselect = false,
                                AutoHeight = true
                            };
                            themeDropDown.Size.SetPixels(240, (int)x.Size.Y.Value);
                            foreach (string theme in UISystem.ThemeFolders)
                            {
                                themeDropDown.AddItem(theme);
                            }
                            themeDropDown.SelectedValue = UISystem.ThemeActiveName;
                            themeDropDown.Events.OnValueChanged = (Control control) =>
                            {
                                UISystem.LoadTheme(themeDropDown.SelectedValue);
                                RestartScene();
                            };
                            x.AddChild(themeDropDown);

                            return false;

                        }
                        return true;
                    });
            }

            _nextExampleButton = new Button($"({_nextSceneButton}) Next ->")
            {
                Anchor = Anchor.CenterRight
            };
            _nextExampleButton.Size.SetPixels(250, (int)bottomPanel.Size.Y.Value);
            _nextExampleButton.Events.OnClick = (Control btn) => { NextScene(); };
            bottomPanel.AddChild(_nextExampleButton);

            #endregion Bottom Panel
        }

		public override void Update()
		{
			base.Update();

            if (_postFXPanel != null)
            {
                _postFXPanelAnimation.Update();
                if (_postFXPanelAnimation.Running)
                {
                    _postFXPanel.Offset.X.SetPixels(_postFXPanelOffsetX * (float)_postFXPanelAnimation.Progress);
                    _postFXButton.Offset.X.SetPixels(_postFXPanel.Offset.X.Value - _postFXPanelOffsetX);
                }
            }

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

            ((IHaveGUI)this).Init();
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

            ((IHaveGUI)this).Init();
        }

		public void RestartScene()
		{
            CurrentFactory.RestartScene();
			_cameraController.Reset();

            ((IHaveGUI)this).Init();
        }
    }
}
