using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;
using MonoGo.Engine.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MonoGo.Engine.UI
{
    /// <summary>
    /// A GUI system instance.
    /// </summary>
    public static class UISystem
    {
        public static string ThemeFolder { get; private set; }

        /// <summary>
        /// Total elapsed time this system is running, in seconds.
        /// </summary>
        public static double ElapsedTime { get; private set; }

        /// <summary>
        /// Last update frame delta time, in seconds.
        /// </summary>
        public static float LastDeltaTime { get; private set; }

        /// <summary>
        /// If true, will render UI cursor (if defined in stylesheet).
        /// </summary>
        public static bool ShowCursor = true;

        /// <summary>
        /// Default stylesheets to use for different entity types when no stylesheet is provided.
        /// </summary>
        public static class DefaultStylesheets
        {
            public static StyleSheet? Panels;
            public static StyleSheet? Paragraphs;
            public static StyleSheet? Titles;
            public static StyleSheet? Buttons;
            public static StyleSheet? HorizontalLines;
            public static StyleSheet? CheckBoxes;
            public static StyleSheet? RadioButtons;
            public static StyleSheet? HorizontalSliders;
            public static StyleSheet? HorizontalSlidersHandle;
            public static StyleSheet? VerticalSliders;
            public static StyleSheet? VerticalSlidersHandle;
            public static StyleSheet? ListPanels;
            public static StyleSheet? ListItems;
            public static StyleSheet? DropDownPanels;
            public static StyleSheet? DropDownItems;
            public static StyleSheet? VerticalScrollbars;
            public static StyleSheet? VerticalScrollbarsHandle;
            public static StyleSheet? TextInput;
            public static StyleSheet? HorizontalProgressBars;
            public static StyleSheet? HorizontalProgressBarsFill;
            public static StyleSheet? VerticalProgressBars;
            public static StyleSheet? VerticalProgressBarsFill;
        }

        /// <summary>
        /// Currently-targeted entity (entity we point on with the cursor).
        /// </summary>
        public static Control? TargetedControl { get; private set; }

        /// <summary>
        /// System-level stylesheet.
        /// Define properties like cursor graphics and general stuff.
        /// </summary>
        public static SystemStyleSheet SystemStyleSheet = new SystemStyleSheet();

        /// <summary>
        /// If true, will debug-render controls.
        /// </summary>
        public static bool DebugRenderControls = false;

        /// <summary>
        /// Control events you can register to.
        /// These events will trigger for any entity in the system.
        /// </summary>
        public static ControlEvents Events;

        /// <summary>
        /// Root entity.
        /// All child controls should be added to this object.
        /// </summary>
        public static Panel Root { get; private set; }

        // queue of scissor regions to cut-off rendering when overflow mode is hidden.
        internal static Queue<Rectangle> _scissorRegionQueue = new();

        /// <summary>
        /// When controls turn into interactive state (for example a button is clicked on), it will be locked in this state for at least this time, in seconds.
        /// This property is useful to make sure the interactive state is properly shown, even if user perform very rapid short clicks.
        /// </summary>
        /// <remarks>This property is especially important when there's interpolation on texture change, and switching to interactive state is not immediate.</remarks>
        internal static float TimeToLockInteractiveState => SystemStyleSheet.TimeToLockInteractiveState;

        /// <param name="themeFolder">UI System theme folder path.</param>
        public static void Init(string themeFolder)
        {
            ThemeFolder = Path.Combine(themeFolder, "DefaultTheme");
            var defaultStyleSheetFilePath = Path.Combine(ThemeFolder, "system_style.json");

            // create renderer and input
            Renderer.Init(ThemeFolder);

            // create root entity
            Root = new Panel(new StyleSheet()) { Identifier = "Root" };

            try
            {
                SystemStyleSheet = JsonSerializer.Deserialize<SystemStyleSheet>(
                    File.ReadAllText(defaultStyleSheetFilePath), JsonConverters.SerializerOptions)!;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read or deserialize UI system stylesheet!", e);
            }

            if (SystemStyleSheet.LoadDefaultStylesheets != null)
            {
                LoadDefaultStylesheets(SystemStyleSheet.LoadDefaultStylesheets, Path.GetDirectoryName(defaultStyleSheetFilePath) ?? string.Empty);
            }
        }

        /// <summary>
        /// Load all default stylesheets from dictionary.
        /// </summary>
        /// <param name="stylesheetsToLoad">Stylesheets to load. Key = stylesheet entity name, Value = path to load from.</param>
        /// <param name="parentFolder">Folder to load stylesheet files from.</param>
        public static void LoadDefaultStylesheets(Dictionary<string, string> stylesheetsToLoad, string parentFolder)
        {
            foreach (var pair in stylesheetsToLoad)
            {
                var entityStyleName = pair.Key;
                var path = pair.Value;

                var field = typeof(DefaultStylesheets).GetField(entityStyleName, BindingFlags.Static | BindingFlags.Public);
                if (field == null)
                {
                    throw new FormatException($"Error loading stylesheet for entity style id '{entityStyleName}': entity key not found under 'DefaultStylesheets'.");
                }

                var fullPath = Path.Combine(parentFolder, path);
                try
                {
                    var stylesheet = StyleSheet.LoadFromJsonFile(fullPath);
                    field.SetValue(null, stylesheet);
                }
                catch (FileNotFoundException)
                {
                    throw new FormatException($"Error loading stylesheet for entity style id '{entityStyleName}': stylesheet file '{fullPath}' not found!");
                }
            }
        }

        public static void Clear()
        {
            foreach (var child in Root._children.ToList())
            {
                child.RemoveSelf();
            }
        }

        /// <summary>
        /// Perform UI updates.
        /// Must be called every update frame in your game main loop.
        /// </summary>
        public static void Update()
        {
            var deltaTime = (float)GameMgr.ElapsedTime;

            // update elapsed time
            LastDeltaTime = deltaTime;
            ElapsedTime += deltaTime;

            // set root to cover entire screen
            var screenBounds = Renderer.GetScreenBounds();
            Root.Size.SetPixels(screenBounds.Width, screenBounds.Height);

            // update all controls
            Root._DoUpdate(deltaTime);

            // check if should lock target entity
            bool keepTargetControl = (TargetedControl != null) ? 
                (TargetedControl.LockFocusOnSelf && TargetedControl.IsCurrentlyVisible() && !TargetedControl.IsCurrentlyLocked() && !TargetedControl.IsCurrentlyDisabled()) 
                : false;

            // if dragging an entity, we can't lose the target
            if (Input.CheckButton(Buttons.MouseLeft) && (TargetedControl != null) && TargetedControl.LockFocusWhileMouseDown)
            {
                keepTargetControl = true;
            }

            // current mouse position
            var cp = Input.ScreenMousePosition.ToPoint();

            // find new entity we target
            if (!keepTargetControl)
            {
                // reset target entity
                TargetedControl = null;

                // iterate all controls to see which entity we point on
                List<Control> controlsToPostProcess = new List<Control>();
                Root.Walk((Control entity) =>
                {
                    // skip controls that are without interactions
                    // note: we don't want to skip locked or disabled controls because they can still 'block' other controls.
                    if (entity.IgnoreInteractions)
                    {
                        return true;
                    }

                    // check if entity can't get focused while mouse is down
                    if (!entity.CanGetFocusWhileMouseIsDown && Input.CheckButton(Buttons.MouseLeft))
                    {
                        return true;
                    }

                    // check if top most interactions
                    if (entity.TopMostInteractions)
                    {
                        controlsToPostProcess.Add(entity);
                        return true;
                    }

                    // check if we point on this entity
                    if (entity.IsCurrentlyVisible() && entity.IsPointedOn(cp))
                    {
                        TargetedControl = entity;
                    }

                    // continue iteration
                    return true;
                });

                // do top-most interactions
                foreach (var entity in controlsToPostProcess)
                {
                    if (entity.IsCurrentlyVisible() && entity.IsPointedOn(cp))
                    {
                        TargetedControl = entity;
                    }
                }
            }

            // calculate current input state
            var currInputState = new CurrentInputState()
            {
                MousePosition = cp,
                LeftMouseButton = Input.CheckButton(Buttons.MouseLeft),
                RightMouseButton = Input.CheckButton(Buttons.MouseRight),
                WheelMouseButton = Input.CheckButton(Buttons.MouseMiddle),
                MouseWheelChange = Input.MouseWheelValue,
                TextInputCommands = TextInputCmd.GetTextInputCommands()
            };
            var inputState = new InputState()
            {
                _Previous = _lastInputState,
                _Current = currInputState,
                ScreenBounds = Renderer.GetScreenBounds()
            };
            _lastInputState = currInputState;
            CurrentInputState = inputState;

            // do interactions with targeted entity
            // unless its locked or disabled
            if (TargetedControl != null)
            {
                if (TargetedControl.TransferInteractionsTo != null)
                {
                    TargetedControl = TargetedControl.TransferInteractionsTo;
                }

                if (!TargetedControl.IsCurrentlyLocked() && !TargetedControl.IsCurrentlyDisabled())
                {
                    TargetedControl.DoInteractions(inputState);
                }
            }

            // do post interactions
            Root.Walk((Control entity) =>
            {
                if (entity.Interactable)
                {
                    entity.PostUpdate(inputState);
                }
                return true;
            });
        }

        // last frame input state
        private static CurrentInputState _lastInputState;

        /// <summary>
        /// Get current input state.
        /// </summary>
        public static InputState CurrentInputState { get; private set; }

        /// <summary>
        /// Add action to call after rendering controls.
        /// </summary>
        internal static void RunAfterDrawingControls(Action action)
        {
            _postDrawActions.Add(action);
        }
        private static List<Action> _postDrawActions = new List<Action>();

        /// <summary>
        /// Render the UI.
        /// Must be called every draw frame in your game rendering loop.
        /// </summary>
        /// <remarks>
        /// Clearing screen, setting render target, or using a 'camera' like object for zooming, is up to the host application.
        /// </remarks>
        public static void Draw()
        {
            Renderer.StartFrame();

            // reset scissors queue
            _scissorRegionQueue.Clear();
            Renderer.ClearScissorRegion();

            // draw all controls
            var screenRect = Renderer.GetScreenBounds();
            var rootDrawResult = new Control.DrawMethodResult()
            {
                BoundingRect = screenRect,
                InternalBoundingRect = screenRect
            };
            Root._DoDraw(rootDrawResult, null);

            // call post-draw actions
            if (_postDrawActions.Count > 0)
            {
                foreach (var action in _postDrawActions)
                {
                    action();
                }
                _postDrawActions.Clear();
            }

            // get which cursor to render
            CursorProperties? cursor = SystemStyleSheet.CursorDefault;
            if (TargetedControl?.IsPointedOn(Input.ScreenMousePosition.ToPoint(), true) ?? false)
            {
                if (TargetedControl.CursorStyle != null)
                {
                    cursor = TargetedControl.CursorStyle;
                }
                else if (TargetedControl.IsCurrentlyDisabled())
                {
                    cursor = SystemStyleSheet.CursorDisabled ?? SystemStyleSheet.CursorDefault;
                }
                else if (TargetedControl.IsCurrentlyLocked())
                {
                    cursor = SystemStyleSheet.CursorLocked ?? SystemStyleSheet.CursorDefault;
                }
                else if (TargetedControl.Interactable) 
                { 
                    cursor = SystemStyleSheet.CursorInteractable ?? SystemStyleSheet.CursorDefault; 
                }
            }

            // debug draw stuff
            if (DebugRenderControls)
            {
                Root.DebugDraw(true);
            }

            // render cursor
            if (ShowCursor && cursor != null)
            {
                var destRect = cursor.SourceRect;
                destRect.X = (int)Input.ScreenMousePosition.X + (int)(cursor.Offset.X * cursor.Scale);
                destRect.Y = (int)Input.ScreenMousePosition.Y + (int)(cursor.Offset.Y * cursor.Scale);
                destRect.Width = (int)(destRect.Width * cursor.Scale);
                destRect.Height = (int)(destRect.Height * cursor.Scale);
                Renderer.DrawTexture(cursor.EffectIdentifier, cursor.TextureId, destRect, cursor.SourceRect, cursor.FillColor);
            }

            Renderer.EndFrame();
        }
    }
}
