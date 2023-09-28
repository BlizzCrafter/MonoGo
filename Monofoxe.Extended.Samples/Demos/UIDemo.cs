using GUI.DataTypes;
using Microsoft.Xna.Framework;
using Monofoxe.Extended.Engine.SceneSystem;
using Monofoxe.Extended.Samples.Misc;
using Monofoxe.Extended.GUI;
using Monofoxe.Extended.GUI.Entities;
using Monofoxe.Extended.GUI.Utils.Forms;
using System.Collections.Generic;
using System.Reflection;
using Monofoxe.Extended.Engine.EC;

namespace Monofoxe.Extended.Samples.Demos
{
    public class UIDemo : SurfaceEntity, IGuiEntity
    {
        List<Panel> panels = new List<Panel>();
        Button nextExampleButton;
        Button previousExampleButton;
        Paragraph targetEntityShow;
        int currExample = 0;

        public UIDemo(Layer layer) : base(layer)
        {           
        }

        public void CreateUI()
        {
            // create top panel
            int topPanelHeight = 65;
            Panel topPanel = new Panel(new Vector2(0, topPanelHeight + 2), PanelSkin.None, Anchor.TopCenter);
            topPanel.Padding = Vector2.Zero;
            UserInterface.Active.AddEntity(topPanel);

            // add previous example button
            previousExampleButton = new Button("<- GUI.Back", ButtonSkin.Alternative, Anchor.TopCenter, new Vector2(280, topPanelHeight), offset: new Vector2(-500, 0));
            previousExampleButton.OnClick = (EntityUI btn) => { this.PreviousExample(); };
            topPanel.AddChild(previousExampleButton);

            // add button to enable debug mode
            Button debugBtn = new Button("Debug Mode", anchor: Anchor.TopCenter, size: new Vector2(240, topPanelHeight), offset: new Vector2(-240, 0));
            debugBtn.OnClick = (EntityUI entity) =>
            {
                UserInterface.Active.DebugDraw = !UserInterface.Active.DebugDraw;
            };
            debugBtn.ToggleMode = true;
            debugBtn.ToolTipText = "Enable special debug drawing mode.";
            topPanel.AddChild(debugBtn);

            // add button to apply transformations
            Button transBtn = new Button("Transform UI", anchor: Anchor.TopCenter, size: new Vector2(240, topPanelHeight), offset: new Vector2(240, 0));
            transBtn.OnClick = (EntityUI entity) =>
            {
                UserInterface.Active.UseRenderTargetTransformMatrix = !UserInterface.Active.UseRenderTargetTransformMatrix;

                if (UserInterface.Active.UseRenderTargetTransformMatrix)
                {
                    UserInterface.Active.RenderTargetTransformMatrix = Matrix.CreateScale(0.6f) *
                        Matrix.CreateRotationZ(0.05f) *
                        Matrix.CreateTranslation(new Vector3(150, 150, 0));
                }
                else UserInterface.Active.RenderTargetTransformMatrix = Matrix.Identity;
            };
            transBtn.ToggleMode = true;
            transBtn.ToolTipText = "Apply transform matrix on the entire UI.";
            topPanel.AddChild(transBtn);

            // events panel for debug
            Panel eventsPanel = new Panel(new Vector2(400, 530), PanelSkin.Simple, Anchor.CenterLeft, new Vector2(-10, 0));
            eventsPanel.Visible = false;

            // events log (single-time events)
            eventsPanel.AddChild(new Label("Events Log:"));
            SelectList eventsLog = new SelectList(size: new Vector2(-1, 280));
            eventsLog.ExtraSpaceBetweenLines = -8;
            eventsLog.ItemsScale = 0.5f;
            eventsLog.Locked = true;
            eventsPanel.AddChild(eventsLog);

            // current events (events that happen while something is true)
            eventsPanel.AddChild(new Label("Current Events:"));
            SelectList eventsNow = new SelectList(size: new Vector2(-1, 100));
            eventsNow.ExtraSpaceBetweenLines = -8;
            eventsNow.ItemsScale = 0.5f;
            eventsNow.Locked = true;
            eventsPanel.AddChild(eventsNow);

            // paragraph to show currently active panel
            targetEntityShow = new Paragraph("test", Anchor.Auto, Color.White, scale: 0.75f);
            eventsPanel.AddChild(targetEntityShow);

            // add the events panel
            UserInterface.Active.AddEntity(eventsPanel);

            // whenever events log list size changes, make sure its not too long. if it is, trim it.
            eventsLog.OnListChange = (EntityUI entity) =>
            {
                SelectList list = (SelectList)entity;
                if (list.Count > 100)
                {
                    list.RemoveItem(0);
                }
            };

            // listen to all global events - one timers
            UserInterface.Active.OnClick = (EntityUI entity) => { eventsLog.AddItem("Click: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnRightClick = (EntityUI entity) => { eventsLog.AddItem("RightClick: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseDown = (EntityUI entity) => { eventsLog.AddItem("MouseDown: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnRightMouseDown = (EntityUI entity) => { eventsLog.AddItem("RightMouseDown: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseEnter = (EntityUI entity) => { eventsLog.AddItem("MouseEnter: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseLeave = (EntityUI entity) => { eventsLog.AddItem("MouseLeave: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseReleased = (EntityUI entity) => { eventsLog.AddItem("MouseReleased: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseWheelScroll = (EntityUI entity) => { eventsLog.AddItem("Scroll: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnStartDrag = (EntityUI entity) => { eventsLog.AddItem("StartDrag: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnStopDrag = (EntityUI entity) => { eventsLog.AddItem("StopDrag: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnFocusChange = (EntityUI entity) => { eventsLog.AddItem("FocusChange: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnValueChange = (EntityUI entity) => { if (entity.Parent == eventsLog) { return; } eventsLog.AddItem("ValueChanged: " + entity.GetType().Name); eventsLog.scrollToEnd(); };

            // clear the current events after every frame they were drawn
            eventsNow.AfterDraw = (EntityUI entity) => { eventsNow.ClearItems(); };

            // listen to all global events - happening now
            UserInterface.Active.WhileDragging = (EntityUI entity) => { eventsNow.AddItem("Dragging: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            UserInterface.Active.WhileMouseDown = (EntityUI entity) => { eventsNow.AddItem("MouseDown: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            UserInterface.Active.WhileMouseHover = (EntityUI entity) => { eventsNow.AddItem("MouseHover: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            eventsNow.MaxItems = 4;

            // add extra info button
            Button infoBtn = new Button("  Events", anchor: Anchor.TopCenter, size: new Vector2(240, topPanelHeight));
            infoBtn.AddChild(new Icon(IconType.Scroll, Anchor.CenterLeft), true);
            infoBtn.OnClick = (EntityUI entity) =>
            {
                eventsPanel.Visible = !eventsPanel.Visible;
            };
            infoBtn.ToggleMode = true;
            infoBtn.ToolTipText = "Show events log.";
            topPanel.AddChild(infoBtn);

            // add next example button
            nextExampleButton = new Button("GUI.Next ->", ButtonSkin.Alternative, Anchor.TopCenter, new Vector2(280, topPanelHeight), offset: new Vector2(500, 0));
            nextExampleButton.OnClick = (EntityUI btn) => { this.NextExample(); };
            nextExampleButton.Identifier = "next_btn";
            topPanel.AddChild(nextExampleButton);

            // zoom in / out factor
            float zoominFactor = 0.05f;

            // scale show
            Paragraph scaleShow = new Paragraph("100%", Anchor.CenterLeft, offset: new Vector2(10, 70));
            UserInterface.Active.AddEntity(scaleShow);

            // init zoom-out button
            Button zoomout = new Button(string.Empty, ButtonSkin.Default, Anchor.CenterLeft, new Vector2(70, 70));
            Icon zoomoutIcon = new Icon(IconType.ZoomOut, Anchor.Center, 0.75f);
            zoomout.AddChild(zoomoutIcon, true);
            zoomout.OnClick = (EntityUI btn) => {
                if (UserInterface.Active.GlobalScale > 0.5f)
                    UserInterface.Active.GlobalScale -= zoominFactor;
                scaleShow.Text = ((int)System.Math.Round(UserInterface.Active.GlobalScale * 100f)).ToString() + "%";
            };
            UserInterface.Active.AddEntity(zoomout);

            // init zoom-in button
            Button zoomin = new Button(string.Empty, ButtonSkin.Default, Anchor.CenterLeft, new Vector2(70, 70), new Vector2(70, 0));
            Icon zoominIcon = new Icon(IconType.ZoomIn, Anchor.Center, 0.75f);
            zoomin.AddChild(zoominIcon, true);
            zoomin.OnClick = (EntityUI btn) => {
                if (UserInterface.Active.GlobalScale < 1.45f)
                    UserInterface.Active.GlobalScale += zoominFactor;
                scaleShow.Text = ((int)System.Math.Round(UserInterface.Active.GlobalScale * 100f)).ToString() + "%";
            };
            UserInterface.Active.AddEntity(zoomin);

            // init all examples

            if (true)
            {

                // example: welcome message
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    var welcomeText = new RichParagraph(@"Welcome to {{MFE_ORANGE}}Monofoxe{{MFE_YELLOW}}.Extended{{DEFAULT}}!

Monofoxe.Extended is an advanced version of the Monofoxe Engine (built with {{MG}}MonoGame{{DEFAULT}}).

It currently extends Monofoxe with a powerful graphical user interface ({{MFE_YELLOW}}GUI{{DEFAULT}}). 

Stay tuned for more things to come! (probably {{MFE_ORANGE}}:){{DEFAULT}})

Please click the {{BUTTON_ALT_BOLD}}GUI.Next{{DEFAULT}} button at the top to see more GUI-DEMOS or the {{BUTTON_BOLD}}Next{{DEFAULT}} button below to see more SAMPLE-DEMOS of the engine.

");
                    panel.AddChild(welcomeText);
                    panel.AddChild(new Paragraph("v" + Assembly.GetAssembly(typeof(Entity)).GetName().Version, Anchor.BottomRight)).FillColor = Color.Yellow;
                }

                // example: features list
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(500, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Widgets Types"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"The following widgets are available:

- Paragraphs
- Headers
- Buttons
- Panels
- CheckBox
- Radio buttons
- Rectangles
- Images & Icons
- Select List
- Dropdown
- Panel Tabs
- Sliders & Progressbars
- Text input
- Tooltip Text
- And more...
"));
                }

                // example: basic concepts
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(740, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Basic Concepts"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Panels are basic containers. They are like window forms.

To position elements inside panels or other widgets, you set an anchor and offset. An anchor is a pre-defined position in parent element, like top-left corner, center, etc. and offset is just the distance from that point.

Another thing to keep in mind is size; Most widgets come with a default size, but for those you need to set size for remember that setting size 0 will take full width / height. For example, size of X = 0, Y = 100 means the widget will be 100 pixels height and the width of its parent (minus the parent padding)."));
                }

                // example: anchors
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(800, 620));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Paragraph(@"Anchors help position elements. For example, this paragraph anchor is 'center'.

The most common anchors are 'Auto' and 'AutoInline', which will place entities one after another automatically.",
                        Anchor.Center, Color.White, 0.8f, new Vector2(320, 0)));

                    panel.AddChild(new Header("Anchors", Anchor.TopCenter, new Vector2(0, 100)));
                    panel.AddChild(new Paragraph("top-left", Anchor.TopLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("top-center", Anchor.TopCenter, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("top-right", Anchor.TopRight, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-left", Anchor.BottomLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-center", Anchor.BottomCenter, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-right", Anchor.BottomRight, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("center-left", Anchor.CenterLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("center-right", Anchor.CenterRight, Color.Yellow, 0.8f));
                }

                // example: buttons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Buttons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("3 button skins included:"));

                    // add default buttons
                    panel.AddChild(new Button("Default", ButtonSkin.Default));
                    panel.AddChild(new Button("Alternative", ButtonSkin.Alternative));
                    panel.AddChild(new Button("Fancy", ButtonSkin.Fancy));

                    // custom button

                    // toggle button
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new Paragraph("Note: buttons can also work in toggle mode:"));
                    Button btn = new Button("Toggle Me!", ButtonSkin.Default);
                    btn.ToggleMode = true;
                    panel.AddChild(btn);
                }

                // example: checkboxes and radio buttons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // checkboxes example
                    panel.AddChild(new Header("CheckBox"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("CheckBoxes example:"));

                    panel.AddChild(new CheckBox("CheckBox 1"));
                    panel.AddChild(new CheckBox("CheckBox 2"));

                    // radio example
                    panel.AddChild(new LineSpace(3));
                    panel.AddChild(new Header("Radio buttons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Radio buttons example:"));

                    panel.AddChild(new RadioButton("Option 1"));
                    panel.AddChild(new RadioButton("Option 2"));
                    panel.AddChild(new RadioButton("Option 3"));
                }

                // example: panels
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // title and text
                    panel.AddChild(new Header("Panels"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("4 alternative panel skins included:"));
                    int panelHeight = 80;
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Fancy, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Fancy Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Golden, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Alternative Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Simple, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Simple Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.ListBackground, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("List Background", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                }

                // example: draggable
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, 690));
                    panel.Draggable = true;
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // title and text
                    panel.AddChild(new Header("Draggable"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("This panel can be dragged, try it out!"));
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new LineSpace());
                    Paragraph paragraph = new Paragraph("Any type of entity can be dragged. For example, try to drag this text!");
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Yellow));
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Purple), EntityState.MouseHover);
                    paragraph.Draggable = true;
                    paragraph.LimitDraggingToParentBoundaries = false;
                    panel.AddChild(paragraph);

                    // internal panel with internal draggable
                    Panel panelInt = new Panel(new Vector2(250, 250), PanelSkin.Golden, Anchor.AutoCenter);
                    panelInt.Draggable = true;
                    panelInt.AddChild(new Paragraph("This panel is draggable too, but limited to its parent boundaries.", Anchor.Center, Color.White, 0.85f));
                    panel.AddChild(panelInt);
                }

                // example: animators
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(550, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Animators"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Animators are classes that bring UI elements into life by animating them. For example, take a look at these animators:"));

                    // float up-down
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new Paragraph(@"Float Up-Down animator"));
                        entity.AttachAnimator(new GUI.Animators.FloatUpDownAnimator());
                    }

                    // wave animation
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new RichParagraph(@"Wave text animator"));
                        entity.AttachAnimator(new GUI.Animators.TextWaveAnimator());
                    }

                    // fade out
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new Button(@"Fade Out (click to see)"));
                        var animator = entity.AttachAnimator(new GUI.Animators.FadeOutAnimator() { Enabled = false });
                        entity.OnClick += (EntityUI ent) =>
                        {
                            animator.Enabled = true;
                        };
                    }

                    // type writer animator
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new RichParagraph(@""));
                        var animator = entity.AttachAnimator(new GUI.Animators.TypeWriterAnimator()
                        {
                            TextToType = @"This is a type writer animation, text will appear as if someone is typing it in real time. {{YELLOW}}Click on the paragraph to reset animation."
                        });
                        entity.OnClick += (EntityUI ent) =>
                        {
                            animator.Reset();
                        };
                    }
                }

                // example: sliders
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // sliders title
                    panel.AddChild(new Header("Sliders"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Sliders help pick numeric value in range:"));

                    panel.AddChild(new Paragraph("\nDefault slider"));
                    panel.AddChild(new Slider(0, 10, SliderSkin.Default));

                    panel.AddChild(new Paragraph("\nFancy slider"));
                    panel.AddChild(new Slider(0, 10, SliderSkin.Fancy));

                    // progressbar title
                    panel.AddChild(new LineSpace(3));
                    panel.AddChild(new Header("Progress bar"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Works just like sliders:"));
                    panel.AddChild(new ProgressBar(0, 10));
                }

                // example: lists
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("SelectLists let you pick a value from a list of items:"));

                    SelectList list = new SelectList(new Vector2(0, 280));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    list.AddItem("Ranger");
                    list.AddItem("Rogue");
                    list.AddItem("Paladin");
                    list.AddItem("Cleric");
                    list.AddItem("Warlock");
                    list.AddItem("Barbarian");
                    list.AddItem("Monk");
                    list.AddItem("Ranger");
                    panel.AddChild(list);
                }

                // example: list as tables
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(620, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList as a Table"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("With few simple tricks you can also create lists that behave like a table:"));

                    // create the list
                    SelectList list = new SelectList(new Vector2(0, 280));

                    // lock and create title
                    list.LockedItems[0] = true;
                    list.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10}", "{{RED}}", "Name", "Class", "Level"));

                    // add items as formatted table
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Joe", "Mage", "5"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Ron", "Monk", "7"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Alex", "Rogue", "3"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Jim", "Paladin", "7"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Abe", "Cleric", "8"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "James", "Warlock", "20"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Bob", "Bard", "1"));
                    panel.AddChild(list);
                }

                // example: lists skins
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList - Skin"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Just like panels, SelectList can use alternative skins:"));

                    SelectList list = new SelectList(new Vector2(0, 280), skin: PanelSkin.Golden);
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    list.AddItem("Ranger");
                    list.AddItem("Rogue");
                    list.AddItem("Paladin");
                    list.AddItem("Cleric");
                    list.AddItem("Warlock");
                    list.AddItem("Barbarian");
                    list.AddItem("Monk");
                    list.AddItem("Ranger");
                    panel.AddChild(list);
                }

                // example: dropdown
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // dropdown title
                    panel.AddChild(new Header("DropDown"));
                    panel.AddChild(new HorizontalLine());

                    panel.AddChild(new Paragraph("DropDown is just like a list, but take less space since it hide the list when not used:"));
                    DropDown drop = new DropDown(new Vector2(0, 250));
                    drop.AddItem("Warrior");
                    drop.AddItem("Mage");
                    drop.AddItem("Ranger");
                    drop.AddItem("Rogue");
                    drop.AddItem("Paladin");
                    drop.AddItem("Cleric");
                    drop.AddItem("Warlock");
                    drop.AddItem("Barbarian");
                    drop.AddItem("Monk");
                    drop.AddItem("Ranger");
                    panel.AddChild(drop);

                    panel.AddChild(new Paragraph("And like list, we can set different skins:"));
                    drop = new DropDown(new Vector2(0, 180), skin: PanelSkin.Golden);
                    drop.AddItem("Warrior");
                    drop.AddItem("Mage");
                    drop.AddItem("Monk");
                    drop.AddItem("Ranger");
                    panel.AddChild(drop);

                    panel.AddChild(new Paragraph("And per-item styling:"));
                    drop = new DropDown(new Vector2(0, 180), skin: PanelSkin.Golden);
                    drop.AddItem("{{L_RED}}Warrior");
                    drop.AddItem("{{L_BLUE}}Mage");
                    drop.AddItem("{{CYAN}}Monk");
                    drop.AddItem("{{L_GREEN}}Ranger");
                    panel.AddChild(drop);
                }

                // example: panels with scrollbars / overflow
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, 440));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // dropdown title
                    panel.AddChild(new Header("Panel Overflow"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"You can choose how to handle entities that overflow parent panel's boundaries. 

The default behavior is to simply overflow (eg entities will be drawn as usual), but you can also make overflowing entities clip, or make the entire panel scrollable. 

In this example, we use a panel with scrollbars.

Note that in order to use clipping and scrollbar with Panels you need to set the UserInterface.Active.UseRenderTarget flag to true.

Here's a button, to test clicking while scrolled:"));
                    panel.AddChild(new Button("a button."));
                    panel.AddChild(new Paragraph(@"And here's a dropdown:"));
                    var dropdown = new DropDown(new Vector2(0, 220));
                    for (int i = 1; i < 10; ++i) dropdown.AddItem("Option" + i.ToString());
                    panel.AddChild(dropdown);
                    panel.AddChild(new Paragraph(@"And a list:"));
                    var list = new SelectList(new Vector2(0, 220));
                    for (int i = 1; i < 10; ++i) list.AddItem("Option" + i.ToString());
                    panel.AddChild(list);
                    panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
                    panel.Scrollbar.AdjustMaxAutomatically = true;
                    panel.Identifier = "panel_with_scrollbar";
                }

                // example: icons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(460, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // icons title
                    panel.AddChild(new Header("Icons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Built-in icons:"));

                    foreach (IconType icon in System.Enum.GetValues(typeof(IconType)))
                    {
                        if (icon == IconType.None)
                        {
                            continue;
                        }
                        panel.AddChild(new Icon(icon, Anchor.AutoInline));
                    }

                    panel.AddChild(new Paragraph("And you can also add an inventory-like frame:"));
                    panel.AddChild(new LineSpace());
                    for (int i = 0; i < 6; ++i)
                    {
                        panel.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                }

                // example: text input
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Text Input"));
                    panel.AddChild(new HorizontalLine());

                    // inliner
                    panel.AddChild(new Paragraph("Text input let you get free text from the user:"));
                    TextInput text = new TextInput(false);
                    text.PlaceholderText = "Insert text..";
                    panel.AddChild(text);

                    // multiline
                    panel.AddChild(new Paragraph("Text input can also be multiline, and use different panel skins:"));
                    TextInput textMulti = new TextInput(true, new Vector2(0, 220), skin: PanelSkin.Golden);
                    textMulti.PlaceholderText = @"Insert multiline text..";
                    panel.AddChild(textMulti);

                    // with hidden password chars
                    panel.AddChild(new Paragraph("Hidden text input:"));
                    TextInput hiddenText = new TextInput(false);
                    hiddenText.PlaceholderText = "Enter password..";
                    hiddenText.HideInputWithChar = '*';
                    panel.AddChild(hiddenText);
                    var hideCheckbox = new CheckBox("Hide password", isChecked: true);
                    hideCheckbox.OnValueChange += (EntityUI ent) =>
                    {
                        if (hideCheckbox.Checked)
                            hiddenText.HideInputWithChar = '*';
                        else
                            hiddenText.HideInputWithChar = null;
                    };
                    panel.AddChild(hideCheckbox);
                }

                // example: tooltip text
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Tooltip Text"));
                    panel.AddChild(new HorizontalLine());

                    // add entity with tooltip text
                    panel.AddChild(new Paragraph(@"You can attach tooltip text to entities.
This text will be shown when the user points on the entity for few seconds. 

For example, try to point on this button:"));
                    var btn = new Button("Button With Tooltip");
                    btn.ToolTipText = @"This is the button tooltip text!
And yes, it can be multiline.";
                    panel.AddChild(btn);
                    panel.AddChild(new Paragraph(@"Note that you can override the function that generates tooltip text entities if you want to create your own custom style."));
                }

                // example: locked text input
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(500, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Locked Text Input"));
                    panel.AddChild(new HorizontalLine());

                    // inliner
                    panel.AddChild(new Paragraph("A locked multiline text is a cool trick to create long, scrollable text:"));
                    TextInput textMulti = new TextInput(true, new Vector2(0, 370));
                    textMulti.Locked = true;
                    textMulti.TextParagraph.Scale = 0.6f;
                    textMulti.Value = @"The Cleric, Priest, or Bishop is a character class in Dungeons & Dragons and other fantasy role-playing games. 

The cleric is a healer, usually a priest and a holy warrior, originally modeled on or inspired by the Military Orders. 
Clerics are usually members of religious orders, with the original intent being to portray soldiers of sacred orders who have magical abilities, although this role was later taken more clearly by the paladin. 

Most clerics have powers to heal wounds, protect their allies and sometimes resurrect the dead, as well as summon, manipulate and banish undead.

A description of Priests and Priestesses from the Nethack guidebook: Priests and Priestesses are clerics militant, crusaders advancing the cause of righteousness with arms, armor, and arts thaumaturgic. Their ability to commune with deities via prayer occasionally extricates them from peril, but can also put them in it.[1]

A common feature of clerics across many games is that they may not equip pointed weapons such as swords or daggers, and must use blunt weapons such as maces, war-hammers, shields or wand instead. This is based on a popular, but erroneous, interpretation of the depiction of Odo of Bayeux and accompanying text. They are also often limited in what types of armor they can wear, though usually not as restricted as mages.

Related to the cleric is the paladin, who is typically a Lawful Good[citation needed] warrior often aligned with a religious order, and who uses their martial skills to advance its holy cause.";
                    panel.AddChild(textMulti);
                }

                // example: panel tabs
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(540, 440), skin: PanelSkin.None);
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // create panel tabs
                    PanelTabs tabs = new PanelTabs();
                    tabs.BackgroundSkin = PanelSkin.Default;
                    panel.AddChild(tabs);

                    // add first panel
                    {
                        TabData tab = tabs.AddTab("Tab 1");
                        tab.panel.AddChild(new Header("PanelTabs"));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph(@"PanelTab creates a group of internal panels with toggle buttons to switch between them.

Choose a tab in the buttons above for more info..."));
                    }

                    // add second panel
                    {
                        TabData tab = tabs.AddTab("Tab 2");
                        tab.panel.AddChild(new Header("Tab 2"));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph(@"Awesome, you got to tab2!

Maybe something interesting in tab3?"));
                    }

                    // add third panel
                    {
                        TabData tab = tabs.AddTab("Tab 3");
                        tab.panel.AddChild(new Header("Nope."));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph("Nothing to see here."));
                    }
                }

                // example: messages
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Message Box"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("It's easy to create simple message boxes:"));

                    // button to create simple message box
                    {
                        var btn = new Button("Show Simple Message", ButtonSkin.Default);
                        btn.OnClick += (GUI.Entities.EntityUI entity) =>
                        {
                            GUI.Utils.MessageBox.ShowMsgBox("Hello World!", "This is a simple message box. It doesn't say much, really.");
                        };
                        panel.AddChild(btn);
                    }

                    // button to create message box with custombuttons
                    panel.AddChild(new Paragraph("Or you can create custom message and buttons:"));
                    {
                        var btn = new Button("Show Custom Message", ButtonSkin.Default);
                        btn.OnClick += (GUI.Entities.EntityUI entity) =>
                        {
                            GUI.Utils.MessageBox.ShowMsgBox("Custom Message!", "In this message there are two custom buttons.\n\nYou can set different actions per button. For example, click on 'Surprise' and see what happens!", new GUI.Utils.MessageBox.MsgBoxOption[] {
                                new GUI.Utils.MessageBox.MsgBoxOption("Close", () => { return true; }),
                                new GUI.Utils.MessageBox.MsgBoxOption("Surprise", () => { GUI.Utils.MessageBox.ShowMsgBox("Files Removed Successfully", "Win32 was successfully removed from this computer. Please restart to complete OS destruction."); return true; })
                                });
                        };
                        panel.AddChild(btn);
                    }

                    // button to create message with extras
                    panel.AddChild(new Paragraph("And you can also add extra entities to the message box:"));
                    {
                        var btn = new Button("Message With Extras", ButtonSkin.Default);
                        btn.OnClick += (EntityUI entity) =>
                        {
                            var textInput = new TextInput(false);
                            textInput.PlaceholderText = "Enter your name";
                            GUI.Utils.MessageBox.ShowMsgBox("Message With Extra!", "In this message box we attached an extra entity from outside (a simple text input).\n\nPretty neat, huh?", new GUI.Utils.MessageBox.MsgBoxOption[] {
                                new GUI.Utils.MessageBox.MsgBoxOption("Close", () => { return true; }),
                                }, new EntityUI[] { textInput });
                        };
                        panel.AddChild(btn);
                    }
                }

                // example: forms
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(560, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("UI Forms"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Forms are objects that help you generate form-like UI groups and easily extract the data of different fields. For example click the button below: 
                    "));

                    // add create form button
                    var btn = panel.AddChild(new Button(@"Show Form"));
                    btn.OnClick += (EntityUI ent) =>
                    {
                        var newForm = new Form(new FormFieldData[] {
                            new FormFieldData(FormFieldType.TextInput, "text1", "Text Field") { DefaultValue = "Some Default Val" },
                            new FormFieldData(FormFieldType.Slider, "slider1", "Slider Field") { Min = 5, Max = 15, DefaultValue = 10 },
                            new FormFieldData(FormFieldType.RadioButtons, "radios1", "Radio Buttons Field") { Choices = new string[] {"option1", "option2" }, DefaultValue = "option1" },
                            new FormFieldData(FormFieldType.Checkbox, "checkbox1", "Checkbox Field") { },
                            new FormFieldData(FormFieldType.Section, "newsection", "New Form Section") { },
                            new FormFieldData(FormFieldType.DropDown, "dropdown1", "DropDown field") { Choices = new string[] {"option1", "option2", "option3" } },
                        }, null);
                        GUI.Utils.MessageBox.ShowMsgBox("Example Form", "", "Close Form And Show Values", extraEntities: new EntityUI[] { newForm.FormPanel }, onDone: () =>
                        {
                            GUI.Utils.MessageBox.ShowMsgBox("Form Values", string.Format(
                                "Text Field: '{5}{0}{6}'\r\n" +
                                "Slider: '{5}{1}{6}'\r\n" +
                                "Radio Buttons: '{5}{2}{6}'\r\n" +
                                "Checkbox: '{5}{3}{6}'\r\n" +
                                "DropDown: '{5}{4}{6}'",
                                newForm.GetValue("text1"),
                                newForm.GetValue("slider1"),
                                newForm.GetValue("radios1"),
                                newForm.GetValue("checkbox1"),
                                newForm.GetValue("dropdown1"),
                                "{{L_GREEN}}", "{{DEFAULT}}"
                                ));
                        });
                    };
                }

                // example: top menu
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(750, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Top Menu"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Also a classical top menu is possible:"));

                    var layout = new GUI.Utils.MenuBar.MenuLayout();
                    layout.AddMenu("File", 180);
                    layout.AddItemToMenu("File", "New", () => { GUI.Utils.MessageBox.ShowMsgBox("Something New!", "Lets make something new."); });
                    layout.AddItemToMenu("File", "Save", () => { GUI.Utils.MessageBox.ShowMsgBox("Something Saved!", "Your thing was saved successfully."); });
                    layout.AddItemToMenu("File", "Load", () => { GUI.Utils.MessageBox.ShowMsgBox("Something Loaded!", "Your thing was loaded successfully."); });
                    layout.AddItemToMenu("File", "Exit", () => { GUI.Utils.MessageBox.ShowMsgBox("Not Yet", "We still have much to see."); });
                    layout.AddMenu("Display", 220);
                    layout.AddItemToMenu("Display", "Zoom In", () => { UserInterface.Active.GlobalScale += 0.1f; });
                    layout.AddItemToMenu("Display", "Zoom Out", () => { UserInterface.Active.GlobalScale -= 0.1f; });
                    layout.AddItemToMenu("Display", "Reset Zoom", () => { UserInterface.Active.GlobalScale = 1f; });
                    layout.AddMenu("Interactive", 270);
                    layout.AddItemToMenu("Interactive", "Click Me", (GUI.Utils.MenuBar.MenuCallbackContext context) =>
                    {
                        context.Entity.ChangeItem(context.ItemIndex, "I was clicked!");
                    });
                    layout.AddItemToMenu("Interactive", "Toggle Me", (GUI.Utils.MenuBar.MenuCallbackContext context) =>
                    {
                        context.Entity.Tag = context.Entity.Tag == "on" ? "off" : "on";
                        context.Entity.ChangeItem(context.ItemIndex, (context.Entity.Tag == "on" ? "{{L_GREEN}}" : "") + "Toggle Me");
                    });

                    var menuBar = GUI.Utils.MenuBar.Create(layout);
                    menuBar.Anchor = Anchor.Auto;
                    panel.AddChild(menuBar);
                    panel.AddChild(new LineSpace(24));

                    panel.AddChild(new Paragraph("Usually this menu cover the top of the screen and not be inside another panel. Like with most entities, you can also set its skin:"));
                    menuBar = GUI.Utils.MenuBar.Create(layout, PanelSkin.Fancy);
                    menuBar.Anchor = Anchor.Auto;
                    panel.AddChild(menuBar);
                }

                // example: disabled
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(480, 580));
                    panel.Enabled = false;
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // disabled title
                    panel.AddChild(new Header("Disabled"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Entities can be disabled:"));

                    // internal panel
                    Panel panel2 = new Panel(Vector2.Zero, PanelSkin.None, Anchor.Auto);
                    panel2.Padding = Vector2.Zero;
                    panel.AddChild(panel2);
                    panel2.AddChild(new Button("button"));

                    panel2.AddChild(new LineSpace());
                    for (int i = 0; i < 6; ++i)
                    {
                        panel2.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                    panel2.AddChild(new Paragraph("\nDisabled entities are drawn in black & white, and you cannot interact with them.."));

                    SelectList list = new SelectList(new Vector2(0, 130));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    panel2.AddChild(list);
                    panel2.AddChild(new CheckBox("disabled.."));
                }

                // example: Locked
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, 610));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // locked title
                    panel.AddChild(new Header("Locked"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Entities can also be locked:",
                        Anchor.Auto));

                    Panel panel2 = new Panel(Vector2.Zero, PanelSkin.None, Anchor.Auto);
                    panel2.Padding = Vector2.Zero;
                    panel2.Locked = true;

                    panel.AddChild(panel2);
                    panel2.AddChild(new Button("button"));
                    panel2.AddChild(new LineSpace());

                    for (int i = 0; i < 6; ++i)
                    {
                        panel2.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                    panel2.AddChild(new Paragraph("\nLocked entities will not respond to input, but unlike disabled entities they are drawn normally, eg with colors:"));

                    SelectList list = new SelectList(new Vector2(0, 130));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    panel2.AddChild(list);
                    panel2.AddChild(new CheckBox("locked.."));
                }

                // example: Cursors
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Cursor"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("3 basic cursor types included:"));

                    // default cursor show
                    {
                        Button btn = new Button("Default", ButtonSkin.Default);
                        btn.OnMouseEnter = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        btn.OnMouseLeave = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                    // pointer cursor show
                    {
                        Button btn = new Button("Pointer", ButtonSkin.Default);
                        btn.OnMouseEnter = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.Pointer); };
                        btn.OnMouseLeave = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                    // ibeam cursor show
                    {
                        Button btn = new Button("IBeam", ButtonSkin.Default);
                        btn.OnMouseEnter = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
                        btn.OnMouseLeave = (EntityUI entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }
                }

                // example: Misc
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(530, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // misc title
                    panel.AddChild(new Header("Miscellaneous"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Some cool tricks you can do:"));

                    // button with icon
                    Button btn = new Button("Button With Icon");
                    btn.ButtonParagraph.SetAnchorAndOffset(Anchor.CenterLeft, new Vector2(60, 0));
                    btn.AddChild(new Icon(IconType.Book, Anchor.CenterLeft), true);
                    panel.AddChild(btn);

                    // change progressbar color
                    panel.AddChild(new Paragraph("Different ProgressBar colors:"));
                    ProgressBar pb = new ProgressBar();
                    pb.ProgressFill.FillColor = Color.Red;
                    pb.Caption.Text = "Optional caption...";
                    panel.AddChild(pb);

                    // paragraph style with mouse
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    Paragraph paragraph = new Paragraph("Hover / click styling..");
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Purple), EntityState.MouseDown);
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Red), EntityState.MouseHover);
                    panel.AddChild(paragraph);
                    panel.AddChild(new HorizontalLine());

                    // colored rectangle
                    panel.AddChild(new Paragraph("Colored rectangle:"));
                    ColoredRectangle rect = new ColoredRectangle(Color.Blue, Color.Red, 4, new Vector2(0, 40));
                    panel.AddChild(rect);
                    panel.AddChild(new HorizontalLine());
                }

                // example: epilogue
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, 400));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("End Of GUI-DEMO"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new RichParagraph(@"That was only the GUI-DEMO! There is still much to learn about {{MFE_ORANGE}}Monofoxe{{MFE_YELLOW}}.Extended{{DEFAULT}}.

Try more samples by clicking the {{BUTTON_BOLD}}Next{{DEFAULT}} button below.

If you like this engine then don't forget to star the repo on GitHub.

{{MFE_ORANGE}}:){{DEFAULT}}"));
                }

                // init panels and buttons
                UpdateAfterExampleChange();

            }

            // once done init, clear events log
            eventsLog.ClearItems();
        }

        public override void Update()
        {
            base.Update();

            if (targetEntityShow != null)
            {
                targetEntityShow.Text = "Target Entity: " + (UserInterface.Active.TargetEntity != null ? UserInterface.Active.TargetEntity.GetType().Name : "null");
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        /// <summary>
        /// Show next UI example.
        /// </summary>
        public void NextExample()
        {
            currExample++;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Show previous UI example.
        /// </summary>
        public void PreviousExample()
        {
            currExample--;
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
            panels[currExample].Visible = true;

            // disable / enable next and previous buttons
            nextExampleButton.Enabled = currExample != panels.Count - 1;
            previousExampleButton.Enabled = currExample != 0;
        }
    }
}
