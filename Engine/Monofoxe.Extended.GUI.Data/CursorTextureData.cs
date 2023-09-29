﻿namespace Monofoxe.Extended.GUI.Data
{
    /// <summary>
    /// Meta data we attach to cursor textures.
    /// The values of these structs are defined in xml files that share the same name as the texture with _md sufix.
    /// </summary>
    public class CursorTextureData
    {
        /// <summary>Cursor offset from mouse position, on X axis, in texture pixels.</summary>
        public int OffsetX = 0;

        /// <summary>Cursor offset from mouse position, on Y axis, in texture pixels.</summary>
        public int OffsetY = 0;

        /// <summary>Width, in pixels, to draw this cursor. The height will be calculated automatically to fit texture propotions.</summary>
        public int DrawWidth = 64;
    }
}
