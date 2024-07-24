using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonoGo.Samples.Demos
{
    public class UIDemo : Entity, IHaveGUI
    {
        public static int CurrentExample = 0;

        List<Panel> panels = new();
        Button nextExampleButton;
        Button previousExampleButton;

        public UIDemo(Layer layer) : base(layer)
        {
        }

        public void CreateUI()
        {
            // load some alt stylesheets that are not loaded by default from the system stylesheet
            var hProgressBarAltStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "progress_bar_horizontal_alt.json"));
            var hProgressBarAltFillStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "progress_bar_horizontal_alt_fill.json"));
            var panelTitleStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "panel_title.json"));
            var panelImageStyle = StyleSheet.LoadFromJsonFile(Path.Combine(UISystem.ThemeActiveFolder, "Styles", "panel_image.json"));

            // create top panel
            int topPanelHeight = 65;
            Panel topPanel = new(null!)
            {
                Identifier = "Top Panel",
                Anchor = Anchor.TopCenter
            };
            topPanel.Size.Y.SetPixels(topPanelHeight + 2);
            UISystem.Add(topPanel);

            // add previous example button
            previousExampleButton = new("<- GUI.Back")
            {
                Anchor = Anchor.TopCenter
            };
            previousExampleButton.Size.SetPixels(280, topPanelHeight);
            previousExampleButton.Offset.X.SetPixels(-500);
            previousExampleButton.Events.OnClick = (Control control) => PreviousExample();
            topPanel.AddChild(previousExampleButton);

            // add button to enable debug mode
            {
                Button button = new("Debug Mode")
                {
                    Anchor = Anchor.TopCenter,
                    ToggleCheckOnClick = true
            };
                button.Size.SetPixels(240, topPanelHeight);
                button.Offset.X.SetPixels(-240);
                button.Events.OnClick = (Control control) => UISystem.DebugDraw = !UISystem.DebugDraw;
                topPanel.AddChild(button);
            }

            // add button to apply transformations
            {
                Button button = new("Transform UI")
                {
                    Anchor = Anchor.TopCenter,
                    ToggleCheckOnClick = true
            };
                button.Size.SetPixels(240, topPanelHeight);
                button.Offset.X.SetPixels(240);
                button.Events.OnClick = (Control control) =>
                {
                    UISystem.UseRenderTargetTransformMatrix = !UISystem.UseRenderTargetTransformMatrix;

                    if (UISystem.UseRenderTargetTransformMatrix)
                    {
                        RenderMgr.GUITransformMatrix = Matrix.CreateScale(0.6f) *
                            Matrix.CreateRotationZ(0.05f) *
                            Matrix.CreateTranslation(new Vector3(150, 150, 0));
                    }
                    else RenderMgr.GUITransformMatrix = Matrix.Identity;
                };
                topPanel.AddChild(button);
            }

            // create button to open git repo
            {
                Button button = new("GitHub")
                {
                    Anchor = Anchor.TopCenter
                };
                button.Events.OnValueChanged = (Control control) =>
                {
                };
                button.Size.SetPixels(240, topPanelHeight);
                topPanel.AddChild(button);
            }

            // add next example button
            nextExampleButton = new("GUI.Next ->")
            {
                Anchor = Anchor.TopCenter
            };
            nextExampleButton.Size.SetPixels(280, topPanelHeight);
            nextExampleButton.Offset.X.SetPixels(500);
            nextExampleButton.Identifier = "next_btn";
            nextExampleButton.Events.OnClick = (Control control) => NextExample();
            topPanel.AddChild(nextExampleButton);

            // init all examples
            if (true)
            {
                // example: welcome message
                {
                    var logo = new Panel(panelImageStyle)
                    {
                        Anchor = Anchor.AutoCenter
                    };
                    var logoTexture = ResourceHub.GetResource<Sprite>("DemoSprites", "Logo")[0].Texture;
                    logo.StyleSheet.Default.Icon.Texture = logoTexture;
                    logo.Size.SetPixels(logoTexture.Width, logoTexture.Height);
                    logo.StyleSheet.Default.MarginAfter = new Point(0, 25);

                    // add title and text
                    var panel = CreateDemoContainer(null, new Point(1200, -1));
                    panel.StyleSheet = new StyleSheet(); // Empty StyleSheet to hide the panel.
                    var welcomeText = new Paragraph(@"Welcome!

This special game engine is built ontop of ${FC:FF0000}MonoGame${RESET},the powerfull gamedev framework which is running under the hood of many fantastic games like ${FC:FF0000}Stardew Valley${RESET} and ${FC:FF0000}Celeste${RESET}.

Stay tuned for more things to come! (probably ${FC:FFFF00}:)${RESET})

Please click the ${FC:FF0000}GUI.Next${RESET} button at the top to see more GUI-DEMOS or the ${FC:FFFF00}Next${RESET} button below to see more SAMPLE-DEMOS of the engine.

") { TextOverflowMode = TextOverflowMode.WrapWords };
                    welcomeText.StyleSheet.Default.FontSize = 28;
                    panel.AddChild(logo);
                    panel.AddChild(welcomeText);
                    var version = new Paragraph("${FC:FFFF00}v" + Assembly.GetAssembly(typeof(Entity)).GetName().Version + "${RESET}")
                    {
                        Anchor = Anchor.BottomRight
                    };
                    panel.AddChild(version);
                }

                // create container for a demo example
                Panel CreateDemoContainer(string demoTitle, Point size)
                {
                    // create panel
                    var panel = new Panel();
                    panel.Size.SetPixels(size.X, size.Y);
                    panel.Anchor = Anchor.Center;
                    panel.AutoHeight = true;
                    panel.OverflowMode = OverflowMode.HideOverflow;
                    UISystem.Add(panel);
                    panels.Add(panel);

                    if (demoTitle != null)
                    {
                        // add title and underline
                        panel.AddChild(new Title(demoTitle) { Anchor = Anchor.AutoCenter });
                        panel.AddChild(new HorizontalLine());
                    }

                    // return panel
                    return panel;
                }

                // anchors
                {
                    var panel = CreateDemoContainer("Anchors", new Point(780, 1));
                    panel.AddChild(new Paragraph(@"Controls are positioned using Anchors. An Anchor can be a pre-defined position on the parent control, like Top-Left, or Center.
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
                    panel.AddChild(new Paragraph(@"Placing controls automatically, based on their siblings."));

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
                            anchorsPanel.AddChild(new Paragraph("Inline anchors arrange controls next to each other, and only break line when need to.") { Anchor = Anchor.AutoLTR });
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
                    var panel = CreateDemoContainer("Panels", new Point(800, 1));
                    panel.AddChild(new Paragraph(
                        @"Panels are simple containers for controls. They can have graphics or be transparent and used only for grouping.
"));
                    panel.AddChild(new HorizontalLine());

                    {
                        var panelLeft = new Panel(null!);
                        panelLeft.Size.X.SetPercents(50f);
                        panelLeft.Size.Y.SetPixels(160);
                        panelLeft.Anchor = Anchor.AutoInlineLTR;
                        panel.AddChild(panelLeft);

                        panelLeft.AddChild(new Paragraph("Invisible left panel.\n"));
                        panelLeft.AddChild(new Button());
                    }
                    {
                        var panelRight = new Panel(null!);
                        panelRight.Size.X.SetPercents(50f);
                        panelRight.Size.Y.SetPixels(160);
                        panelRight.Anchor = Anchor.AutoInlineLTR;
                        panel.AddChild(panelRight);

                        panelRight.AddChild(new Paragraph("Invisible right panel.\n"));
                        panelRight.AddChild(new Button());
                    }

                    panel.AddChild(new Paragraph(
                        @"You can add a small title to panels when you create them by using a custom style sheet:
"));
                    {
                        var titledPanel = new Panel();
                        titledPanel.AutoHeight = true;
                        titledPanel.Size.X.SetPercents(100f);
                        titledPanel.Anchor = Anchor.AutoLTR;
                        panel.AddChild(titledPanel);

                        var title = new Paragraph(panelTitleStyle, "Panel Title");
                        titledPanel.AddChild(title);
                        title.Anchor = Anchor.TopCenter;
                        title.Offset.Y.SetPixels(-26);

                        titledPanel.AddChild(new Paragraph("Looks nice, isn't it? Check out the source code to see how it works."));
                    }

                    panel.AddChild(new Paragraph(@"
BTW: This panel can be dragged, lets try it out! The small box in the corner is draggable too:"));
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
                    panel.AddChild(new Paragraph("Easily place buttons and register click events:"));
                    {
                        int clicksCount = 0;
                        var btn = panel.AddChild(new Button("Click Me!"));
                        btn.Events.OnClick += (Control control) =>
                        {
                            clicksCount++;
                            btn.Paragraph.Text = "Thanks x " + clicksCount;
                        };
                    }

                    panel.AddChild(new Paragraph(@"
Buttons can also function as checkboxes, allowing you to click on them to toggle their state (checked/unchecked):"));
                    {
                        var btn = panel.AddChild(new Button("Toggle Me!"));
                        btn.ToggleCheckOnClick = true;
                    }

                    panel.AddChild(new Paragraph(@"
And they can even function as a radio button, meaning only one button can be checked at any given time:"));
                    {
                        var btn = panel.AddChild(new Button("First Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                    {
                        var btn = panel.AddChild(new Button("Second Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                    {
                        var btn = panel.AddChild(new Button("Third Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                }

                // paragraphs
                {
                    var panel = CreateDemoContainer("Paragraphs", new Point(650, 1));
                    panel.AddChild(new Paragraph(
                        @"${FC:00FF00}Paragraphs${RESET} are controls that draw text.
They can be used as labels for buttons, titles, or long texts like the one you read now.

${FC:00FF00}Paragraphs${RESET} support special ${OC:FF0000}style changing commands${RESET}, so you can easily ${OC:00FFFF,FC:000000,OW:2}highlight specific words${RESET} within the paragraph.

You can change ${FC:00FF00}Fill Color${RESET}, ${OC:AA0000}Outline Color${RESET}, and ${OW:0}Outline Width${RESET}.

Another thing to keep in mind about paragraphs is that you can change the way they wrap when exceeding the parent width. They can either wrap with breaking words, wrap while keeping words intact, or overflow without wrapping.
"));

                }

                // checkbox and radio
                {
                    var panel = CreateDemoContainer("Checkbox / Radio", new Point(680, 1));

                    panel.AddChild(new Paragraph(
                        @"Basic Checkbox control:
"));
                    panel.AddChild(new Checkbox("Checkbox Option 1"));
                    panel.AddChild(new Checkbox("Checkbox Option 2"));
                    panel.AddChild(new Checkbox("Checkbox Option 3"));

                    panel.AddChild(new HorizontalLine());

                    panel.AddChild(new Paragraph(
                        @"Radio button controls:
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
                        slider.Events.OnValueChanged = (Control control) => { label.Text = $"Slider Value: {slider.Value}\n"; };
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
                        slider.Events.OnValueChanged = (Control control) => { label.Text = $"Slider Value: {slider.Value}\n"; };
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
                        progressBar.Events.AfterUpdate = (Control control) =>
                        {
                            _timeForNextValueChange -= UISystem.LastDeltaTime;
                            if (_timeForNextValueChange <= 0f)
                            {
                                progressBar.Value = Random.Shared.Next(progressBar.MaxValue);
                                _timeForNextValueChange = 3f;
                            }
                        };
                        progressBar.Events.OnValueChanged = (Control control) => { label.Text = $"Progress Bar Value: {progressBar.Value}\n"; };
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
                        progressBar.Events.OnValueChanged = (Control control) => { label.Text = $"Progress Bar Value: {progressBar.Value}\n"; };
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
                    var panel = CreateDemoContainer("List Box", new Point(800, 700));
                    panel.AutoHeight = false;
                    panel.CreateVerticalScrollbar(true);

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
                        listbox.Events.OnValueChanged = (Control control) =>
                        {
                            selectedParagraph.Text = "Selected Class: " + (listbox.SelectedValue ?? "None");
                        };
                    }
                }

                // drop down
                {
                    var panel = CreateDemoContainer("Drop Down", new Point(680, 1));

                    panel.AddChild(new Paragraph(
                        @"Drop Down controls are basically list boxes, but they collapse while they are not interacted with. For example:
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
                        dropdown.Events.OnValueChanged = (Control control) =>
                        {
                            selectedParagraph.Text = "Selected Class: " + (dropdown.SelectedValue ?? "None");
                        };
                    }
                }

                // scrollbars
                {
                    var panel = CreateDemoContainer("Scrollbars", new Point(780, 450));
                    panel.AutoHeight = false;
                    panel.CreateVerticalScrollbar(true);
                    panel.AddChild(new Paragraph(
                        @"Sometimes panels content is too long, and we need scrollbars to show everything.
This panel has some random controls below that go wayyyy down.

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
                        @"Text Input control is useful to get free text input from the user:
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
                    var panel = CreateDemoContainer("Locked / Disabled", new Point(900, 600));
                    panel.AutoHeight = false;
                    panel.CreateVerticalScrollbar(true);
                    panel.AddChild(new Paragraph(
                        @"You can disable controls to make them ignore user interactions and render them with 'disabled' effect (in this demo, grayscale):
"));
                    panel.AddChild(new Button("Disabled Button") { Enabled = false });
                    panel.AddChild(new Paragraph(
                        @"
When you disable a panel, all controls under it will be disabled too.

If you want to just lock items without rendering them with 'disabled' style, you can also set the Locked property. For example the following button is locked, but will render normally:
"));
                    panel.AddChild(new Button("Locked Button") { Locked = true });
                    panel.AddChild(new Paragraph(
            @"
Any type of control can be locked and disabled and locked:
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

                /*// example: epilogue
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, 400), PanelSkin.None);
                    panels.Add(panel);
                    UISystem.Root.AddChild(panel);

                    // add title and text
                    panel.AddChild(new Paragraph("End Of GUI-DEMO"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"That was only the GUI-DEMO! There is still much to learn about {{MG_YELLOW}}Mono{{MG_RED}}Go{{DEFAULT}}.

Try more samples by clicking the {{MG_YELLOW}}Next{{DEFAULT}} button below.

If you like this engine then don't forget to star the repo on GitHub.

Have a nice day!

:: {{MG_FANCY}}BlizzCrafter{{DEFAULT}} =)"));
                }*/

                // init panels and buttons
                UpdateAfterExampleChange();

            }
        }

        /// <summary>
        /// Show next UI example.
        /// </summary>
        public void NextExample()
        {
            CurrentExample++;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Show previous UI example.
        /// </summary>
        public void PreviousExample()
        {
            CurrentExample--;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Called after we change current example index, to hide all examples
        /// except for the currently active example + disable prev / next buttons if
        /// needed (if first or last example).
        /// </summary>
        protected void UpdateAfterExampleChange()
        {
            // hide all panels and show current example panel
            foreach (Panel panel in panels)
            {
                panel.Visible = false;
            }
            panels[CurrentExample].Visible = true;

            // disable / enable next and previous buttons
            nextExampleButton.Enabled = CurrentExample != panels.Count - 1;
            previousExampleButton.Enabled = CurrentExample != 0;
        }
    }
}
