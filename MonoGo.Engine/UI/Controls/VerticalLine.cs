using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A graphical vertical line that separates between entities.
    /// </summary>
    public class VerticalLine : Control
    {
        /// <summary>
        /// Create the vertical line.
        /// </summary>
        /// <param name="stylesheet">Vertical line stylesheet.</param>
        public VerticalLine(StyleSheet? stylesheet) : base(stylesheet)
        {
            IgnoreInteractions = true;
        }

        /// <summary>
        /// Create the vertical line with default stylesheets.
        /// </summary>
        public VerticalLine() : this(UISystem.DefaultStylesheets.VerticalLines)
        {
        }

        /// <inheritdoc/>
        protected override Anchor GetDefaultEntityTypeAnchor()
        {
            return Anchor.AutoInlineLTR;
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultEntityTypeSize()
        {
            var ret = new MeasureVector();
            ret.X.SetPixels(8);
            ret.Y.SetPercents(100f);
            return ret;
        }
    }
}
