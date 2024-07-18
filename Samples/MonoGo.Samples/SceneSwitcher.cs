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
            //new SceneFactory(typeof(UIDemo)),
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

            // load some alt stylesheets that are not loaded by default from the system stylesheet
            var hProgressBarAltStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeFolder, "Styles", "progress_bar_horizontal_alt.json"));
            var hProgressBarAltFillStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeFolder, "Styles", "progress_bar_horizontal_alt_fill.json"));
            var panelTitleStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeFolder, "Styles", "panel_title.json"));

            // create button for enable/ disable debug mode
            {
                Button enableDebugModeBtn = new Button("Debug Mode")
                {
                    ToggleCheckOnClick = true,
                    Anchor = Anchor.TopRight
                };
                enableDebugModeBtn.Events.OnValueChanged = (EntityUI entity) =>
                {
                    UISystem.DebugRenderEntities = enableDebugModeBtn.Checked;
                };
                enableDebugModeBtn.Size.X.SetPixels(200);
                UISystem.Root.AddChild(enableDebugModeBtn);
            }

            // create button to open git repo
            {
                Button openGitRepoBtn = new Button("Git Repo")
                {
                    ToggleCheckOnClick = true,
                    Anchor = Anchor.TopLeft
                };
                openGitRepoBtn.Events.OnValueChanged = (EntityUI entity) =>
                {
                };
                openGitRepoBtn.Size.X.SetPixels(200);
                UISystem.Root.AddChild(openGitRepoBtn);
            }

            // create panel for demo buttons
            Panel demoSelection;
            {
                // create panel to select demo
                demoSelection = new Panel();
                demoSelection.Anchor = Anchor.CenterLeft;
                demoSelection.AutoWidth = true;
                demoSelection.AutoHeight = true;
                UISystem.Root.AddChild(demoSelection);

                // add title and horizontal line
                demoSelection.AddChild(new Title("Demo Selection:")
                {
                    Anchor = Anchor.AutoCenter
                });
                demoSelection.AddChild(new HorizontalLine());
            }

            // create container for a demo example
            List<Panel> _demoPanels = new();
            Panel CreateDemoContainer(string demoTitle, Point size)
            {
                // create panel
                bool isFirst = _demoPanels.Count == 0;
                var panel = new Panel();
                panel.Size.SetPixels(size.X, size.Y);
                panel.Anchor = Anchor.Center;
                panel.AutoHeight = true;
                panel.OverflowMode = OverflowMode.HideOverflow;
                panel.Visible = _demoPanels.Count == 0;
                UISystem.Root.AddChild(panel);
                _demoPanels.Add(panel);

                // add title and underline
                panel.AddChild(new Title(demoTitle) { Anchor = Anchor.AutoCenter });
                panel.AddChild(new HorizontalLine());

                // create button to select this demo
                var button = new Button(demoTitle);
                button.Identifier = demoTitle;
                button.Anchor = Anchor.AutoLTR;
                button.Events.OnClick = (EntityUI entity) =>
                {
                    foreach (var sibling in _demoPanels) { sibling.Visible = false; }
                    panel.Visible = true;
                };
                button.ToggleCheckOnClick = true;
                button.ExclusiveSelection = true;
                button.CanClickToUncheck = false;
                button.Checked = isFirst;
                demoSelection.AddChild(button);

                // return panel
                return panel;
            }

            // intro page
            {
                var panel = CreateDemoContainer("Welcome To Iguina!", new Point(690, 1));
                panel.AddChild(new Paragraph($@"Welcome to ${{FC:00FFFF}}Iguina${{RESET}} UI demo!

Iguina is framework-agnostic UI library that can work with any rendering framework, provided the host application offers 'drivers' for rendering and input. In this demo, we are using '${{FC:00FFFF}}MonoGameRenderer${{RESET}}' drivers.

On the left panel, you'll find a list of all the subjects included in this demo. ${{FC:00FF00}}Click on a subject to select it and explore the components and features it showcases${{RESET}}.

PS. this demo UI theme was made by me and its public domain, so feel free to use it!
") { TextOverflowMode = TextOverflowMode.WrapWords });
            }

            // anchors
            {
                var panel = CreateDemoContainer("Anchors", new Point(780, 1));
                panel.AddChild(new Paragraph(
                    @"In Iguina, Entities are positioned using Anchors. An Anchor can be a pre-defined position on the parent entity, like Top-Left, or an automatic anchor, like AutoLTR, which places entities in rows from left to right.

The panel below shows all built-in non-automatic anchors:
"));

                var anchorsPanel = new Panel();
                anchorsPanel.Size.X.SetPercents(100f);
                anchorsPanel.Size.Y.SetPixels(400);
                anchorsPanel.Anchor = Anchor.AutoCenter;
                panel.AddChild(anchorsPanel);

                anchorsPanel.AddChild(new Paragraph("TopLeft") { Anchor = Anchor.TopLeft });
                anchorsPanel.AddChild(new Paragraph("TopRight") { Anchor = Anchor.TopRight });
                anchorsPanel.AddChild(new Paragraph("TopCenter") { Anchor = Anchor.TopCenter });
                anchorsPanel.AddChild(new Paragraph("BottomLeft") { Anchor = Anchor.BottomLeft });
                anchorsPanel.AddChild(new Paragraph("BottomRight") { Anchor = Anchor.BottomRight });
                anchorsPanel.AddChild(new Paragraph("BottomCenter") { Anchor = Anchor.BottomCenter });
                anchorsPanel.AddChild(new Paragraph("CenterLeft") { Anchor = Anchor.CenterLeft });
                anchorsPanel.AddChild(new Paragraph("CenterRight") { Anchor = Anchor.CenterRight });
                anchorsPanel.AddChild(new Paragraph("Center") { Anchor = Anchor.Center });
            }

            // auto anchors
            {
                var panel = CreateDemoContainer("Auto Anchors", new Point(750, 1));
                panel.AddChild(new Paragraph(
                    @"Previously we saw regular Anchors. Now its time to explore the Automatic anchors.

Auto Anchors are a set of anchors that place entities automatically, based on their siblings. For example:
"));

                var anchorsPanel = new Panel();
                anchorsPanel.Size.X.SetPercents(100f);
                anchorsPanel.AutoHeight = true;
                anchorsPanel.Anchor = Anchor.AutoCenter;
                panel.AddChild(anchorsPanel);

                {
                    anchorsPanel.AddChild(new Paragraph("AutoLTR first item.") { Anchor = Anchor.AutoLTR });
                    anchorsPanel.AddChild(new Paragraph("AutoLTR second item. Will be in a different row.") { Anchor = Anchor.AutoLTR });
                    var btn = anchorsPanel.AddChild(new Button("Button set to AutoLTR too.") { Anchor = Anchor.AutoLTR });
                    btn.Size.X.SetPixels(400);
                }
                anchorsPanel.AddChild(new HorizontalLine());
                {
                    anchorsPanel.AddChild(new Paragraph("This item is AutoRTL.") { Anchor = Anchor.AutoRTL });
                    anchorsPanel.AddChild(new Paragraph("AutoRTL second item. Will be in a different row.") { Anchor = Anchor.AutoRTL });
                    var btn = anchorsPanel.AddChild(new Button("Button set to AutoRTL too.") { Anchor = Anchor.AutoRTL });
                    btn.Size.X.SetPixels(400);
                }
                anchorsPanel.AddChild(new HorizontalLine());
                {
                    {
                        anchorsPanel.AddChild(new Paragraph("We also have inline anchors that arrange entities next to each other, and only break line when need to. For example, AutoInlineLTR buttons:") { Anchor = Anchor.AutoLTR });
                        for (int i = 0; i < 5; ++i)
                        {
                            var btn = anchorsPanel.AddChild(new Button("AutoInlineLTR") { Anchor = Anchor.AutoInlineLTR });
                            btn.Size.X.SetPixels(200);
                        }
                    }
                }
            }

            // panels
            {
                var panel = CreateDemoContainer("Panels", new Point(650, 1));
                panel.AddChild(new Paragraph(
                    @"Panels are simple containers for entities. They can have graphics, like the panel this text is in, or be transparent and used only for grouping.

For example, see these two buttons and two paragraphs? Each set is inside an invisible panel that takes up 50% of the parent panel's width. One is aligned left, the other right: 
"));
                panel.AddChild(new HorizontalLine());

                {
                    var panelLeft = new Panel(null!);
                    panelLeft.Size.X.SetPercents(50f);
                    panelLeft.Size.Y.SetPixels(160);
                    panelLeft.Anchor = Anchor.AutoInlineLTR;
                    panel.AddChild(panelLeft);

                    panelLeft.AddChild(new Paragraph("This is left panel!\n"));
                    panelLeft.AddChild(new Button());
                }
                {
                    var panelRight = new Panel(null!);
                    panelRight.Size.X.SetPercents(50f);
                    panelRight.Size.Y.SetPixels(160);
                    panelRight.Anchor = Anchor.AutoInlineLTR;
                    panel.AddChild(panelRight);

                    panelRight.AddChild(new Paragraph("This is right panel!\n"));
                    panelRight.AddChild(new Button());
                }

                panel.AddChild(new Paragraph(
                    @"You can add a small title to panels when you create them. It's not a built-in feature in Iguina, but its very easy to pull off: 

"));
                {
                    var titledPanel = new Panel();
                    titledPanel.Size.X.SetPercents(100f);
                    titledPanel.Size.Y.SetPixels(150);
                    titledPanel.Anchor = Anchor.AutoLTR;
                    panel.AddChild(titledPanel);

                    var title = new Paragraph(panelTitleStyle, "Panel Title");
                    titledPanel.AddChild(title);
                    title.Anchor = Anchor.TopCenter;
                    title.Offset.Y.SetPixels(-26);

                    titledPanel.AddChild(new Paragraph("Looks nice, isn't it? Check out the source code to see how we did it."));
                }

                panel.AddChild(new Paragraph(
                    @"
Did you know that entities can be draggable? This panel can be dragged, lets try it out!
The small box in the corner is draggable too:
"));
                panel.DraggableMode = DraggableMode.DraggableConfinedToScreen;

                // create draggable small box
                var draggableBox = panel.AddChild(new Panel()
                {
                    DraggableMode = DraggableMode.DraggableConfinedToParent,
                    Anchor = Anchor.BottomRight,
                });
                draggableBox.Size.SetPixels(20, 20);
            }

            // buttons
            {
                var panel = CreateDemoContainer("Buttons", new Point(650, 1));
                panel.AddChild(new Paragraph(
                    @"If you see this panel, it means you already used a button! Iguina has basic buttons you can easily place and register to their click events:
"));
                {
                    int clicksCount = 0;
                    var btn = panel.AddChild(new Button("Click Me!"));
                    btn.Events.OnClick += (EntityUI entity) =>
                    {
                        clicksCount++;
                        btn.Paragraph.Text = "Thanks x " + clicksCount;
                    };
                }

                panel.AddChild(new Paragraph(
    @"
Buttons can also function as checkboxes, allowing you to click on them to toggle their state (checked/unchecked):
"));
                {
                    var btn = panel.AddChild(new Button("Toggle Me!"));
                    btn.ToggleCheckOnClick = true;
                }

                panel.AddChild(new Paragraph(
    @"
And they can even function as a radio button, meaning only one button can be checked at any given time:
"));
                {
                    var btn = panel.AddChild(new Button("Pick Me!"));
                    btn.ToggleCheckOnClick = true;
                    btn.CanClickToUncheck = false;
                    btn.ExclusiveSelection = true;
                }
                {
                    var btn = panel.AddChild(new Button("No, pick Me!"));
                    btn.ToggleCheckOnClick = true;
                    btn.CanClickToUncheck = false;
                    btn.ExclusiveSelection = true;
                }
                {
                    var btn = panel.AddChild(new Button("Ignore them pick me!!"));
                    btn.ToggleCheckOnClick = true;
                    btn.CanClickToUncheck = false;
                    btn.ExclusiveSelection = true;
                }
            }

            // paragraphs
            {
                var panel = CreateDemoContainer("Paragraphs", new Point(650, 1));
                panel.AddChild(new Paragraph(
                    @"${FC:00FF00}Paragraphs${RESET} are entities that draw text.
They can be used as labels for buttons, titles, or long texts like the one you read now.

${FC:00FF00}Paragraphs${RESET} support special ${OC:FF0000}style changing commands${RESET}, so you can easily ${OC:00FFFF,FC:000000,OW:2}highlight specific words${RESET} within the paragraph.

You can change ${FC:00FF00}Fill Color${RESET}, ${OC:AA0000}Outline Color${RESET}, and ${OW:0}Outline Width${RESET}. To learn more, check out the source code of this demo project, or read the ${FC:FF00FF}official docs${RESET}.

Another thing to keep in mind about paragraphs is that you can change the way they wrap when exceeding the parent width. They can either wrap with breaking words, wrap while keeping words intact, or overflow without wrapping.
"));

            }

            // checkbox and radio
            {
                var panel = CreateDemoContainer("Checkbox / Radio", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"Iguina provides a basic Checkbox entity:
"));
                panel.AddChild(new Checkbox("Checkbox Option 1"));
                panel.AddChild(new Checkbox("Checkbox Option 2"));
                panel.AddChild(new Checkbox("Checkbox Option 3"));

                panel.AddChild(new HorizontalLine());

                panel.AddChild(new Paragraph(
                    @"Iguina also provides radio button entities:
"));
                panel.AddChild(new RadioButton("Radio Option 1")).Checked = true;
                panel.AddChild(new RadioButton("Radio Option 2"));
                panel.AddChild(new RadioButton("Radio Option 3"));
            }

            // sliders
            {
                var panel = CreateDemoContainer("Sliders", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"Sliders are useful to select numeric values:
"));
                {
                    var slider = panel.AddChild(new Slider());
                    var label = panel.AddChild(new Paragraph(@$"Slider Value: {slider.Value}
"));
                    slider.Events.OnValueChanged = (EntityUI entity) => { label.Text = $"Slider Value: {slider.Value}\n"; };
                }

                panel.AddChild(new Paragraph(
                    @"Sliders can also be vertical and scrollbars:
"));
                {
                    var slider = panel.AddChild(new Slider(Orientation.Vertical));

                    slider.Size.Y.SetPixels(280);
                    slider.Offset.X.SetPixels(40);
                    var label = panel.AddChild(new Paragraph(@$"Slider Value: {slider.Value}
"));
                    slider.Events.OnValueChanged = (EntityUI entity) => { label.Text = $"Slider Value: {slider.Value}\n"; };
                }

                {
                    var slider = panel.AddChild(new Slider(UISystem.DefaultStylesheets.VerticalScrollbars, UISystem.DefaultStylesheets.VerticalScrollbarsHandle, Orientation.Vertical));
                    slider.Size.Y.SetPixels(260);
                    slider.Offset.X.SetPixels(140);
                    slider.Offset.Y.SetPixels(90);
                    slider.Anchor = Anchor.BottomLeft;
                }
            }

            // progress bars
            {
                var panel = CreateDemoContainer("Progress Bars", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"Progress Bars are similar to sliders, but are designed to show progress or things like health bars:
"));
                {
                    var progressBar = panel.AddChild(new ProgressBar());
                    var label = panel.AddChild(new Paragraph(@$"Progress Bar Value: {progressBar.Value}
"));
                    float _timeForNextValueChange = 3f;
                    progressBar.Events.AfterUpdate = (EntityUI entity) =>
                    {
                        _timeForNextValueChange -= UISystem.LastDeltaTime;
                        if (_timeForNextValueChange <= 0f)
                        {
                            progressBar.Value = Random.Shared.Next(progressBar.MaxValue);
                            _timeForNextValueChange = 3f;
                        }
                    };
                    progressBar.Events.OnValueChanged = (EntityUI entity) => { label.Text = $"Progress Bar Value: {progressBar.Value}\n"; };
                }

                panel.AddChild(new Paragraph(
                    @"By default Progress Bars are not interactable, but you can make them behave like sliders by settings 'IgnoreInteractions' to false:
"));
                {
                    var progressBar = panel.AddChild(new ProgressBar());
                    var label = panel.AddChild(new Paragraph(@$"Progress Bar Value: {progressBar.Value}
"));
                    progressBar.Handle.OverrideStyles.FillColor = new Color(255, 0, 0, 255);
                    progressBar.IgnoreInteractions = false;
                    progressBar.Events.OnValueChanged = (EntityUI entity) => { label.Text = $"Progress Bar Value: {progressBar.Value}\n"; };
                }

                panel.AddChild(new Paragraph(
                    @"And finally, here's an alternative progress bar design, without animation:
"));
                {
                    var progressBar = panel.AddChild(new ProgressBar(hProgressBarAltStyle, hProgressBarAltFillStyle));
                    progressBar.Size.X.SetPixels(420 + 36);
                    progressBar.MaxValue = 11;
                    progressBar.Value = 6;
                    progressBar.IgnoreInteractions = false;
                    progressBar.Anchor = Anchor.AutoCenter;
                }
            }

            // for lists and dropdowns
            List<string> dndClasses = new List<string> { "Barbarian", "Bard", "Cleric", "Druid", "Fighter", "Monk", "Paladin", "Ranger", "Rogue", "Sorcerer", "Warlock", "Wizard", "Artificer", "Blood Hunter", "Mystic", "Psion", "Alchemist", "Cavalier", "Hexblade", "Arcane Archer", "Samurai", "Zzz" };
            dndClasses.Sort(StringComparer.OrdinalIgnoreCase);

            // list box
            {
                var panel = CreateDemoContainer("List Box", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"List Boxes allow you to add items and select them from a list. For example:
"));
                {
                    panel.AddChild(new Paragraph(
                    @"
Select Race:"));
                    var listbox = panel.AddChild(new ListBox());
                    listbox.AddItem("Human");
                    listbox.AddItem("Elf");
                    listbox.AddItem("Orc");
                    listbox.AddItem("Dwarf");
                    listbox.AutoHeight = true;
                    listbox.AllowDeselect = false;
                    panel.AddChild(new Paragraph(
                    @"Did you notice that you can't clear selection once a value is set? That is a configurable property. In the class selection below, you can clear by clicking the selected item again."));
                }
                {
                    panel.AddChild(new Paragraph(
                    @"
Select Class:"));
                    var listbox = panel.AddChild(new ListBox());
                    foreach (var val in dndClasses)
                    {
                        listbox.AddItem(val);
                    }
                    var selectedParagraph = panel.AddChild(new Paragraph());
                    selectedParagraph.Text = "Selected Class: None";
                    listbox.Events.OnValueChanged = (EntityUI entity) =>
                    {
                        selectedParagraph.Text = "Selected Class: " + (listbox.SelectedValue ?? "None");
                    };
                }
            }

            // drop down
            {
                var panel = CreateDemoContainer("Drop Down", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"Drop Down entities are basically list boxes, but they collapse while they are not interacted with. For example:
"));
                {
                    panel.AddChild(new Paragraph(
                    @"
Select Race:"));
                    var dropdown = panel.AddChild(new DropDown());
                    dropdown.DefaultSelectedText = "< Select Race >";
                    dropdown.AddItem("Human");
                    dropdown.AddItem("Elf");
                    dropdown.AddItem("Orc");
                    dropdown.AddItem("Dwarf");
                    dropdown.AllowDeselect = false;
                    dropdown.AutoHeight = true;
                    panel.AddChild(new Paragraph(
                    @"Did you notice that you can't clear selection once a value is set? That is a configurable property. In the class selection below, you can clear by clicking the selected item again."));
                }
                {
                    panel.AddChild(new Paragraph(
                    @"
Select Class:"));
                    var dropdown = panel.AddChild(new DropDown());
                    dropdown.DefaultSelectedText = "< Select Class >";
                    foreach (var val in dndClasses)
                    {
                        dropdown.AddItem(val);
                    }
                    var selectedParagraph = panel.AddChild(new Paragraph());
                    selectedParagraph.Text = "Selected Class: None";
                    dropdown.Events.OnValueChanged = (EntityUI entity) =>
                    {
                        selectedParagraph.Text = "Selected Class: " + (dropdown.SelectedValue ?? "None");
                    };
                }
            }

            // scrollbars
            {
                var panel = CreateDemoContainer("Scrollbars", new Point(780, 350));
                panel.AutoHeight = false;
                panel.CreateVerticalScrollbar(true);
                panel.AddChild(new Paragraph(
                    @"Sometimes panels content is too long, and we need scrollbars to show everything.
This panel has some random entities below that go wayyyy down.

Use the scrollbar on the right to see more of it.
"));
                panel.AddChild(new Button("Some Button"));
                panel.AddChild(new Paragraph(
                    @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
"));
                panel.AddChild(new Button("Another Button"));
                panel.AddChild(new Slider());
                panel.AddChild(new Checkbox("A Checkbox"));
                panel.AddChild(new RadioButton("A Radio Button"));
                var listbox = panel.AddChild(new ListBox());
                listbox.AddItem("Human");
                listbox.AddItem("Elf");
                listbox.AddItem("Orc");
                listbox.AddItem("Dwarf");
                listbox.Size.Y.SetPixels(170);
            }

            // text input
            {
                var panel = CreateDemoContainer("Text Input", new Point(680, 1));

                panel.AddChild(new Paragraph(
                    @"Text Input entity is useful to get free text input from the user:
"));
                {
                    var textInput = panel.AddChild(new TextInput());
                    textInput.PlaceholderText = "Click to edit text input.";
                }

                panel.AddChild(new Paragraph(
    @"
Text Inputs can also be multiline:
"));
                {
                    var textInput = panel.AddChild(new TextInput());
                    textInput.PlaceholderText = "A multiline text input..\nClick to edit.";
                    textInput.Size.Y.SetPixels(300);
                    textInput.Multiline = true;
                    //textInput.MaxLines = 8;
                    textInput.CreateVerticalScrollbar();
                }
            }

            // locked / disabled
            {
                var panel = CreateDemoContainer("Locked / Disabled", new Point(780, 1));
                panel.AddChild(new Paragraph(
                    @"You can disable entities to make them ignore user interactions and render them with 'disabled' effect (in this demo, grayscale):
"));
                panel.AddChild(new Button("Disabled Button") { Enabled = false });
                panel.AddChild(new Paragraph(
                    @"
When you disable a panel, all entities under it will be disabled too.

If you want to just lock items without rendering them with 'disabled' style, you can also set the Locked property. For example the following button is locked, but will render normally:
"));
                panel.AddChild(new Button("Locked Button") { Locked = true });
                panel.AddChild(new Paragraph(
        @"
Any type of entity can be locked and disabled and locked:
"));
                panel.AddChild(new Slider() { Enabled = false });
                panel.AddChild(new Checkbox("Disabled Checkbox") { Enabled = false });
                panel.AddChild(new RadioButton("Disabled Radio Button") { Enabled = false });
                var listbox = panel.AddChild(new ListBox());
                listbox.AddItem("Human");
                listbox.AddItem("Elf");
                listbox.AddItem("Orc");
                listbox.AddItem("Dwarf");
                listbox.Size.Y.SetPixels(140);
                listbox.Enabled = false;
            }

            /*UserInterface.Active.Clear();

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

            #endregion Bottom Panel*/

            // Create other GUIs last so that we don't steal input focus their.
            CurrentScene?.GetEntityList<Entity>()
                .Where(x => x is IHaveGUI)
                .Select(x => x as IHaveGUI).ToList()
                .ForEach(x => x.CreateUI());
        }

		public override void Update()
		{
			base.Update();

            UISystem.Update();

            /*_postFXPanelAnimation.Update();
            if (_postFXPanelAnimation.Running)
            {
                _postFXPanel.Offset = new Vector2(
                    _postFXPanelOffsetX * (float)_postFXPanelAnimation.Progress, 0);
                _postFXButton.Offset = new Vector2(_postFXPanel.Offset.X -_postFXPanelOffsetX, 0);
            }

            if (Input.CheckButtonPress(_toggleUIButton))
			{
                UserInterface.Active.Root.Visible = !UserInterface.Active.Root.Visible;
            }*/

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

            /*if (_FPS_Paragraph != null) _FPS_Paragraph.Text = "FPS: {{YELLOW}}" + GameMgr.FPS + "{{DEFAULT}}";
            
            UserInterface.Active.Update();*/
        }

		public override void Draw()
        {
            base.Draw();

            UISystem.Draw();
            //UserInterface.Active.Draw();
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
