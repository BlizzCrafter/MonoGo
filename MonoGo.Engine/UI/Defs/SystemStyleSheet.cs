﻿using System.Collections.Generic;

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// System-level stylesheet for UI.
    /// This define general properties that are not related to any specific Control.
    /// </summary>
    public class SystemStyleSheet
    {
        /// <summary>
        /// UI Theme identifier.
        /// </summary>
        public string ThemeIdentifier { get; set; } = null!;
        /// <summary>
        /// Will scale all fonts in the UI system by this value.
        /// </summary>
        public float TextScale { get; set; } = 1f;

        /// <summary>
        /// Will scale all the cursor textures by this value.
        /// </summary>
        public float CursorScale { get; set; } = 1f;

        /// <summary>
        /// Cursor to render in default state.
        /// </summary>
        public CursorProperties? CursorDefault { get; set; }

        /// <summary>
        /// Cursor to render while pointing on an interactable Control that is not locked or disabled.
        /// </summary>
        public CursorProperties? CursorInteractable { get; set; }

        /// <summary>
        /// Cursor to render while pointing on a disabled Control.
        /// </summary>
        public CursorProperties? CursorDisabled { get; set; }

        /// <summary>
        /// Cursor to render while pointing on a locked Control.
        /// </summary>
        public CursorProperties? CursorLocked { get; set; }

        /// <summary>
        /// How much space a row spacer unit takes, in pixels.
        /// This determine what the UI system defines as a default empty "row" size.
        /// </summary>
        public int RowSpaceHeight { get; set; } = 14;
        /// <summary>
        /// Lock controls to interactive state for at least this value in seconds, to make sure the 'interactive' state is properly displayed even for rapid clicks.
        /// </summary>
        public float TimeToLockInteractiveState { get; set; }

        /// <summary>
        /// Default stylesheets to load for controls.
        /// Key = name of stylesheet to load (for example 'Panels' for 'uiSystem.DefaultStylesheets.Panels'.
        /// Value = path, relative to the folder containing this stylesheet, to load from.
        /// </summary>
        public Dictionary<string, string> LoadDefaultStylesheets { get; set; } = null!;
    }
}
