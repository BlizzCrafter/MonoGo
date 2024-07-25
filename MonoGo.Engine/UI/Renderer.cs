using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace MonoGo.Engine.UI
{
    internal static class Renderer
    {
        private static GraphicsDevice _device;
        private static ContentManager _content;
        private static SpriteBatch _spriteBatch;
        private static string _assetsRoot;
        private static Texture2D _whiteTexture;

        private static Dictionary<string, SpriteFont> _fonts = new();
        private static Dictionary<string, Texture2D> _textures = new();
        private static Dictionary<string, Sprite> _sprites = new();

        public static float GlobalTextScale = 1f;

        /// <summary>
        /// Create the monogame renderer.
        /// </summary>
        /// <param name="assetsPath">Root directory to load assets from. Check out the demo project for details.</param>
        public static void Init(string assetsPath)
        {
            _device = GraphicsMgr.Device;
            _spriteBatch = new SpriteBatch(_device);
            _assetsRoot = assetsPath;
            _content = new ContentManager(GameMgr.Game.Services, _assetsRoot);

            // create white texture
            _whiteTexture = new Texture2D(_device, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Load / get font.
        /// </summary>
        public static SpriteFont GetFont(string? fontName)
        {
            var fontNameOrDefault = !string.IsNullOrEmpty(fontName) ? fontName : "Regular";
            if (_fonts.TryGetValue(fontNameOrDefault, out var font)) 
            { 
                return font; 
            }

            var ret = ResourceHub.GetResource<IFont>("Fonts", fontNameOrDefault).SpriteFont;
            _fonts[fontNameOrDefault] = ret;
            return ret;
        }

        /// <summary>
        /// Load / get texture.
        /// </summary>
        public static Texture2D GetTexture(string textureId)
        {
            if (_textures.TryGetValue(textureId, out var texture))
            {
                return texture;
            }

            var ret = _content.Load<Texture2D>(Path.ChangeExtension(textureId, null));
            _textures[textureId] = ret;
            return ret;
        }

        public static Sprite GetSprite(string textureId)
        {
            if (_sprites.TryGetValue(textureId, out var sprite))
            {
                return sprite;
            }

            var texture = _content.Load<Texture2D>(Path.ChangeExtension(textureId, null));
            var ret = new Sprite(new Frame(texture, RectangleF.Empty, Vector2.Zero), Vector2.Zero, textureId);
            _sprites[textureId] = ret;
            return ret;
        }

        /// <summary>
        /// Load / get effect from id.
        /// </summary>
        public static Effect? GetEffect(string? effectId)
        {
            if (effectId == null) { return null; }
            var firstCharToUpper = string.Concat(effectId[..1].ToUpper(), effectId.AsSpan(1));
            return ResourceHub.GetResource<Effect>("Effects", firstCharToUpper);
        }

        /// <summary>
        /// Set active effect id.
        /// </summary>
        public static void SetEffect(string? effectId)
        {
            if (_currEffectId != effectId)
            {
                _spriteBatch.End();
                _currEffectId = effectId;
                BeginBatch();
            }
        }
        private static string? _currEffectId;

        /// <summary>
        /// Convert iguina color to mg color.
        /// </summary>
        private static Color ToMgColor(Color color)
        {
            var colorMg = new Color(color.R, color.G, color.B, color.A);
            if (color.A < 255)
            {
                float factor = (float)color.A / 255f;
                colorMg.R = (byte)((float)color.R * factor);
                colorMg.G = (byte)((float)color.G * factor);
                colorMg.B = (byte)((float)color.B * factor);
            }
            return colorMg;
        }

        /// <summary>
        /// Called at the beginning of every frame.
        /// </summary>
        public static void StartFrame()
        {
            _currEffectId = null;
            _currScissorRegion = null;
            BeginBatch();
        }

        /// <summary>
        /// Called at the end of every frame.
        /// </summary>
        public static void EndFrame()
        {
            _spriteBatch.End();
        }

        public static void StartCursor()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        public static void EndCursor()
        {
            _spriteBatch.End();
        }

        /// <inheritdoc/>
        public static Rectangle GetScreenBounds()
        {
            var size = GameMgr.WindowManager.CanvasSize;
            return new Rectangle(0, 0, (int)size.X, (int)size.Y);
        }

        /// <inheritdoc/>
        public static void DrawTexture(string? effectIdentifier, Texture2D texture, Rectangle destRect, Rectangle sourceRect, Color color)
        {
            SetEffect(effectIdentifier);
            var colorMg = ToMgColor(color);
            _spriteBatch.Draw(texture,
                new Rectangle(destRect.X, destRect.Y, destRect.Width, destRect.Height),
                new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height),
                colorMg);
        }

        /// <inheritdoc/>
        public static void DrawTexture(string? effectIdentifier, string textureId, Rectangle destRect, Rectangle sourceRect, Color color)
        {
            SetEffect(effectIdentifier);
            var texture = GetTexture(textureId);
            var colorMg = ToMgColor(color);
            _spriteBatch.Draw(texture,
                new Rectangle(destRect.X, destRect.Y, destRect.Width, destRect.Height),
                new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height),
                colorMg);
        }

        /// <inheritdoc/>
        public static void DrawSprite(string textureId, Rectangle destRect, Rectangle sourceRect, Color color)
        {
            var sprite = GetSprite(textureId);
            var colorMg = ToMgColor(color);

            sprite.Draw(
                destRect.ToRectangleF(), 
                0,
                sourceRect.ToRectangleF(),
                Angle.Right, 
                colorMg);
        }

        /// <inheritdoc/>
        public static Point MeasureText(string text, string? fontId, int fontSize, float spacing)
        {
            var spriteFont = GetFont(fontId);
            float scale = (fontSize / 24f) * GlobalTextScale; // 24 is the default font sprite size. you need to adjust this to your own sprite font.
            spriteFont.Spacing = spacing - 1f;
            return MeasureStringNew(spriteFont, text, scale);
        }

        /// <inheritdoc/>
        public static int GetTextLineHeight(string? fontId, int fontSize)
        {
            return (int)MeasureText("WI", fontId, fontSize, 1f).Y;
        }

        /// <inheritdoc/>

        [Obsolete("Note: currently we render outline in a primitive way. To improve performance and remove some visual artifact during transitions, its best to implement a shader that draw text with outline properly.")]
        public static void DrawText(string? effectIdentifier, string text, string? fontId, int fontSize, Point position, Color fillColor, Color outlineColor, int outlineWidth, float spacing)
        {
            SetEffect(effectIdentifier);

            var spriteFont = GetFont(fontId);
            spriteFont.Spacing = spacing - 1f;
            float scale = (fontSize / 24f) * GlobalTextScale; // 24 is the default font sprite size. you need to adjust this to your own sprite font.

            // draw outline
            if ((outlineColor.A > 0) && (outlineWidth > 0))
            {
                // because we draw outline in a primitive way, we want it to fade a lot faster than fill color
                if (outlineColor.A < 255)
                {
                    float alphaFactor = (float)(outlineColor.A / 255f);
                    outlineColor.A = (byte)((float)fillColor.A * Math.Pow(alphaFactor, 7));
                }

                // draw outline
                var outline = ToMgColor(outlineColor);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X - outlineWidth, position.Y), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X, position.Y - outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + outlineWidth, position.Y), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X, position.Y + outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X - outlineWidth, position.Y - outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X - outlineWidth, position.Y + outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + outlineWidth, position.Y - outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), outline, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            }

            // draw fill
            {
                var colorMg = ToMgColor(fillColor);
                _spriteBatch.DrawString(spriteFont, text, new Vector2(position.X, position.Y), colorMg, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            }
        }

        /// <inheritdoc/>
        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            SetEffect(null);

            var texture = _whiteTexture;
            var colorMg = ToMgColor(color);
            _spriteBatch.Draw(texture,
                new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height),
                null,
                colorMg);
        }

        /// <inheritdoc/>
        public static void SetScissorRegion(Rectangle region)
        {
            _currScissorRegion = region;
            _currEffectId = null;
            _spriteBatch.End();
            BeginBatch();
        }

        /// <inheritdoc/>
        public static Rectangle? GetScissorRegion()
        {
            return _currScissorRegion;
        }

        // current scissor region
        private static Rectangle? _currScissorRegion = null;

        /// <summary>
        /// Begin a new rendering batch.
        /// </summary>
        private static void BeginBatch()
        {
            var effect = GetEffect(_currEffectId);
            if (_currScissorRegion != null)
            {
                _device.ScissorRectangle = new Rectangle(_currScissorRegion.Value.X, _currScissorRegion.Value.Y, _currScissorRegion.Value.Width, _currScissorRegion.Value.Height);
            }
            var raster = new RasterizerState
            {
                CullMode = _device.RasterizerState.CullMode,
                DepthBias = _device.RasterizerState.DepthBias,
                FillMode = _device.RasterizerState.FillMode,
                MultiSampleAntiAlias = _device.RasterizerState.MultiSampleAntiAlias,
                SlopeScaleDepthBias = _device.RasterizerState.SlopeScaleDepthBias,
                ScissorTestEnable = _currScissorRegion.HasValue
            };
            _device.RasterizerState = raster;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: effect, rasterizerState: raster);
        }

        /// <inheritdoc/>
        public static void ClearScissorRegion()
        {
            _currScissorRegion = null;
            _currEffectId = null;
            _spriteBatch.End();
            BeginBatch();
        }

        /// <summary>
        /// MonoGame measure string sucks and return wrong result.
        /// So I copied the code that render string and changed it to measure instead.
        /// </summary>
        public static Point MeasureStringNew(SpriteFont spriteFont, string text, float scale)
        {
            var matrix = Matrix.Identity;
            {
                matrix.M11 = scale;
                matrix.M22 = scale;
                matrix.M41 = 0;
                matrix.M42 = 0;
            }

            bool flag3 = true;
            var zero2 = Vector2.Zero;
            Point ret = new Point();
            {
                foreach (char c in text)
                {
                    switch (c)
                    {
                        case '\n':
                            zero2.X = 0f;
                            zero2.Y += spriteFont.LineSpacing;
                            flag3 = true;
                            continue;
                        case '\r':
                            continue;
                    }

                    var glyph = spriteFont.GetGlyphs()[c];
                    if (flag3)
                    {
                        zero2.X = Math.Max(glyph.LeftSideBearing, 0f);
                        flag3 = false;
                    }
                    else
                    {
                        zero2.X += spriteFont.Spacing + glyph.LeftSideBearing;
                    }

                    Vector2 position2 = zero2;

                    position2.X += glyph.Cropping.X;
                    position2.Y += glyph.Cropping.Y;
                    Vector2.Transform(ref position2, ref matrix, out position2);
                    ret.X = (int)Math.Max((float)(position2.X + (float)glyph.BoundsInTexture.Width * scale), (float)(ret.X));
                    ret.Y = (int)Math.Max((float)(position2.Y + (float)spriteFont.LineSpacing * scale), (float)(ret.Y));

                    zero2.X += glyph.Width + glyph.RightSideBearing;
                }
            }

            //ret.Y += spriteFont.LineSpacing / 2;
            return ret;
        }
    }
}
