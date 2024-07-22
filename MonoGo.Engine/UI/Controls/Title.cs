using MonoGo.Engine.EC;
using MonoGo.Engine.UI.Defs;


namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A title text entity.
    /// Same as a paragraph, but with different defaults values and stylesheet.
    /// </summary>
    public class Title : Paragraph
    {
        /// <summary>
        /// Create the title.
        /// </summary>
        /// <param name="stylesheet">Title stylesheet.</param>
        /// <param name="text">Title text.</param>
        public Title(StyleSheet? stylesheet, string text = "New Title", IHaveGUI? owner = null) : base(stylesheet, text, owner)
        {
        }

        /// <summary>
        /// Create the title with default stylesheets.
        /// </summary>
        /// <param name="text">Title text.</param>
        public Title(string text = "New Title", IHaveGUI? owner = null) : this(UISystem.DefaultStylesheets.Titles, text, owner)
        {
        }
    }
}
