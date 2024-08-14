using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace MonoGo.Engine.UI.Defs
{
    public enum TextInputCommands
    {
        MoveCaretLeft,
        MoveCaretRight,
        MoveCaretUp,
        MoveCaretDown,
        Backspace,
        Delete,
        BreakLine,
        MoveCaretEnd,
        MoveCaretStart,
        MoveCaretEndOfLine,
        MoveCaretStartOfLine,
    }

    /// <summary>
    /// All input properties for controls interactions.
    /// </summary>
    public struct InputState
    {
        /// <summary>
        /// Current frame state.
        /// </summary>
        internal CurrentInputState _Current;

        /// <summary>
        /// Previous frame state.
        /// </summary>
        internal CurrentInputState _Previous;

        /// <summary>
        /// Is mouse left button is currently down.
        /// </summary>
        public bool LeftMouseDown => _Current.LeftMouseButton;

        /// <summary>
        /// Is mouse right button is currently down.
        /// </summary>
        public bool RightMouseDown => _Current.RightMouseButton;

        /// <summary>
        /// Is mouse wheel button is currently down.
        /// </summary>
        public bool WheelMouseDown => _Current.WheelMouseButton;

        /// <summary>
        /// Was left mouse button pressed this frame.
        /// </summary>
        public bool LeftMousePressedNow => _Current.LeftMouseButton && !_Previous.LeftMouseButton;

        /// <summary>
        /// Was right mouse button pressed this frame.
        /// </summary>
        public bool RightMousePressedNow => _Current.RightMouseButton && !_Previous.RightMouseButton;

        /// <summary>
        /// Was mouse wheel button pressed this frame.
        /// </summary>
        public bool WheelMousePressedNow => _Current.WheelMouseButton && !_Previous.WheelMouseButton;

        /// <summary>
        /// Was left mouse button released this frame.
        /// </summary>
        public bool LeftMouseReleasedNow => !_Current.LeftMouseButton && _Previous.LeftMouseButton;

        /// <summary>
        /// Was right mouse button released this frame.
        /// </summary>
        public bool RightMouseReleasedNow => !_Current.RightMouseButton && _Previous.RightMouseButton;

        /// <summary>
        /// Was mouse wheel button released this frame.
        /// </summary>
        public bool WheelMouseReleasedNow => !_Current.WheelMouseButton && _Previous.WheelMouseButton;

        /// <summary>
        /// Mouse wheel change.
        /// </summary>
        public int MouseWheelChange => _Current.MouseWheelChange;

        /// <summary>
        /// Current mouse position.
        /// </summary>
        public Point MousePosition => _Current.MousePosition;

        /// <summary>
        /// Mouse movement this frame.
        /// </summary>
        public Point MouseMove => new Point(_Current.MousePosition.X - _Previous.MousePosition.X, _Current.MousePosition.Y - _Previous.MousePosition.Y);

        /// <summary>
        /// Get current frame text input commands.
        /// </summary>
        public TextInputCommands[] TextInputCommands => _Current.TextInputCommands;

        /// <summary>
        /// Current screen bounds.
        /// </summary>
        public Rectangle ScreenBounds;
    }

    /// <summary>
    /// Input state for a specific frame.
    /// </summary>
    public struct CurrentInputState
    {
        public bool LeftMouseButton;
        public bool RightMouseButton;
        public bool WheelMouseButton;
        public Point MousePosition;
        public int MouseWheelChange;
        public int[] TextInput;
        public TextInputCommands[] TextInputCommands;
    }

    internal struct TextInputCmd
    {
        static long[] _timeToAllowNextInputCommand = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static Keys[] _inputTextCommandToKeyboardKey = new Keys[]
        {
           Keys.Left,
           Keys.Right,
           Keys.Up,
           Keys.Down,
           Keys.Back,
           Keys.Delete,
           Keys.Enter,
           Keys.End,
           Keys.Home,
           Keys.End,
           Keys.Home
        };

        internal static TextInputCommands[] GetTextInputCommands()
        {
            List<TextInputCommands> ret = new();
            var keyboard = Keyboard.GetState();
            var ctrlDown = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            {
                foreach (var value in Enum.GetValues(typeof(TextInputCommands)))
                {
                    var key = _inputTextCommandToKeyboardKey[(int)value];
                    long msPassed = millisecondsSinceEpoch - _timeToAllowNextInputCommand[(int)value];
                    if (keyboard.IsKeyDown(key))
                    {
                        if (msPassed > 0)
                        {
                            _timeToAllowNextInputCommand[(int)value] = (millisecondsSinceEpoch + (msPassed >= 250 ? 450 : 45));
                            var command = (TextInputCommands)value;
                            if ((command == TextInputCommands.MoveCaretEnd) && !ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretEndOfLine) && ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretStart) && !ctrlDown) { continue; }
                            if ((command == TextInputCommands.MoveCaretStartOfLine) && ctrlDown) { continue; }
                            ret.Add(command);
                        }
                    }
                    else
                    {
                        _timeToAllowNextInputCommand[(int)value] = 0;
                    }
                }
            }
            return ret.ToArray();
        }
    }
}
