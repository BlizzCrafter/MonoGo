using MonoGo.Engine.EC;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Checkbox entity type.
    /// </summary>
    public class Checkbox : CheckedControl
    {
        /// <summary>
        /// Checkbox label.
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <inheritdoc/>
        internal override bool Interactable => true;

        /// <summary>
        /// Create the checkbox.
        /// </summary>
        /// <param name="stylesheet">Checkbox stylesheet.</param>
        /// <param name="text">Checkbox text.</param>
        public Checkbox(StyleSheet? stylesheet, string text = "New Checkbox") : base(stylesheet)
        {
            // create the checkbox paragraph
            Paragraph = new Paragraph(stylesheet, text);
            Paragraph.DrawFillTexture = false;
            AddChildInternal(Paragraph);
            Paragraph.CopyStateFrom = this;

            // make checkable
            ToggleCheckOnClick = true;
            CanClickToUncheck = true;

        }

        /// <summary>
        /// Create the checkbox with default stylesheets.
        /// </summary>
        /// <param name="text">Checkbox text.</param>
        public Checkbox(string text = "New Button") : this(UISystem.DefaultStylesheets.CheckBoxes, text)
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
