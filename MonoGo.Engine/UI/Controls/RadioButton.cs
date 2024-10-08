﻿using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Radio button Control type.
    /// </summary>
    public class RadioButton : CheckedControl
    {
        /// <summary>
        /// Radio button label.
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <inheritdoc/>
        internal override bool Interactable => true;

        /// <summary>
        /// Create the radio button.
        /// </summary>
        /// <param name="stylesheet">Radio button stylesheet.</param>
        /// <param name="text">Radio button text.</param>
        public RadioButton(StyleSheet? stylesheet, string text = "New Radio Button") : base(stylesheet)
        {
            // create the radio button paragraph
            Paragraph = new Paragraph(stylesheet, text);
            Paragraph.DrawFillTexture = false;
            AddChildInternal(Paragraph);
            Paragraph.CopyStateFrom = this;

            // make checkable
            ToggleCheckOnClick = true;
            CanClickToUncheck = false;
            ExclusiveSelection = true;
        }

        /// <summary>
        /// Create the radio button with default stylesheets.
        /// </summary>
        /// <param name="text">Radio button text.</param>
        public RadioButton(string text = "New Radio Button") : 
            this(UISystem.DefaultStylesheets.RadioButtons ?? UISystem.DefaultStylesheets.CheckBoxes, text)
        {
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultControlTypeSize()
        {
            var ret = new MeasureVector();
            ret.X.SetPercents(100f);
            ret.Y.SetPixels(54);
            return ret;
        }
    }
}
