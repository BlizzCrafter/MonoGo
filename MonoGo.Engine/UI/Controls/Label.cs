using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A label text Control.
    /// Same as a paragraph, but with different defaults values and stylesheet.
    /// </summary>
    public class Label : Paragraph
    {
        /// <summary>
        /// Create the label.
        /// </summary>
        /// <param name="stylesheet">Label stylesheet.</param>
        /// <param name="text">Label text.</param>
        /// <param name="ignoreInteractions">If true, this label will ignore user interactions.</param>
        public Label(StyleSheet? stylesheet, string text = "New Label", bool ignoreInteractions = true) : base(stylesheet, text, ignoreInteractions)
        {
        }

        /// <summary>
        /// Create the label with default stylesheets.
        /// </summary>
        /// <param name="system">Parent UI system.</param>
        /// <param name="text">Label text.</param>
        /// <param name="ignoreInteractions">If true, this label will ignore user interactions.</param>
        public Label(string text = "New Label", bool ignoreInteractions = true) : this(UISystem.DefaultStylesheets.Labels, text, ignoreInteractions)
        {
        }
    }
}
