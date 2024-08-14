﻿using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Utils
{
    /// <summary>
    /// Render utilities.
    /// </summary>
    internal static class DrawUtils
    {
        /// <summary>
        /// Draw a horizontal strip texture with sides.
        /// </summary>
        static void DrawHorizontal(string textureId, float textureScale, string? effectId, Rectangle leftSrc, Rectangle centerSrc, Rectangle rightSrc, float width, Point position, Color color)
        {
            // left and right dest rects
            var leftDest = new Rectangle(position.X, position.Y, (int)(leftSrc.Width * textureScale), (int)(leftSrc.Height * textureScale));
            var rightDest = new Rectangle(position.X + (int)(width - (rightSrc.Width * textureScale)), position.Y, (int)(rightSrc.Width * textureScale), (int)(rightSrc.Height * textureScale));

            // draw center parts
            var centerDest = new Rectangle(position.X + leftDest.Width, position.Y, (int)(centerSrc.Width * textureScale), (int)(centerSrc.Height * textureScale));
            bool didFinish = false;
            while (!didFinish)
            {
                var cutOff = (centerDest.X + centerDest.Width) - (rightDest.X);
                if (cutOff > 0)
                {
                    centerDest.Width -= cutOff;
                    centerSrc.Width -= (int)((float)cutOff / textureScale);
                    didFinish = true;
                }
                if (centerDest.Width > 0)
                {
                    Renderer.DrawTexture(effectId, textureId, centerDest, centerSrc, color);
                    centerDest.X += centerDest.Width;
                }
            }

            // draw left side
            Renderer.DrawTexture(effectId, textureId, leftDest, leftSrc, color);

            // draw right side
            Renderer.DrawTexture(effectId, textureId, rightDest, rightSrc, color);
        }

        /// <summary>
        /// Draw a vertical strip texture with sides.
        /// </summary>
        static void DrawVertical(string textureId, float textureScale, string? effectId, Rectangle topSrc, Rectangle centerSrc, Rectangle bottomSrc, float height, Point position, Color color)
        {
            // top and bottom dest rects
            var topDest = new Rectangle(position.X, position.Y, (int)(topSrc.Width * textureScale), (int)(topSrc.Height * textureScale));
            var bottomDest = new Rectangle(position.X, position.Y + (int)(height - (bottomSrc.Height * textureScale)), (int)(bottomSrc.Width * textureScale), (int)(bottomSrc.Height * textureScale));

            // draw center parts
            var centerDest = new Rectangle(position.X, position.Y + topDest.Height, (int)(centerSrc.Width * textureScale), (int)(centerSrc.Height * textureScale));
            bool didFinish = false;
            while (!didFinish)
            {
                var cutOff = (centerDest.Y + centerDest.Height) - (bottomDest.Y);
                if (cutOff > 0)
                {
                    centerDest.Height -= cutOff;
                    centerSrc.Height -= (int)((float)cutOff / textureScale);
                    didFinish = true;
                }
                if (centerDest.Height > 0)
                {
                    Renderer.DrawTexture(effectId, textureId, centerDest, centerSrc, color);
                    centerDest.Y += centerDest.Height;
                }
            }

            // draw top side
            Renderer.DrawTexture(effectId, textureId, topDest, topSrc, color);

            // draw bottom side
            Renderer.DrawTexture(effectId, textureId, bottomDest, bottomSrc, color);
        }
        /// <summary>
        /// Render framed texture.
        /// </summary>
        public static void Draw(string? effectId, FramedTexture texture, Rectangle dest, Color color)
        {
            // to avoid glitches
            if (dest.Width <= 0 || dest.Height <= 0) { return; }

            // get all source rects
            var topLeftSrc = texture.TopLeftSourceRect;
            var topRightSrc = texture.TopRightSourceRect;
            var bottomLeftSrc = texture.BottomLeftSourceRect;
            var bottomRightSrc = texture.BottomRightSourceRect;

            // add offset
            dest.X += texture.Offset.X;
            dest.Y += texture.Offset.Y;

            // is this a horizontal strip only?
            bool isHorizontalStrip = texture.InternalSourceRect.Height == texture.ExternalSourceRect.Height;
            if (isHorizontalStrip)
            {
                DrawHorizontal(texture.TextureId, texture.TextureScale, effectId, texture.LeftSourceRect, texture.InternalSourceRect, texture.RightSourceRect, dest.Width, new Point(dest.X, dest.Y), color);
                return;
            }
            bool isVerticalStrip = texture.InternalSourceRect.Width == texture.ExternalSourceRect.Width;
            if (isVerticalStrip)
            {
                DrawVertical(texture.TextureId, texture.TextureScale, effectId, texture.TopSourceRect, texture.InternalSourceRect, texture.BottomSourceRect, dest.Height, new Point(dest.X, dest.Y), color);
                return;
            }

            // calculate all dest rects
            var topLeftDest = new Rectangle(dest.X, dest.Y, (int)(topLeftSrc.Width * texture.TextureScale), (int)(topLeftSrc.Height * texture.TextureScale));
            var topRightDest = new Rectangle(dest.Right - (int)(topRightSrc.Width * texture.TextureScale), dest.Y, (int)(topRightSrc.Width * texture.TextureScale), (int)(topRightSrc.Height * texture.TextureScale));
            var bottomLeftDest = new Rectangle(dest.X, dest.Bottom - (int)(bottomLeftSrc.Height * texture.TextureScale), (int)(bottomLeftSrc.Width * texture.TextureScale), (int)(bottomLeftSrc.Height * texture.TextureScale));
            var bottomRightDest = new Rectangle(dest.Right - (int)(bottomRightSrc.Width * texture.TextureScale), dest.Bottom - (int)(bottomRightSrc.Height * texture.TextureScale), (int)(bottomRightSrc.Width * texture.TextureScale), (int)(bottomRightSrc.Height * texture.TextureScale));

            // render center parts
            {
                var srcRect = texture.InternalSourceRect;
                var destRect = new Rectangle(topLeftDest.Right, topLeftDest.Bottom, (int)(srcRect.Width * texture.TextureScale), (int)(srcRect.Height * texture.TextureScale));

                bool lastRow = false;
                while (true)
                {
                    // end of row?
                    var cutOff = destRect.Right - topRightDest.Left;
                    if (cutOff >= 0)
                    {
                        destRect.Width -= cutOff;
                        srcRect.Width -= (int)(cutOff / texture.TextureScale);
                    }

                    // check if we exceed bottom
                    var cutOffBottom = destRect.Bottom - bottomLeftDest.Top;
                    if (cutOffBottom >= 0)
                    {
                        destRect.Height -= cutOffBottom;
                        srcRect.Height -= (int)(cutOffBottom / texture.TextureScale);
                        lastRow = true;
                    }

                    // edge case - last row align perfectly
                    if (destRect.Height == 0) { break; }

                    // draw part
                    if (destRect.Width > 0 && destRect.Height > 0)
                    {
                        Renderer.DrawTexture(effectId, texture.TextureId, destRect, srcRect, color);
                    }
                    destRect.X += destRect.Width;
                
                    // go row down
                    if (cutOff >= 0)
                    {
                        // last row? break
                        if (lastRow) { break; }

                        // reset width and go row down
                        destRect.X = topLeftDest.Right;
                        srcRect.Width = texture.InternalSourceRect.Width;
                        destRect.Y += destRect.Height;
                    }
                }
            }

            // render frame
            // top frame
            {
                bool keepDrawing = true;
                var srcRect = texture.TopSourceRect;
                var destRect = new Rectangle(topLeftDest.Right, dest.Top, (int)(srcRect.Width * texture.TextureScale), (int)(srcRect.Height * texture.TextureScale));
                while (keepDrawing)
                {
                    var cutOff = destRect.Right - topRightDest.Left;
                    if (cutOff >= 0)
                    {
                        destRect.Width -= cutOff;
                        srcRect.Width -= (int)(cutOff / texture.TextureScale);
                        if (destRect.Width <= 0 || srcRect.Width <= 0) { break; }
                        keepDrawing = false;
                    }
                    Renderer.DrawTexture(effectId, texture.TextureId, destRect, srcRect, color);
                    destRect.X += destRect.Width;
                }
            }
            {
                // bottom frame
                {
                    bool keepDrawing = true;
                    var srcRect = texture.BottomSourceRect;
                    var destRect = new Rectangle(bottomLeftDest.Right, bottomLeftDest.Top, (int)(srcRect.Width * texture.TextureScale), (int)(srcRect.Height * texture.TextureScale));
                    while (keepDrawing)
                    {
                        var cutOff = destRect.Right - bottomRightDest.Left;
                        if (cutOff >= 0)
                        {
                            destRect.Width -= cutOff;
                            srcRect.Width -= (int)(cutOff / texture.TextureScale);
                            if (destRect.Width <= 0 || srcRect.Width <= 0) { break; }
                            keepDrawing = false;
                        }
                        Renderer.DrawTexture(effectId, texture.TextureId, destRect, srcRect, color);
                        destRect.X += destRect.Width;
                    }
                }
                // left frame
                {
                    bool keepDrawing = true;
                    var srcRect = texture.LeftSourceRect;
                    var destRect = new Rectangle(dest.Left, topLeftDest.Bottom, (int)(srcRect.Width * texture.TextureScale), (int)(srcRect.Height * texture.TextureScale));
                    while (keepDrawing)
                    {
                        var cutOff = destRect.Bottom - bottomLeftDest.Top;
                        if (cutOff >= 0)
                        {
                            destRect.Height -= cutOff;
                            srcRect.Height -= (int)(cutOff / texture.TextureScale);
                            if (destRect.Height <= 0 || srcRect.Height <= 0) { break; }
                            keepDrawing = false;
                        }
                        Renderer.DrawTexture(effectId, texture.TextureId, destRect, srcRect, color);
                        destRect.Y += destRect.Height;
                    }
                }
                // right frame
                {
                    bool keepDrawing = true;
                    var srcRect = texture.RightSourceRect;
                    var destRect = new Rectangle(topRightDest.Left, topRightDest.Bottom, (int)(srcRect.Width * texture.TextureScale), (int)(srcRect.Height * texture.TextureScale));
                    while (keepDrawing)
                    {
                        var cutOff = destRect.Bottom - bottomRightDest.Top;
                        if (cutOff >= 0)
                        {
                            destRect.Height -= cutOff;
                            srcRect.Height -= (int)(cutOff / texture.TextureScale);
                            if (destRect.Height <= 0 || srcRect.Height <= 0) { break; }
                            keepDrawing = false;
                        }
                        Renderer.DrawTexture(effectId, texture.TextureId, destRect, srcRect, color);
                        destRect.Y += destRect.Height;
                    }
                }
            }

            // render corners
            {
                Renderer.DrawTexture(effectId, texture.TextureId, topLeftDest, topLeftSrc, color);
                Renderer.DrawTexture(effectId, texture.TextureId, topRightDest, topRightSrc, color);
                Renderer.DrawTexture(effectId, texture.TextureId, bottomLeftDest, bottomLeftSrc, color);
                Renderer.DrawTexture(effectId, texture.TextureId, bottomRightDest, bottomRightSrc, color);
            }
        }

        /// <summary>
        /// Render stretched texture.
        /// </summary>
        public static void Draw(string? effectId, StretchedTexture texture, Rectangle dest, Color color)
        {
            if (texture.ExtraSize.HasValue)
            {
                dest.X -= texture.ExtraSize.Value.Left;
                dest.Width += texture.ExtraSize.Value.Left + texture.ExtraSize.Value.Right;
                dest.Y -= texture.ExtraSize.Value.Top;
                dest.Height += texture.ExtraSize.Value.Top + texture.ExtraSize.Value.Bottom;
            }

            // to avoid glitches
            if (dest.Width <= 0 || dest.Height <= 0) { return; }

            Renderer.DrawTexture(effectId, texture.TextureId, dest, texture.SourceRect, color);
        }

        /// <summary>
        /// Render icon texture.
        /// </summary>
        public static void Draw(string? effectId, IconTexture texture, Rectangle dest, Color color)
        {
            // to avoid glitches
            if (dest.Width <= 0 || dest.Height <= 0) { return; }

            if (texture.Texture != null)
            {
                Renderer.DrawTexture(effectId, texture.Texture, dest, texture.SourceRect, color);
            }
            else Renderer.DrawTexture(effectId, texture.TextureId, dest, texture.SourceRect, color);
        }
    }
}
