using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;
using System;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Color picker lets the user pick a color from a 2d source texture, with X and Y axes.
    /// This is useful for color pickers that have a combination of hue and brightness.
    /// </summary>
    public class ColorPicker : Control, IColorPicker
    {
        /// <summary>
        /// The Control used as the color picker handle.
        /// </summary>
        public Control Handle { get; private set; }

        // last handle offset
        Point _lastHandleOffset = new Point(-1, -1);

        /// <summary>
        /// Get / set color picker value, as a color extracted from the source texture.
        /// </summary>
        public Color ColorValue
        {
            get => _colorValue;
            set
            {
                _colorValue = value;
                var srcTexture = SourceTextureData;
                var offset = Renderer.FindPixelOffsetInTexture(srcTexture!.TextureId, srcTexture.SourceRect, value, false);
                if (offset.HasValue)
                {
                    SetHandleOffsetFromSource(offset.Value);
                }
            }
        }

        /// <inheritdoc/>
        public void SetColorValueApproximate(Color value)
        {
            _colorValue = value;
            var srcTexture = SourceTextureData;
            var offset = Renderer.FindPixelOffsetInTexture(srcTexture!.TextureId, srcTexture.SourceRect, value, true);
            if (offset.HasValue)
            {
                SetHandleOffsetFromSource(offset.Value);
            }
        }

        // current color value
        Color _colorValue;

        // offset in source texture, from source rectangle top-left corner
        Point _offsetInSource;

        /// <summary>
        /// Get source stretch texture.
        /// </summary>
        StretchedTexture? SourceTextureData => StyleSheet.GetProperty<StretchedTexture>("FillTextureStretched", State, null, OverrideStyles);

        /// <summary>
        /// Create the color picker.
        /// </summary>
        /// <param name="stylesheet">Color picker stylesheet.</param>
        /// <param name="handleStylesheet">Color picker handle stylesheet.</param>
        public ColorPicker(StyleSheet? stylesheet, StyleSheet? handleStylesheet) : base(stylesheet)
        {
            // create handle
            Handle = new Control(handleStylesheet);
            Handle.CopyStateFrom = this;
            Handle.Anchor = Anchor.TopLeft;
            Handle.TransferInteractionsTo = this;
            AddChildInternal(Handle);
            Handle.IgnoreScrollOffset = true;
        }

        /// <summary>
        /// Create the color picker with default stylesheets.
        /// </summary>
        public ColorPicker() :
            this(
                UISystem.DefaultStylesheets.ColorPickers,
                UISystem.DefaultStylesheets.ColorPickersHandle)
        {
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);

            if ((_lastHandleOffset.X != _offsetInSource.X) || (_lastHandleOffset.Y != _offsetInSource.Y))
            {
                _lastHandleOffset = _offsetInSource;
                UpdateValueFromHandle();
                Events.OnValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Get factor from destination size to source size.
        /// </summary>
        Vector2 GetDestToSourceFactor()
        {
            var srcTexture = SourceTextureData;
            var srcWidth = (srcTexture?.SourceRect.Width ?? 1f);
            var srcHeight = (srcTexture?.SourceRect.Height ?? 1f);
            float factorX = srcWidth / (float)Math.Max(1, LastBoundingRect.Width);
            float factorY = srcHeight / (float)Math.Max(1, LastBoundingRect.Height);
            return new Vector2(factorX, factorY);
        }

        /// <summary>
        /// Set the color picker handle offset.
        /// </summary>
        /// <param name="offset">Color picker offset, in pixels, from top-left corner.</param>
        public void SetHandleOffset(Point offset)
        {
            var factor = GetDestToSourceFactor();
            _offsetInSource.X = (int)Math.Ceiling((float)offset.X * factor.X);
            _offsetInSource.Y = (int)Math.Ceiling((float)offset.Y * factor.Y);
        }

        /// <summary>
        /// Set the color picker handle offset, with offset representing pixel offset in texture from the picker source rectangle top left corner.
        /// </summary>
        /// <param name="offset">Color picker offset, in pixels, from top-left corner of the texture source rectangle.</param>
        public void SetHandleOffsetFromSource(Point offset)
        {
            _offsetInSource = offset;
        }

        /// <summary>
        /// Update color value from handle offset.
        /// </summary>
        void UpdateValueFromHandle()
        {
            var srcTexture = SourceTextureData;
            _colorValue = Renderer.GetPixelFromTexture(srcTexture!.TextureId ?? string.Empty, new Point(srcTexture.SourceRect.X + _offsetInSource.X, srcTexture.SourceRect.Y + _offsetInSource.Y));
        }

        /// <inheritdoc/>
        internal override void DoInteractions(InputState inputState)
        {
            // do base interactions
            base.DoInteractions(inputState);

            // select value via mouse
            if (inputState.LeftMouseDown)
            {
                SetHandleOffset(new Point(inputState.MousePosition.X - LastBoundingRect.X, inputState.MousePosition.Y - LastBoundingRect.Y));
            }
        }

        /// <inheritdoc/>
        protected override DrawMethodResult Draw(DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            // update handle offset
            var factor = GetDestToSourceFactor();
            Handle.Offset.X.Value = MathF.Ceiling(_offsetInSource.X / factor.X - Handle.LastBoundingRect.Width / 2);
            Handle.Offset.Y.Value = MathF.Ceiling(_offsetInSource.Y / factor.Y - Handle.LastBoundingRect.Height / 2);

            return base.Draw(parentDrawResult, siblingDrawResult);
        }
    }
}
