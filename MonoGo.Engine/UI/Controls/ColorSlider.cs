using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;
using System;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Color slider is a color picker Control that is based on slider, where the user can pick a color from a range.
    /// This picker is useful for when the user can pick hue without choosing brightness or saturation.
    /// </summary>
    public class ColorSlider : Slider, IColorPicker
    {
        /// <summary>
        /// Get / set slider color value, based on the default state source texture.
        /// </summary>
        public Color ColorValue
        {
            get
            {
                var srcRect = SourceRectangle;
                if (Orientation == Orientation.Horizontal)
                {
                    return Renderer.GetPixelFromTexture(SourceTextureId, new Point(srcRect.Left + (int)Math.Round(ValuePercent * (srcRect.Width - 0.1f)), srcRect.Top + srcRect.Height / 2));
                }
                else
                {
                    return Renderer.GetPixelFromTexture(SourceTextureId, new Point(srcRect.Left + srcRect.Width / 2, srcRect.Top + (int)Math.Round(ValuePercent * (srcRect.Height - 0.1f))));
                }
            }
            set
            {
                var src = SourceRectangle;
                src.Y += src.Height / 2;
                src.Height = 1;
                var offset = Renderer.FindPixelOffsetInTexture(SourceTextureId, SourceRectangle, value, false);
                if (offset.HasValue)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        ValueSafe = offset.Value.X;
                    }
                    else
                    {
                        ValueSafe = offset.Value.Y;
                    }
                }
            }
        }

        /// <summary>
        /// Create the colors slider.
        /// </summary>
        /// <param name="stylesheet">Slider stylesheet.</param>
        /// <param name="stylesheet">Slider handle stylesheet.</param>
        /// <param name="orientation">Slider orientation.</param>
        public ColorSlider(StyleSheet? stylesheet, StyleSheet? handleStylesheet, Orientation orientation = Orientation.Horizontal) : base(stylesheet, handleStylesheet, orientation)
        {
            AutoSetRange = true;
            SetAutoRange();
        }

        /// <summary>
        /// Create the color slider with default stylesheets.
        /// </summary>
        /// <param name="system">Parent UI system.</param>
        /// <param name="orientation">Slider orientation.</param>
        public ColorSlider(Orientation orientation = Orientation.Horizontal) :
            this(
                (orientation == Orientation.Horizontal) ? (UISystem.DefaultStylesheets.HorizontalColorSliders ?? UISystem.DefaultStylesheets.HorizontalSliders) : (UISystem.DefaultStylesheets.VerticalColorSliders ?? UISystem.DefaultStylesheets.VerticalSliders),
                (orientation == Orientation.Horizontal) ? (UISystem.DefaultStylesheets.HorizontalColorSlidersHandle ?? UISystem.DefaultStylesheets.HorizontalSlidersHandle) : (UISystem.DefaultStylesheets.VerticalColorSlidersHandle ?? UISystem.DefaultStylesheets.VerticalSlidersHandle),
                orientation)
        {
        }

        /// <inheritdoc/>
        public void SetColorValueApproximate(Color value)
        {
            var src = SourceRectangle;
            src.Y += src.Height / 2;
            src.Height = 1;
            var offset = Renderer.FindPixelOffsetInTexture(SourceTextureId, SourceRectangle, value, true);
            if (offset.HasValue)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    ValueSafe = offset.Value.X;
                }
                else
                {
                    ValueSafe = offset.Value.Y;
                }
            }
        }

        /// <summary>
        /// Get source stretch texture.
        /// </summary>
        StretchedTexture? SourceTextureData => StyleSheet.GetProperty<StretchedTexture>("FillTextureStretched", State, null, OverrideStyles);

        /// <summary>
        /// Get slider source rectangle.
        /// </summary>
        Rectangle SourceRectangle => SourceTextureData?.SourceRect ?? Rectangle.Empty;

        /// <summary>
        /// Get slider source texture.
        /// </summary>
        string SourceTextureId => SourceTextureData?.TextureId ?? string.Empty;

        /// <inheritdoc/>
        protected override void SetAutoRange()
        {
            MinValue = 0;
            if (Orientation == Orientation.Horizontal)
            {
                MaxValue = Math.Max(SourceTextureData?.SourceRect.Width ?? LastBoundingRect.Width, 10);
            }
            else
            {
                MaxValue = Math.Max(SourceTextureData?.SourceRect.Height ?? LastBoundingRect.Height, 10);
            }
            StepsCount = (uint)MaxValue;
        }
    }
}
