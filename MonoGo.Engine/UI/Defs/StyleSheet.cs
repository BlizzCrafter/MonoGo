﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// Define the graphics style of a GUI entity and a state.
    /// </summary>
    public class StyleSheetState
    {
        /// <summary>
        /// Fill texture, tiled and with frame over the target region.
        /// </summary>
        public FramedTexture? FillTextureFramed { get; set; }

        /// <summary>
        /// Fill texture, stretched over the target region.
        /// </summary>
        public StretchedTexture? FillTextureStretched { get; set; }

        /// <summary>
        /// Fill texture, with size based on its source rectangle.
        /// </summary>
        public IconTexture? Icon { get; set; }

        /// <summary>
        /// Fill color tint.
        /// </summary>
        public Color? FillColor { get; set; }

        /// <summary>
        /// Text alignment for controls with text.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TextAlignment? TextAlignment { get; set; }

        /// <summary>
        /// Font identifier, or null to use default font.
        /// </summary>
        public string? FontIdentifier { get; set; }

        /// <summary>
        /// Text fill color.
        /// </summary>
        public Color? TextFillColor { get; set; }

        /// <summary>
        /// Text fill color to use when entity has no value.
        /// </summary>
        public Color? NoValueTextFillColor { get; set; }

        /// <summary>
        /// Text outline color.
        /// </summary>
        public Color? TextOutlineColor { get; set; }

        /// <summary>
        /// Text outline width.
        /// </summary>
        public int? TextOutlineWidth { get; set; }

        /// <summary>
        /// Text spacing factor.
        /// 1f = Default spacing.
        /// </summary>
        public float? TextSpacing { get; set; }

        /// <summary>
        /// Font size for text.
        /// </summary>
        public int? FontSize { get; set; }

        /// <summary>
        /// Effect to use while rendering this entity.
        /// This can be used by the host application as actual shader name, a flag identifier to change rendering, or any other purpose.
        /// </summary>
        public string? EffectIdentifier { get; set; }

        /// <summary>
        /// Internal padding, in pixels.
        /// Padding decrease the size of the internal bounding rect of controls.
        /// </summary>
        public Sides? Padding { get; set; }

        /// <summary>
        /// Expand the bounding rectangle of this entity by these values.
        /// </summary>
        public Sides? ExtraSize { get; set; }

        /// <summary>
        /// Extra offset to add to controls with auto anchor from their siblings.
        /// </summary>
        public Point? MarginBefore { get; set; }

        /// <summary>
        /// Extra offset to add to next controls in parent that has auto anchor from this entity.
        /// </summary>
        public Point? MarginAfter { get; set; }

        public StyleSheetState DeepCopy()
        {
            StyleSheetState copy = (StyleSheetState)MemberwiseClone();
            copy.FillTextureFramed = FillTextureFramed?.DeepCopy();
            copy.FillTextureStretched = FillTextureStretched?.DeepCopy();
            copy.Icon = Icon?.DeepCopy();
            copy.FillColor = FillColor.HasValue ? new Color(FillColor.Value.R, FillColor.Value.G, FillColor.Value.B, FillColor.Value.A) : null;
            copy.TextAlignment = TextAlignment;
            copy.FontIdentifier = new string(FontIdentifier);
            copy.TextFillColor = TextFillColor.HasValue ? new Color(TextFillColor.Value.R, TextFillColor.Value.G, TextFillColor.Value.B, TextFillColor.Value.A) : null;
            copy.NoValueTextFillColor = NoValueTextFillColor.HasValue ? new Color(NoValueTextFillColor.Value.R, NoValueTextFillColor.Value.G, NoValueTextFillColor.Value.B, NoValueTextFillColor.Value.A) : null;
            copy.TextOutlineColor = TextOutlineColor.HasValue ? new Color(TextOutlineColor.Value.R, TextOutlineColor.Value.G, TextOutlineColor.Value.B, TextOutlineColor.Value.A) : null;
            copy.TextOutlineWidth = TextOutlineWidth;
            copy.TextSpacing = TextSpacing;
            copy.FontSize = FontSize;
            copy.EffectIdentifier = EffectIdentifier;
            copy.Padding = Padding.HasValue ? new Sides(Padding.Value.Left, Padding.Value.Right, Padding.Value.Top, Padding.Value.Bottom) : null;
            copy.ExtraSize = ExtraSize.HasValue ? new Sides(ExtraSize.Value.Left, ExtraSize.Value.Right, ExtraSize.Value.Top, ExtraSize.Value.Bottom) : null;
            copy.MarginBefore = MarginBefore.HasValue ? new Point(MarginBefore.Value.X, MarginBefore.Value.Y) : null;
            copy.MarginAfter = MarginAfter.HasValue ? new Point(MarginAfter.Value.X, MarginAfter.Value.Y) : null;
            return copy;
        }
    }

    /// <summary>
    /// Define the graphics style of a GUI entity.
    /// </summary>
    public class StyleSheet
    {
        // cached property getters
        static Dictionary<string, PropertyInfo> _cachedProperties = new();

        /// <summary>
        /// Default entity width.
        /// </summary>
        public Measurement? DefaultWidth { get; set; }

        /// <summary>
        /// Default entity height.
        /// </summary>
        public Measurement? DefaultHeight { get; set; }

        /// <summary>
        /// Control min width.
        /// </summary>
        public int? MinWidth { get; set; }

        /// <summary>
        /// Control min height.
        /// </summary>
        public int? MinHeight { get; set; }

        /// <summary>
        /// Default anchor for the entity.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Anchor? DefaultAnchor { get; set; }

        /// <summary>
        /// Default text anchor for controls with text.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Anchor? DefaultTextAnchor { get; set; }

        /// <summary>
        /// Default stylesheet.
        /// </summary>
        public StyleSheetState? Default { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is targeted.
        /// </summary>
        public StyleSheetState? Targeted { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is interacted with.
        /// </summary>
        public StyleSheetState? Interacted { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is checked.
        /// </summary>
        public StyleSheetState? Checked { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is targeted checked.
        /// </summary>
        public StyleSheetState? TargetedChecked { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is disabled.
        /// </summary>
        public StyleSheetState? Disabled { get; set; }

        /// <summary>
        /// Stylesheet for when the entity is disabled, but also checked.
        /// </summary>
        public StyleSheetState? DisabledChecked { get; set; }

        // stylesheet to use for null state.
        static StyleSheetState _nullStyle = new StyleSheetState();

        /// <summary>
        /// If bigger than 0, will interpolate between states of the entity using this value as speed factor.
        /// If this value is 0 (default), will not perform interpolation.
        /// </summary>
        /// <remarks>This value affect textures and colors, but not other state properties.</remarks>
        public float? InterpolateStatesSpeed { get; set; }

        /// <summary>
        /// If bigger than 0, will interpolate internal moving parts of this entity using this value as speed factor.
        /// If this value is 0, will not perform interpolation on offset.
        /// Note: this does not affect the entire entity offset, its only for things like slider entity handle offset and internal mechanisms.
        /// Default value is 0.
        /// </summary>
        public float? InterpolateOffsetsSpeed { get; set; }

        public StyleSheet DeepCopy()
        {
            StyleSheet copy = (StyleSheet)MemberwiseClone();
            copy.DefaultWidth = DefaultWidth.HasValue ? new Measurement() { Value = DefaultWidth.Value.Value, Units = DefaultWidth.Value.Units } : null;
            copy.DefaultHeight = DefaultHeight.HasValue ? new Measurement() { Value = DefaultHeight.Value.Value, Units = DefaultHeight.Value.Units } : null;
            copy.MinWidth = MinWidth;
            copy.MinHeight = MinHeight;
            copy.DefaultAnchor = DefaultAnchor;
            copy.DefaultTextAnchor = DefaultTextAnchor;
            copy.Default = Default?.DeepCopy();
            copy.Targeted = Targeted?.DeepCopy();
            copy.Interacted = Interacted?.DeepCopy();
            copy.Checked = Checked?.DeepCopy();
            copy.TargetedChecked = TargetedChecked?.DeepCopy();
            copy.Disabled = Disabled?.DeepCopy();
            copy.DisabledChecked = DisabledChecked?.DeepCopy();
            copy.InterpolateStatesSpeed = InterpolateStatesSpeed;
            copy.InterpolateOffsetsSpeed = InterpolateOffsetsSpeed;
            return copy;
        }

        /// <summary>
        /// Get stylesheet for a given entity state.
        /// </summary>
        public StyleSheetState GetStyle(ControlState state)
        {
            StyleSheetState? ret = Default;
            switch (state)
            {
                case ControlState.Disabled: ret = Disabled; break;
                case ControlState.DisabledChecked: ret = DisabledChecked; break;
                case ControlState.Interacted: ret = Interacted; break;
                case ControlState.Targeted: ret = Targeted; break;
                case ControlState.TargetedChecked: ret = TargetedChecked; break;
                case ControlState.Checked: ret = Checked; break;
            }
            return ret ?? _nullStyle;
        }

        /// <summary>
        /// Get property value by entity state, or return Default if not set for given state.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <param name="state">Control state to get property for.</param>
        /// <param name="defaultValue">Default value to return if not found.</param>
        /// <param name="overrideProperties">If provided, will first attempt to get property from this state.</param>
        /// <returns>Value from state, from default, or the given default</returns>
        public T? GetProperty<T>(string propertyName, ControlState state, T? defaultValue, StyleSheetState? overrideProperties)
        {
            // get property info
            if (!_cachedProperties.TryGetValue(propertyName, out var propertyInfo))
            {
                propertyInfo = typeof(StyleSheetState).GetProperty(propertyName);
                if (propertyInfo == null) { throw new Exception($"Stylesheet property '{propertyName}' is not defined!"); }
                _cachedProperties[propertyName] = propertyInfo;
            }

            // get state and value
            var stylesheet = GetStyle(state);
            bool valueFound = false; // <-- due to c# stupidity, you can't set T? to null properly..
            T? ret = default; 
            try
            {
                object? value = propertyInfo.GetValue(stylesheet);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException($"Stylesheet property name '{propertyName}' type is '{propertyInfo.GetType().Name}', but it was requested as type '{typeof(T).Name}'!", e);
            }

            // check if we got override style
            if (overrideProperties != null)
            {
                object? value = propertyInfo.GetValue(overrideProperties);
                if (value != null) 
                { 
                    ret = (T?)value;
                    return ret;
                }
            }

            // special: if state is disabled checked and not defined, revert to disabled state before trying default
            if (!valueFound && (state == ControlState.DisabledChecked) && (Disabled != null)) 
            {
                object? value = propertyInfo.GetValue(Disabled);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }

            // special: if state is targeted checked and not defined, revert to checked state before trying default
            if (!valueFound && (state == ControlState.TargetedChecked) && (Checked != null)) 
            {
                object? value = propertyInfo.GetValue(Checked);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }

            // special: if state is targeted checked and not defined not even in checked, revert to interacted state before trying default
            if (!valueFound && (state == ControlState.TargetedChecked) && (Interacted != null))
            {
                object? value = propertyInfo.GetValue(Interacted);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }

            // special: if state is checked and not defined, revert to interacted state before trying default
            if (!valueFound && (state == ControlState.Checked) && (Interacted != null))
            {
                object? value = propertyInfo.GetValue(Interacted);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }

            // if not set and not default, try to get from default
            if (!valueFound && (stylesheet != Default) && (Default != null)) 
            { 
                object? value = propertyInfo.GetValue(Default);
                if (value != null) { ret = (T?)value; valueFound = true; }
            }

            // return value or default
            return valueFound ? ret : defaultValue;
        }

        /// <summary>
        /// Load and return stylesheet from Json file content.
        /// </summary>
        /// <param name="content">Json file content.</param>
        /// <returns>Loaded stylesheet.</returns>
        public static StyleSheet LoadFromJsonMemory(string content)
        {
            return JsonSerializer.Deserialize<StyleSheet>(content, JsonConverters.SerializerOptions)!;
        }

        /// <summary>
        /// Load and return stylesheet from JSON file path.
        /// </summary>
        /// <param name="filename">Json file path.</param>
        /// <returns>Loaded stylesheet.</returns>
        public static StyleSheet LoadFromJsonFile(string filename)
        {
            return LoadFromJsonMemory(File.ReadAllText(filename));
        }

        /// <summary>
        /// Serialize this stylesheet into Json content.
        /// </summary>
        /// <returns>Json content.</returns>
        public string SaveToJsonMemory()
        {
            return JsonSerializer.Serialize(this, JsonConverters.SerializerOptions);
        }

        /// <summary>
        /// Serialize this stylesheet into Json file.
        /// </summary>
        public void SaveToJsonFile(string filename)
        {
            File.WriteAllText(filename, SaveToJsonMemory());
        }
    }
}
