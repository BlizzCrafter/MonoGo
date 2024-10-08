﻿using MonoGo.Engine.UI.Defs;
using System;
using System.Globalization;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Text input that accept only numbers, and have a - and + buttons.
    /// </summary>
    public class NumericInput : TextInput
    {
        /// <inheritdoc/>
        public override bool Multiline
        {
            get => false;
            set => throw new InvalidOperationException("Numeric input fields can't be multiline!");
        }

        /// <inheritdoc/>
        public override string? PlaceholderText
        {
            get => DefaultValue.ToString();
            set => throw new InvalidOperationException("Numeric input fields can't have placeholder text!");
        }

        /// <summary>
        /// How much to increase / decrease values when clicking on the plus / minus buttons.
        /// </summary>
        public decimal ButtonsStepSize = 1;

        /// <inheritdoc/>
        public override string Value
        {
            get => base.Value;
            set
            {
                // remove decimal if not accepted
                if (!AcceptsDecimal)
                {
                    value = value.Split(DecimalSeparator)[0];
                }

                // trim
                value = value.Trim();

                // if empty or a single decimal separator, stop here and set to empty
                if ((value.Length == 0) || (value.Length == 1 && value[0] == DecimalSeparator))
                {
                    base.Value = string.Empty;
                    _valueFloat = null;
                    return;
                }

                // special case: if the only input is - it might be the begining of a negative number, so we allow it
                if ((MinValue == null || MinValue.Value < 0) && (value == "-"))
                {
                    base.Value = value;
                    _valueFloat = null;
                    return;
                }

                // set float value and base value
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo, out decimal result))
                {
                    // check min value
                    if (result < MinValue)
                    {
                        result = MinValue.Value;
                        value = result.ToString();
                    }

                    // check max value
                    if (result > MaxValue)
                    {
                        result = MaxValue.Value;
                        value = result.ToString();
                    }

                    // special - if value is 0, make sure input is not 00000...)
                    if (result == 0)
                    {
                        value = "0";
                    }
                    // if not 0, trim zeroes from the start
                    else if (value.StartsWith('0'))
                    {
                        value = value.TrimStart('0');
                    }

                    // set value
                    _valueFloat = result;
                    base.Value = value;
                }

                // failed to parse? don't change value!
            }
        }

        /// <inheritdoc/>
        internal override void PostUpdate(InputState inputState)
        {
            base.PostUpdate(inputState);

            if (inputState.LeftMouseDown)
            {
                if (_lockSelf && (_minusButton?.IsPointedOn(inputState.MousePosition) ?? false))
                {
                    _lockSelf = false;
                }

                if (_lockSelf && (_plusButton?.IsPointedOn(inputState.MousePosition) ?? false))
                {
                    _lockSelf = false;
                }
            }
        }

        /// <summary>
        /// Optional min value.
        /// </summary>
        public decimal? MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    if (_maxValue <= _minValue) { throw new ArgumentOutOfRangeException("Numeric Input min value must be smaller than max value!"); }
                }
            }
        }
        decimal? _minValue;

        /// <summary>
        /// Optional max value.
        /// </summary>
        public decimal? MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    if (_maxValue <= _minValue) { throw new ArgumentOutOfRangeException("Numeric Input max value must be bigger than min value!"); }
                }
            }
        }
        decimal? _maxValue;

        /// <summary>
        /// Get / set value as a float number.
        /// </summary>
        public decimal NumericValue
        {
            get => _valueFloat ?? DefaultValue;
            set
            {
                if (!AcceptsDecimal && (Math.Floor(value) != value))
                {
                    throw new InvalidOperationException("Can't set NumericInput float value while not accepting decimal point!");
                }

                if (value < MinValue) { throw new ArgumentOutOfRangeException("Numeric Input value can't be smaller than min value!"); }
                if (value > MaxValue) { throw new ArgumentOutOfRangeException("Numeric Input value can't be bigger than max value!"); }

                Value = value.ToString(CultureInfo);
            }
        }

        // current value as float
        decimal? _valueFloat = null;

        /// <summary>
        /// Default value to return when there's no numeric input or when an invalid value is provided.
        /// </summary>
        public decimal DefaultValue = 0;

        /// <summary>
        /// Culture info to use when parsing floats.
        /// This will affect the decimal point character.
        /// </summary>
        public CultureInfo CultureInfo = CultureInfo.InvariantCulture;

        /// <summary>
        /// Return the character used as decimal point.
        /// </summary>
        public char DecimalSeparator => CultureInfo.NumberFormat.NumberDecimalSeparator[0];

        /// <summary>
        /// If true, this numeric input accepts a decimal point.
        /// If false it will not support it.
        /// </summary>
        public bool AcceptsDecimal = true;

        // minus / plus buttons
        Button? _minusButton;
        Button? _plusButton;

        /// <summary>
        /// Set / get the minus button text.
        /// </summary>
        public string? MinusButtonText
        {
            get => _minusButton?.Paragraph?.Text ?? null;
            set { if (_minusButton != null) { _minusButton.Paragraph.Text = value ?? string.Empty; } }
        }

        /// <summary>
        /// Set / get the plus button text.
        /// </summary>
        public string? PlusButtonText
        {
            get => _plusButton?.Paragraph?.Text ?? null;
            set { if (_plusButton != null) { _plusButton.Paragraph.Text = value ?? string.Empty; } }
        }

        /// <summary>
        /// Create the numeric text input.
        /// </summary>
        /// <param name="stylesheet">Numeric input stylesheet.</param>
        /// <param name="addPlusButton">If true, will add a plus button to increase value.</param>
        /// <param name="addMinusButton">If true, will add a minus button to decrease value.</param>
        public NumericInput(StyleSheet? stylesheet, bool addPlusButton = true, bool addMinusButton = true) : base(stylesheet)
        {
            var buttonStyle = UISystem.DefaultStylesheets.NumericTextInputButton ?? UISystem.DefaultStylesheets.Buttons;

            // add + button
            if (addPlusButton)
            {
                var plusButton = new Button(buttonStyle, "+");
                plusButton.Anchor = Anchor.CenterRight;
                plusButton.Offset.X.SetPixels(-GetPadding().Right + plusButton.GetMarginAfter().X);
                plusButton.Events.OnClick = (Control Control) =>
                {
                    var value = NumericValue + ButtonsStepSize;
                    if (value < MinValue) { value = MinValue.Value; }
                    if (value > MaxValue) { value = MaxValue.Value; }
                    NumericValue = value;
                };
                AddChildInternal(plusButton);
                _plusButton = plusButton;
            }

            // add - button
            if (addMinusButton)
            {
                var minusButton = new Button(buttonStyle, "-");
                minusButton.Anchor = Anchor.CenterLeft;
                minusButton.Offset.X.SetPixels(-GetPadding().Left + minusButton.GetMarginBefore().X);
                minusButton.Events.OnClick = (Control Control) =>
                {
                    var value = NumericValue - ButtonsStepSize;
                    if (value < MinValue) { value = MinValue.Value; }
                    if (value > MaxValue) { value = MaxValue.Value; }
                    NumericValue = value;
                };
                AddChildInternal(minusButton);
                _minusButton = minusButton;
            }
        }

        /// <summary>
        /// Create the numeric text input with default stylesheets.
        /// </summary>
        /// <param name="addPlusButton">If true, will add a plus button to increase value.</param>
        /// <param name="addMinusButton">If true, will add a minus button to decrease value.</param>
        public NumericInput(bool addPlusButton = true, bool addMinusButton = true)
            : this(UISystem.DefaultStylesheets.NumericTextInput ?? UISystem.DefaultStylesheets.TextInput, addPlusButton, addMinusButton)
        {
        }

        /// <inheritdoc/>
        protected override int GetInputMaxWidth()
        {
            return base.GetInputMaxWidth() - (_plusButton != null ? _plusButton.LastBoundingRect.Width : 0);
        }
    }
}
