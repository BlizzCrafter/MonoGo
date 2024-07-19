using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Button entity type.
    /// </summary>
    public class Button : CheckedControl
    {
        /// <summary>
        /// Button label.
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <inheritdoc/>
        internal override bool Interactable => true;

        /// <summary>
        /// Create the button.
        /// </summary>
        /// <param name="stylesheet">Button stylesheet.</param>
        /// <param name="text">Button text.</param>
        public Button(StyleSheet? stylesheet, string text = "New Button") : base(stylesheet)
        {
            // create the button paragraph
            Paragraph = new Paragraph(stylesheet, text);
            Paragraph.DrawFillTexture = false;
            AddChildInternal(Paragraph);
            Paragraph.CopyStateFrom = this;
            OverflowMode = OverflowMode.HideOverflow;
        }

        /// <summary>
        /// Create the button with default stylesheets.
        /// </summary>
        /// <param name="text">Button text.</param>
        public Button(string text = "New Button") : this(UISystem.DefaultStylesheets.Buttons, text)
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
