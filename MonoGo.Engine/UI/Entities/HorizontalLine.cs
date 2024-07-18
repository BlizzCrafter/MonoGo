using MonoGo.Engine.UI.Defs;


namespace MonoGo.Engine.UI.Entities
{
    /// <summary>
    /// A graphical horizontal line that separates between entities.
    /// </summary>
    public class HorizontalLine : EntityUI
    {
        /// <summary>
        /// Create the horizontal line.
        /// </summary>
        /// <param name="stylesheet">Horizontal line stylesheet.</param>
        public HorizontalLine(StyleSheet? stylesheet) : base(stylesheet)
        {
        }

        /// <summary>
        /// Create the horizontal line with default stylesheets.
        /// </summary>
        public HorizontalLine() : this(UISystem.DefaultStylesheets.HorizontalLines)
        {
        }

        /// <inheritdoc/>
        protected override Anchor GetDefaultEntityTypeAnchor()
        {
            return Anchor.AutoCenter;
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultEntityTypeSize()
        {
            var ret = new MeasureVector();
            ret.X.SetPercents(100f);
            ret.Y.SetPixels(8);
            return ret;
        }
    }
}
