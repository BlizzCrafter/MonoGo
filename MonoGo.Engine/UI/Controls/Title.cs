using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A title text Control.
    /// Same as a paragraph, but with different defaults values and stylesheet.
    /// </summary>
    public class Title : Paragraph
    {
        /// <summary>
        /// Create the title.
        /// </summary>
        /// <param name="stylesheet">Title stylesheet.</param>
        /// <param name="text">Title text.</param>
        public Title(StyleSheet? stylesheet, string text = "New Title", bool ignoreInteractions = true) : base(stylesheet, text, ignoreInteractions)
        {
        }

        /// <summary>
        /// Create the title with default stylesheets.
        /// </summary>
        /// <param name="text">Title text.</param>
        public Title(string text = "New Title", bool ignoreInteractions = true) : this(UISystem.DefaultStylesheets.Titles, text, ignoreInteractions)
        {
        }
    }
}
