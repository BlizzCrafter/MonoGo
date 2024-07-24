
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// A texture to render as icon, with size based on source rectangle.
    /// </summary>
    public class IconTexture
    {
        /// <summary>
        /// Texture identifier.
        /// </summary>
        public string TextureId { get; set; } = null!;

        /// <summary>
        /// Texture override.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The source rectangle of the texture to draw.
        /// </summary>
        public Rectangle SourceRect { get; set; }

        /// <summary>
        /// Will scale icon by this factor.
        /// </summary>
        public float TextureScale { get; set; } = 1f;

        public IconTexture DeepCopy()
        {
            IconTexture copy = (IconTexture)MemberwiseClone();
            copy.TextureId = new string(TextureId);
            copy.SourceRect = new Rectangle(SourceRect.X, SourceRect.Y, SourceRect.Width, SourceRect.Height);
            copy.TextureScale = TextureScale;
            return copy;
        }
    }
}
