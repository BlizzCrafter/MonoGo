using MonoGo.Engine.EC;
using MonoGo.Engine.UI.Defs;


namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A graphical horizontal line that separates between controls.
    /// </summary>
    public class HorizontalLine : Control
    {
        /// <summary>
        /// Create the horizontal line.
        /// </summary>
        /// <param name="stylesheet">Horizontal line stylesheet.</param>
        public HorizontalLine(StyleSheet? stylesheet, Entity? owner = null) : base(stylesheet, owner)
        {
        }

        /// <summary>
        /// Create the horizontal line with default stylesheets.
        /// </summary>
        public HorizontalLine(Entity? owner = null) : this(UISystem.DefaultStylesheets.HorizontalLines, owner)
        {
        }

        /// <inheritdoc/>
        protected override Anchor GetDefaultControlTypeAnchor()
        {
            return Anchor.AutoCenter;
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultControlTypeSize()
        {
            var ret = new MeasureVector();
            ret.X.SetPercents(100f);
            ret.Y.SetPixels(8);
            return ret;
        }
    }
}
