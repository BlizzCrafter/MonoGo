﻿using Microsoft.Xna.Framework;

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// A texture to render stretched over the given region.
    /// </summary>
    public class StretchedTexture
    {
        /// <summary>
        /// Texture identifier.
        /// </summary>
        public string TextureId { get; set; } = null!;

        /// <summary>
        /// The source rectangle of the texture to draw.
        /// </summary>
        public Rectangle SourceRect { get; set; }

        /// <summary>
        /// Add extra size to the sides of this texture when rendering it.
        /// </summary>
        public Sides? ExtraSize { get; set; }

        public StretchedTexture DeepCopy()
        {
            StretchedTexture copy = (StretchedTexture)MemberwiseClone ();
            copy.TextureId = new string(TextureId);
            copy.SourceRect = new Rectangle(SourceRect.X, SourceRect.Y, SourceRect.Width, SourceRect.Height);
            copy.ExtraSize = ExtraSize.HasValue ? new Sides(ExtraSize.Value.Left, ExtraSize.Value.Right, ExtraSize.Value.Top, ExtraSize.Value.Bottom) : null;
            return copy;
        }
    }
}
