﻿using MonoGo.Engine.UI.Defs;
using MonoGo.Engine.UI.Utils;
using System;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A slider to get numeric input.
    /// </summary>
    public class Slider : Control
    {
        /// <summary>
        /// The entity used as slider handle.
        /// </summary>
        public Control Handle { get; private set; }

        /// <summary>
        /// Slider orientation.
        /// </summary>
        public Orientation Orientation { get; private set; }

        /// <summary>
        /// How much to change value via mouse wheel scroll.
        /// </summary>
        public int MouseWheelStep = 1;

        // current handle offset
        float _currHandleOffset;

        /// <inheritdoc/>
        internal override bool LockFocusWhileMouseDown => true;

        /// <inheritdoc/>
        internal override bool CanGetFocusWhileMouseIsDown => false;

        /// <inheritdoc/>
        internal override bool Interactable => true;

        /// <summary>
        /// Slider min value.
        /// </summary>
        public int MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    if (_maxValue < _minValue) { throw new ArgumentOutOfRangeException("Slider min value must be smaller than max value!"); }
                    if (_value < _minValue)
                    {
                        _value = _minValue;
                    }
                }
            }
        }
        int _minValue = 0;

        /// <summary>
        /// Slider max value.
        /// </summary>
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    if (_maxValue < _minValue) { throw new ArgumentOutOfRangeException("Slider max value must be bigger than min value!"); }
                    if (_value > _maxValue)
                    {
                        _value = _maxValue;
                    }
                }
            }
        } 
        int _maxValue = 10;

        /// <summary>
        /// Set / get current value.
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    if (value < MinValue) { throw new ArgumentOutOfRangeException("Slider value can't be smaller than min value!"); }
                    if (value > MaxValue) { throw new ArgumentOutOfRangeException("Slider value can't be bigger than max value!"); }
                    _value = value;
                    Events.OnValueChanged?.Invoke(this);
                    UISystem.Events.OnValueChanged?.Invoke(this);
                }
            }
        }
        int _value = 5;

        /// <summary>
        /// Set current value, after clamping it to be between min and max.
        /// </summary>
        public int ValueSafe
        {
            get => Value;
            set
            {
                Value = Math.Clamp(value, MinValue, MaxValue);
            }
        }

        /// <summary>
        /// Slider steps count (or 0 for MaxValue - MinValue).
        /// </summary>
        public uint StepsCount
        {
            get => _stepsCount;
            set
            {
                if (_stepsCount != value)
                {
                    _stepsCount = value;
                    if (value > ValueRange)
                    {
                        throw new ArgumentOutOfRangeException("Slider steps count can't be bigger than ValueRange (max - min) value!");
                    }
                    Value = MinValue;
                }
            }
        }
        uint _stepsCount = 0;

        /// <summary>
        /// Get value, as percent between 0.f and 1.f.
        /// </summary>
        public float ValuePercent => (float)(Value - MinValue) / (float)(MaxValue - MinValue);

        /// <summary>
        /// If true, slider direction will be flipped.
        /// </summary>
        public bool FlippedDirection = false;

        // so we can point and drag handle
        /// <inheritdoc/>
        protected override bool WalkInternalChildren => true;

        /// <summary>
        /// Get the value range based on max and min values.
        /// </summary>
        public int ValueRange => MaxValue - MinValue;

        /// <summary>
        /// Create the slider.
        /// </summary>
        /// <param name="stylesheet">Slider stylesheet.</param>
        /// <param name="orientation">Slider orientation.</param>
        public Slider(StyleSheet? stylesheet, StyleSheet? handleStylesheet, Orientation orientation = Orientation.Horizontal) : base(stylesheet)
        {
            // set orientation and call default anchor and size again
            Orientation = orientation;
            CalculateDefaultAnchorAndSize();

            // create handle
            Handle = new Control(handleStylesheet);
            Handle.CopyStateFrom = this;
            Handle.Anchor = (orientation == Orientation.Horizontal) ? Anchor.CenterLeft : Anchor.TopCenter;
            Handle.TransferInteractionsTo = this;
            AddChildInternal(Handle);
            Handle.IgnoreScrollOffset = true;
        }

        /// <summary>
        /// Create the slider with default stylesheets.
        /// </summary>
        /// <param name="orientation">Slider orientation.</param>
        public Slider(Orientation orientation = Orientation.Horizontal) : 
            this(
                (orientation == Orientation.Horizontal) ? UISystem.DefaultStylesheets.HorizontalSliders : UISystem.DefaultStylesheets.VerticalSliders,
                (orientation == Orientation.Horizontal) ? UISystem.DefaultStylesheets.HorizontalSlidersHandle : UISystem.DefaultStylesheets.VerticalSlidersHandle,
                orientation)
        {
        }

        /// <inheritdoc/>
        internal override void DoInteractions(InputState inputState)
        {
            // do base interactions
            base.DoInteractions(inputState);

            // special case - max value is 0
            if (ValueRange <= 0) 
            { 
                return; 
            }

            // select value via mouse
            if (inputState.LeftMouseDown)
            {
                // get default steps count and step size
                var stepsCount = (int)StepsCount;
                var valueRange = ValueRange;
                if (stepsCount == 0) { stepsCount = valueRange; }
                var stepSize = valueRange / stepsCount;
                
                // get value in percent
                float valuePercent = (Orientation == Orientation.Horizontal) ?
                    (((float)inputState.MousePosition.X - LastInternalBoundingRect.Left) / (float)LastInternalBoundingRect.Width) :
                    (((float)inputState.MousePosition.Y - LastInternalBoundingRect.Top) / (float)LastInternalBoundingRect.Height);
                if (float.IsNaN(valuePercent)) { valuePercent = 0f; }
                if (valuePercent < 0f) { valuePercent = 0f; }
                if (valuePercent > 1f) {  valuePercent = 1f; }
                if (((Orientation == Orientation.Horizontal) && FlippedDirection) || 
                    ((Orientation == Orientation.Vertical) && !FlippedDirection)) 
                { 
                    valuePercent = 1f - valuePercent; 
                }
                int relativeValue = (int)(MathF.Round((valuePercent * valueRange) / stepSize) * stepSize);
                Value = MinValue + (int)relativeValue;
            }
        }

        public void ScrollToEnd()
        {
            Value = MaxValue;
        }

        /// <inheritdoc/>
        internal override void PerformMouseWheelScroll(int val)
        {
            if (MouseWheelStep != 0)
            {
                if (val > 0)
                {
                    Value = Math.Clamp(Value + MouseWheelStep, MinValue, MaxValue);
                }
                if (val < 0)
                {
                    Value = Math.Clamp(Value - MouseWheelStep, MinValue, MaxValue);
                }
            }
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);
            UpdateHandle(dt);
        }

        /// <summary>
        /// Update slider handle offset.
        /// </summary>
        protected virtual void UpdateHandle(float dt)
        {
            // not visible? skip
            if (!IsCurrentlyVisible()) { return; }

            // special case - no value range
            if (ValueRange <= 0)
            {
                Handle.Visible = false;
                return;
            }
            Handle.Visible = true;

            // get relative value
            int relativeValue = Value - MinValue;

            // set horizontal handle position
            if (Orientation == Orientation.Horizontal)
            {
                var offsetX = FlippedDirection ?
                    (int)((1f - (float)relativeValue / ValueRange) * LastInternalBoundingRect.Width) :
                    (int)(((float)relativeValue / ValueRange) * LastInternalBoundingRect.Width);
                float newOffset = offsetX - Handle.LastBoundingRect.Width / 2;
                _currHandleOffset = InterpolateHandlePosition ?
                    MathUtils.Lerp(_currHandleOffset, newOffset, dt * HandleInterpolationSpeed) : newOffset;
                Handle.Offset.X.SetPixels((int)_currHandleOffset);
            }
            // set vertical handle position
            else
            {
                var offsetY = FlippedDirection ?
                    (int)(((float)relativeValue / ValueRange) * LastInternalBoundingRect.Height) :
                    (int)((1f - (float)relativeValue / ValueRange) * LastInternalBoundingRect.Height);
                float newOffset = offsetY - Handle.LastBoundingRect.Height / 2;
                _currHandleOffset = InterpolateHandlePosition ?
                    MathUtils.Lerp(_currHandleOffset, newOffset, dt * HandleInterpolationSpeed) : newOffset;
                Handle.Offset.Y.SetPixels((int)_currHandleOffset);
            }
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultControlTypeSize()
        {
            var ret = new MeasureVector();
            if (Orientation == Orientation.Horizontal)
            {
                ret.X.SetPercents(100f);
                ret.Y.SetPixels(16);
            }
            else
            {
                ret.Y.SetPercents(100f);
                ret.X.SetPixels(16);
            }
            return ret;
        }
    }
}
