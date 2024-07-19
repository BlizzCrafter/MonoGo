using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;
using MonoGo.Engine.UI.Utils;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Base UI control.
    /// </summary>
    public class Control
    {
        /// <summary>
        /// Control identifier.
        /// </summary>
        public string? Identifier;

        /// <summary>
        /// Anchor for control position, based on its parent control.
        /// </summary>
        public Anchor Anchor = Anchor.AutoLTR;

        /// <summary>
        /// Return if this control has auto-type anchor.
        /// </summary>
        internal bool IsAutoAnchor => ((int)Anchor >= (int)Anchor.AutoLTR) && ((int)Anchor <= (int)Anchor.AutoCenter);

        /// <summary>
        /// Control offset, based on its anchor in parent control.
        /// </summary>
        public MeasureVector Offset;

        /// <summary>
        /// Control size.
        /// </summary>
        public MeasureVector Size;

        /// <summary>
        /// Control events you can register to.
        /// </summary>
        public ControlEvents Events;

        /// <summary>
        /// Define if the user can drag this control around.
        /// </summary>
        public DraggableMode DraggableMode = DraggableMode.NotDraggable;

        /// <summary>
        /// Return if this control can be dragged.
        /// </summary>
        public bool IsDraggable => DraggableMode != DraggableMode.NotDraggable;

        /// <summary>
        /// If true, once this control becomes the active target it will lock itself as active until this flag turns false.
        /// </summary>
        internal virtual bool LockFocusOnSelf => false;

        // for dragging
        Point? _draggedPosition;
        Point? _dragHandlePosition;
        Point? _dragOffsetFromParent;
        Point _dragHandleOffset;

        /// <summary>
        /// If true, this control will ignore scrollbar offset.
        /// </summary>
        internal bool IgnoreScrollOffset;

        /// <summary>
        /// If true, will include this control when calculating auto anchors for internal controls.
        /// </summary>
        internal bool IncludeInInternalAutoAnchorCalculation = true;

        /// <summary>
        /// If true, it means this control was dragged to a different position than its default anchored position.
        /// </summary>
        internal bool WasDraggedToPosition => _draggedPosition.HasValue;

        /// <summary>
        /// Optional user data you can attach to controls.
        /// </summary>
        public object? UserData;

        /// <summary>
        /// If false, object will be in disabled state.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// If true, will set control width automatically to fit its children.
        /// </summary>
        public bool AutoWidth = false;

        /// <summary>
        /// If true, will set control height automatically to fit its children.
        /// </summary>
        public bool AutoHeight = false;

        /// <summary>
        /// If true, the control will not be interactable, but will not change its state to Disabled.
        /// This means it will be rendered in its default state, but ignore user input.
        /// </summary>
        public bool Locked = false;

        /// <summary>
        /// Is this control visible?
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Get if this control has a scrollbar.
        /// </summary>
        protected virtual bool HaveScrollbars => false;

        /// <summary>
        /// If true will interpolate between states when rendering this control.
        /// Affect textures and colors.
        /// </summary>
        internal bool InterpolateStates => StatesInterpolationSpeed > 0f;

        /// <summary>
        /// Interpolation speed for fill textures, if enabled.
        /// </summary>
        internal float StatesInterpolationSpeed => StyleSheet?.InterpolateStatesSpeed ?? 0f;

        /// <summary>
        /// If true will interpolate between offsets of internal controls when rendering this control.
        /// Affect things like handle of a slider control and other moving parts.
        /// </summary>
        internal bool InterpolateHandlePosition => HandleInterpolationSpeed > 0f;

        /// <summary>
        /// Interpolation speed for offsets, if enabled.
        /// </summary>
        internal float HandleInterpolationSpeed => StyleSheet?.InterpolateOffsetsSpeed ?? 0f;

        /// <summary>
        /// Control min width.
        /// </summary>
        internal int MinWidth => StyleSheet?.MinWidth ?? 0;

        /// <summary>
        /// Control min height.
        /// </summary>
        internal int MinHeight => StyleSheet?.MinHeight ?? 0;

        // interpolation for next state
        protected float _interpolateToNextState = 0f;
        protected ControlState _lastState = ControlState.Default;
        protected ControlState _prevState = ControlState.Default;

        // to prevent 'flickering' with interaction state in case user perform quick clicks, we "lock" state to active when we switch to it for few ms
        float _timeToRemainInteractedState = 0f;

        /// <summary>
        /// If set, cursor will be rendered with these properties whenever we point on this control.
        /// This will override any other cursor behavior. For example, even if you have a custom cursor for disabled controls and this control is disabled,
        /// when pointing on it this cursor will be used instead of the disabled cursor style.
        /// </summary>
        public CursorProperties? CursorStyle;

        /// <summary>
        /// If true, this control will ignore all interactions and essentially be click-through.
        /// Unlike Disabled or Locked, this means that the control will never be the active target control, and it will not block the user from clicking controls under it.
        /// Also, this will not affect the state of the control, and the state will just remain default.
        /// </summary>
        public bool IgnoreInteractions = false;

        /// <summary>
        /// How to treat child controls that go out of this control bounds.
        /// </summary>
        public OverflowMode OverflowMode = OverflowMode.AllowOverflow;

        /// <summary>
        /// If true, it means this control can be interacted with by default.
        /// </summary>
        internal virtual bool Interactable => TransferInteractionsTo?.Interactable ?? _overrideInteractableState ?? false;

        // override interactable state for default controls
        internal bool? _overrideInteractableState = false;

        /// <summary>
        /// Stylesheet to use for this control.
        /// </summary>
        public StyleSheet StyleSheet;

        /// <summary>
        /// When set, this method is called before rendering this control, and it allows us to override its fill color.
        /// Parameters are the control and its current fill color, return value is new color to use.
        /// This method is useful to create color-based animations.
        /// </summary>
        public Func<Control, Color, Color>? ColorAnimator;

        /// <summary>
        /// Styles to override stylesheet defaults, regardless of control state.
        /// </summary>
        public StyleSheetState OverrideStyles = new();

        // child controls
        internal List<Control> _children = new();

        // internal child controls, for controls that are part of the control and cannot be removed
        List<Control> _internalChildren = new();

        // internal child controls, for controls that are part of the control and cannot be removed, but rendered on top of everything
        List<Control> _internalChildrenTopMost = new();

        /// <summary>
        /// If true, it means we need to lock focus on this control while mouse is held down.
        /// </summary>
        internal virtual bool LockFocusWhileMouseDown => IsDraggable;

        /// <summary>
        /// If true, it means this control can get focus while mouse is down and pressed outside of it.
        /// </summary>
        internal virtual bool CanGetFocusWhileMouseIsDown => TransferInteractionsTo?.CanGetFocusWhileMouseIsDown ?? true;

        /// <summary>
        /// If true, this control will get priority for interactions.
        /// </summary>
        internal virtual bool TopMostInteractions => (Parent != null) && Parent.TopMostInteractions;

        /// <summary>
        /// Last drawn internal bounding rect.
        /// </summary>
        public Rectangle LastInternalBoundingRect { get; private set; }

        /// <summary>
        /// Last drawn bounding rect.
        /// </summary>
        public Rectangle LastBoundingRect { get; private set; }

        /// <summary>
        /// Last drawn visible bounding rect, with parents visible region taken into consideration.
        /// </summary>
        public Rectangle LastVisibleBoundingRect { get; private set; }

        /// <summary>
        /// If set, will copy the state of this control instead of calculating its own state.
        /// </summary>
        internal Control? CopyStateFrom;

        /// <summary>
        /// If set, will transfer interactions to this control instead of self.
        /// For example, if we point on this control, it will be as if we point on the other control.
        /// </summary>
        internal Control? TransferInteractionsTo;

        /// <summary>
        /// If false, will not draw fill textures for this control.
        /// </summary>
        internal bool DrawFillTexture = true;

        /// <summary>
        /// If set, will lock this control to this state.
        /// </summary>
        internal ControlState? LockedState;

        /// <summary>
        /// Current control state.
        /// </summary>
        public ControlState State
        {
            get
            {
                // special: locked state
                if (LockedState != null)
                {
                    _stateCached = LockedState.Value;
                    return _stateCached.Value;
                }

                // special: copy state from another control
                if (CopyStateFrom != null)
                {
                    _stateCached = CopyStateFrom.State;
                    return _stateCached.Value;
                }

                // special: locked in interactive state
                if (_timeToRemainInteractedState > 0f)
                {
                    _stateCached = ControlState.Interacted;
                    return _stateCached.Value;
                }

                // return cached value
                if (_stateCached.HasValue) 
                { 
                    return _stateCached.Value; 
                }
                // locked
                else if (IsCurrentlyLocked())
                {
                    _stateCached = _isChecked ? ControlState.Checked : ControlState.Default;
                }
                // disabled / disabled checked
                else if (IsCurrentlyDisabled())
                {
                    _stateCached = _isChecked ? ControlState.DisabledChecked : ControlState.Disabled;
                }
                // interacted state
                else if (IsBeingPressed)
                {
                    _stateCached = ControlState.Interacted;
                    if (_timeToRemainInteractedState <= 0f) 
                    {
                        _timeToRemainInteractedState = UISystem.TimeToLockInteractiveState; 
                    }
                }
                // targeted / targeted checked
                else if (IsTargeted)
                {
                    _stateCached = _isChecked ? ControlState.TargetedChecked : ControlState.Targeted;
                }
                // default state
                else
                {
                    _stateCached = _isChecked ? ControlState.Checked : ControlState.Default;
                }

                return _stateCached.Value;
            }
        }

        // cached state value
        ControlState? _stateCached;

        // property to indicate if this control is checked or not.
        // for most controls types, this is never used. for controls that can be checked (Radio button, checkbox, button), this property is used.
        // we define it in base class because we need it for state calculation.
        protected bool _isChecked = false;

        /// <summary>
        /// If true, it means this control is currently being pressed.
        /// </summary>
        public bool IsBeingPressed => _wasMouseDownLastInteraction && IsTargeted;

        /// <summary>
        /// Return if this control is being targeted right now.
        /// </summary>
        public bool IsTargeted => UISystem.TargetedControl == this;

        // true if last time the mouse pointed on this control, left mouse button was down
        bool _wasMouseDownLastInteraction;

        /// <summary>
        /// Result of drawing method.
        /// Contains calculated bounding rectangles.
        /// </summary>
        public struct DrawMethodResult
        {
            public Rectangle InternalBoundingRect;
            public Rectangle BoundingRect;
            public Point ScrollOffset;
            public bool WasDragged;
        }

        /// <summary>
        /// Parent control.
        /// </summary>
        public Control? Parent { get; private set; } = null!;

        // empty stylesheet to use when no stylesheet is set
        static internal StyleSheet _emptyStylesheet = new();

        /// <summary>
        /// Create the control.
        /// </summary>
        /// <param name="stylesheet">Control stylesheet.</param>
        public Control(StyleSheet? stylesheet)
        {
            // store system and stylesheet
            StyleSheet = stylesheet ?? _emptyStylesheet;

            // set some defaults from stylesheet
            CalculateDefaultAnchorAndSize();
        }

        /// <summary>
        /// Calculate and set default anchor and size.
        /// </summary>
        protected void CalculateDefaultAnchorAndSize()
        {
            if (StyleSheet != null)
            {
                // set default size
                Size = GetDefaultControlTypeSize();
                if (StyleSheet.DefaultWidth.HasValue)
                {
                    Size.X = StyleSheet.DefaultWidth.Value;
                }
                if (StyleSheet.DefaultHeight.HasValue)
                {
                    Size.Y = StyleSheet.DefaultHeight.Value;
                }

                // set default text anchor
                if (this is Paragraph && StyleSheet.DefaultTextAnchor.HasValue)
                {
                    Anchor = StyleSheet.DefaultTextAnchor.Value;
                }
                // set default anchor for general controls
                else if (StyleSheet.DefaultAnchor.HasValue)
                {
                    Anchor = StyleSheet.DefaultAnchor.Value;
                }
                // default anchor for control type
                else
                {
                    Anchor = GetDefaultControlTypeAnchor();
                }
            }
        }

        /// <summary>
        /// If this control position was changed due to dragging, this method will reset position back to its original position before the dragging.
        /// </summary>
        public void ResetDraggedOffset()
        {
            _draggedPosition = null;
        }

        /// <summary>
        /// Return true if this control or any of its parent controls are locked.
        /// </summary>
        /// <returns>True if this control or one of its parents is locked.</returns>
        public bool IsCurrentlyLocked()
        {
            if (_lockedState.HasValue) { return _lockedState.Value; }

            // self is locked
            if (Locked)
            {
                _lockedState = true;
            }
            // check if parent is locked
            else if (Parent != null && Parent.IsCurrentlyLocked())
            {
                _lockedState = true;
            }
            // not locked
            else
            {
                _lockedState = false;
            }

            return _lockedState.Value;
        }
        bool? _lockedState;

        /// <summary>
        /// Return true if this control or any of its parent controls are disabled.
        /// </summary>
        /// <returns>True if this control or one of its parents is disabled.</returns>
        public bool IsCurrentlyDisabled()
        {
            if (_disabledState.HasValue) { return _disabledState.Value; }

            // self is disabled
            if (!Enabled)
            {
                _disabledState = true;
            }
            // check if parent is disabled
            else if (Parent != null && Parent.IsCurrentlyDisabled())
            {
                _disabledState = true;
            }
            // not disabled
            else
            {
                _disabledState = false;
            }

            return _disabledState.Value;
        }
        bool? _disabledState;


        /// <summary>
        /// Return true if this control is actually visible, ie itself and all its parents are visible..
        /// </summary>
        /// <returns>True if this control and all of its parents are visible.</returns>
        public bool IsCurrentlyVisible()
        {
            if (_visibilityState.HasValue) { return _visibilityState.Value; }

            // self is not visible?
            if (!Visible)
            {
                _visibilityState = false;
            }
            // check if parent is not visible
            else if (Parent != null && !Parent.IsCurrentlyVisible())
            {
                _visibilityState = false;
            }
            // visible
            else
            {
                _visibilityState = true;
            }

            return _visibilityState.Value;
        }
        bool? _visibilityState;

        /// <summary>
        /// Get default control size for this control type.
        /// </summary>
        protected virtual MeasureVector GetDefaultControlTypeSize()
        {
            var ret = new MeasureVector();
            ret.SetPercents(100f, 100f);
            return ret;
        }

        /// <summary>
        /// Get default control anchor for this control type.
        /// </summary>
        protected virtual Anchor GetDefaultControlTypeAnchor()
        {
            return Anchor.AutoLTR;
        }

        /// <summary>
        /// Add a child control internally.
        /// This is transparent to the user, and the control cannot be removed.
        /// </summary>
        /// <param name="child">Child control to add.</param>
        /// <param name="topMost">If true, will add internal child in a way that it will be rendered on top.</param>
        /// <exception cref="Exception">Thrown if child already have a parent.</exception>
        protected void AddChildInternal(Control child, bool topMost = false)
        {
            if (child.Parent != null) { throw new Exception("Internal Control to add as child already have a parent control! Remove it first."); }
            if (topMost)
            {
                _internalChildrenTopMost.Add(child);
            }
            else
            {
                _internalChildren.Add(child);
            }
            child.Parent = this;
        }

        /// <summary>
        /// Remove a child control internally.
        /// This is transparent to the user, and the control cannot be re-added.
        /// </summary>
        /// <param name="child">Child control to remove.</param>
        /// <exception cref="Exception">Thrown if child have a different parent or don't have a parent at all.</exception>
        protected void RemoveChildInternal(Control child)
        {
            if (child.Parent != this) { throw new Exception("Internal Control to remove is not a child of this parent control!"); }
            _internalChildren.Remove(child);
            _internalChildrenTopMost.Remove(child);
            child.Parent = null!;
        }

        /// <summary>
        /// Add a child control.
        /// </summary>
        /// <param name="child">Child control to add.</param>
        /// <exception cref="Exception">Thrown if child already have a parent.</exception>
        public T AddChild<T>(T child) where T : Control
        {
            if (child.Parent != null) { throw new Exception("Control to add as child already have a parent control! Remove it first."); }
            _children.Add(child);
            child.Parent = this;
            return child;
        }

        /// <summary>
        /// Remove a child control.
        /// </summary>
        /// <param name="child">Child control to remove.</param>
        /// <exception cref="Exception">Thrown if child have a different parent or don't have a parent at all.</exception>
        public void RemoveChild(Control child)
        {
            if (child.Parent != this) { throw new Exception("Control to remove is not a child of this parent control!"); }
            _children.Remove(child);
            child.Parent = null!;
        }

        /// <summary>
        /// Remove self from parent.
        /// </summary>
        public void RemoveSelf()
        {
            Parent?.RemoveChild(this);
        }

        /// <summary>
        /// Perform mouse wheel scroll action on this control.
        /// </summary>
        /// <param name="val">Mouse wheel scroll value.</param>
        internal virtual void PerformMouseWheelScroll(int val)
        {
            Parent?.PerformMouseWheelScroll(val);
        }

        /// <summary>
        /// Debug-render this control properties.
        /// </summary>
        internal virtual void DebugDraw(bool debugDrawChildren)
        {
            // skip if not visible
            if (!Visible) { return; }

            // check if should skip
            bool interactable = true;
            if (!(this is Paragraph))
            {
                if (!Interactable || IgnoreInteractions) { interactable = false; }
            }

            // pick fill color
            Color fillColor;
            if (IsCurrentlyLocked())
            {
                fillColor = new Color(255, 0, 0, 65);
            }
            else if (IsCurrentlyDisabled())
            {
                fillColor = new Color(255, 0, 255, 65);
            }
            else if (IsTargeted)
            {
                fillColor = new Color((byte)0, (byte)255, (byte)Math.Clamp(100 + 155 * Math.Sin(UISystem.ElapsedTime * 5), (byte)0, (byte)255), (byte)50);
            }
            else if (Interactable)
            {
                fillColor = new Color(0, 255, 0, 50);
            }
            else
            {
                fillColor = new Color(0, 155, 0, 50);
            }

            // draw internal rect
            Renderer.DrawRectangle(LastInternalBoundingRect, fillColor);

            // draw external rect
            if (interactable)
            {
                Renderer.DrawRectangle(LastBoundingRect, fillColor);
            }

            // draw anchor
            Point? anchorPosition = null;
            switch (Anchor)
            {
                case Anchor.TopLeft:
                    anchorPosition = new Point(LastBoundingRect.Left, LastBoundingRect.Top);
                    break;

                case Anchor.TopCenter:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width / 2, LastBoundingRect.Top);
                    break;

                case Anchor.TopRight:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width, LastBoundingRect.Top);
                    break;

                case Anchor.CenterLeft:
                    anchorPosition = new Point(LastBoundingRect.Left, LastBoundingRect.Top + LastBoundingRect.Height / 2);
                    break;

                case Anchor.Center:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width / 2, LastBoundingRect.Top + LastBoundingRect.Height / 2);
                    break;

                case Anchor.CenterRight:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width, LastBoundingRect.Top + LastBoundingRect.Height / 2);
                    break;

                case Anchor.BottomLeft:
                    anchorPosition = new Point(LastBoundingRect.Left, LastBoundingRect.Top + LastBoundingRect.Height);
                    break;

                case Anchor.BottomCenter:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width / 2, LastBoundingRect.Top + LastBoundingRect.Height);
                    break;

                case Anchor.BottomRight:
                    anchorPosition = new Point(LastBoundingRect.Left + LastBoundingRect.Width, LastBoundingRect.Top + LastBoundingRect.Height);
                    break;
            }
            if (anchorPosition.HasValue)
            {
                Renderer.DrawRectangle(new Rectangle(anchorPosition.Value.X - 6, anchorPosition.Value.Y - 6, 12, 12), new Color(255, 0, 0, 200));
            }

            // debug draw children
            if (debugDrawChildren)
            {
                foreach (var child in _children)
                {
                    child.DebugDraw(debugDrawChildren);
                }
            }

            // debug draw drag position
            if (_dragHandlePosition.HasValue)
            {
                Renderer.DrawRectangle(new Rectangle(_dragHandlePosition.Value.X - 6, _dragHandlePosition.Value.Y - 6, 12, 12), new Color(255, 255, 0, 255));
            }
        }

        /// <summary>
        /// Called after drawing a child.
        /// </summary>
        protected virtual void PostDrawingChild(DrawMethodResult? drawResult)
        {
        }

        /// <summary>
        /// Render this control and its children.
        /// </summary>
        /// <param name="parentDrawResult">Parent control last rendering results. Root controls will get bounding boxes covering the entire screen.</param>
        /// <param name="siblingDrawResult">Older sibling control last rendering results. For first control, it will be null.</param>
        /// <returns>Calculated bounding rectangles.</returns>
        internal DrawMethodResult _DoDraw(DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            // skip if not visible
            if (!Visible) { return new DrawMethodResult(); }

            Events.BeforeDraw?.Invoke(this);
            UISystem.Events.BeforeDraw?.Invoke(this);

            // draw self and get bounding rect
            var selfRect = Draw(parentDrawResult, siblingDrawResult);

            // set last bounding rects
            LastBoundingRect = selfRect.BoundingRect;
            LastInternalBoundingRect = selfRect.InternalBoundingRect;
            var scissorRegion = Renderer.GetScissorRegion();
            if (scissorRegion.HasValue)
            {
                LastVisibleBoundingRect = Extensions.MergeRectangles(LastBoundingRect, scissorRegion.Value);
            }
            else
            {
                LastVisibleBoundingRect = LastBoundingRect;
            }

            // apply scissor to hide overflow controls
            bool setScissor = (OverflowMode == OverflowMode.HideOverflow);
            bool enqueuedPreviousScissorRegion = false;
            if (setScissor)
            {
                // if we already have scissor region set, queue it.
                var newRegion = LastInternalBoundingRect;
                var currentRegion = Renderer.GetScissorRegion();
                if (currentRegion.HasValue)
                {
                    UISystem._scissorRegionQueue.Enqueue(currentRegion.Value);
                    enqueuedPreviousScissorRegion = true;
                    newRegion = Extensions.MergeRectangles(newRegion, currentRegion.Value);
                }

                // set region
                Renderer.SetScissorRegion(newRegion);
                
            }

            // draw internal children that are not affected by scrollbar
            siblingDrawResult = null;
            foreach (var child in _internalChildren)
            {
                if (child.Visible && child.IgnoreScrollOffset)
                {
                    var newSiblingDrawResult = child._DoDraw(selfRect, siblingDrawResult);
                    if (child.IncludeInInternalAutoAnchorCalculation) { siblingDrawResult = newSiblingDrawResult; }
                }
            }

            // get self scrolling offset and apply it
            var selfRectScrolled = selfRect;
            if (HaveScrollbars)
            {
                var scrollOffset = GetScrollOffset();
                var scrollPadding = GetScrollExtraPadding();
                
                selfRectScrolled.BoundingRect.X += scrollOffset.X;
                selfRectScrolled.BoundingRect.Y += scrollOffset.Y;
                selfRectScrolled.InternalBoundingRect.X += scrollOffset.X;
                selfRectScrolled.InternalBoundingRect.Y += scrollOffset.Y;

                selfRectScrolled.InternalBoundingRect.X += scrollPadding.Left;
                selfRectScrolled.InternalBoundingRect.Width -= scrollPadding.Right + scrollPadding.Left;
                selfRectScrolled.InternalBoundingRect.Y += scrollPadding.Top;
                selfRectScrolled.InternalBoundingRect.Height -= scrollPadding.Bottom + scrollPadding.Top;
            }

            // draw internal children affected by scrollbar
            siblingDrawResult = null;
            foreach (var child in _internalChildren)
            {
                if (child.Visible && !child.IgnoreScrollOffset)
                {
                    var newSiblingDrawResult = child._DoDraw(selfRectScrolled, siblingDrawResult);
                    if (child.IncludeInInternalAutoAnchorCalculation) { siblingDrawResult = newSiblingDrawResult; }
                }
            }

            // draw children
            siblingDrawResult = null;
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (var child in _children)
            {
                if (child.Visible)
                {
                    // draw child
                    siblingDrawResult = child._DoDraw(child.IgnoreScrollOffset ? selfRect : selfRectScrolled, siblingDrawResult);
                    PostDrawingChild(siblingDrawResult);

                    // adjust auto size
                    if (AutoWidth) { 
                        maxWidth = (int)MathF.Max(maxWidth, siblingDrawResult.Value.BoundingRect.Right - LastBoundingRect.Left + (LastBoundingRect.Width - LastInternalBoundingRect.Width) / 2); 
                    }
                    if (AutoHeight) { 
                        maxHeight = (int)MathF.Max(maxHeight, siblingDrawResult.Value.BoundingRect.Bottom - LastBoundingRect.Top + (LastBoundingRect.Height - LastInternalBoundingRect.Height) / 2); 
                    }
                }
            }

            // draw top internal children
            siblingDrawResult = null;
            foreach (var child in _internalChildrenTopMost)
            {
                if (child.Visible)
                {
                    var newSiblingDrawResult = child._DoDraw(selfRect, siblingDrawResult);
                    if (child.IncludeInInternalAutoAnchorCalculation) { siblingDrawResult = newSiblingDrawResult; }
                }
            }

            // reset scissor region if set
            if (setScissor)
            {
                if (enqueuedPreviousScissorRegion && UISystem._scissorRegionQueue.TryDequeue(out Rectangle lastRegion))
                {
                    Renderer.SetScissorRegion(lastRegion);
                }
                else
                {
                    Renderer.ClearScissorRegion();
                }
            }

            // set auto size
            SetAutoSizes(maxWidth, maxHeight);

            // trigger event
            Events.AfterDraw?.Invoke(this);
            UISystem.Events.AfterDraw?.Invoke(this);

            // return self internal bounding rect
            return selfRect;
        }

        /// <summary>
        /// Implemented setting auto width / auto height.
        /// </summary>
        /// <param name="maxWidth">Max width, calculated based on children.</param>
        /// <param name="maxHeight">Max height, calculated based on children.</param>
        protected virtual void SetAutoSizes(int maxWidth, int maxHeight)
        {
            // set auto size
            if (AutoWidth)
            {
                Size.X.SetPixels(maxWidth);
            }
            if (AutoHeight)
            {
                Size.Y.SetPixels(maxHeight);
            }
        }

        /// <summary>
        /// Get current style margin-before property.
        /// </summary>
        internal Point GetMarginBefore() { return StyleSheet.GetProperty("MarginBefore", State, Point.Zero, OverrideStyles); }

        /// <summary>
        /// Get current style margin-after property.
        /// </summary>
        internal Point GetMarginAfter() { return StyleSheet.GetProperty("MarginAfter", State, Point.Zero, OverrideStyles); }

        /// <summary>
        /// Get current style extra-size property.
        /// </summary>
        internal Sides GetExtraSize() { return StyleSheet.GetProperty("ExtraSize", State, Sides.Zero, OverrideStyles); }

        /// <summary>
        /// Get current style padding property.
        /// </summary>
        internal Sides GetPadding() { return StyleSheet.GetProperty("Padding", State, Sides.Zero, OverrideStyles); }

        /// <summary>
        /// Implement per-control rendering.
        /// </summary>
        /// <param name="parentDrawResult">Parent control last rendering results. Root controls will get bounding boxes covering the entire screen.</param>
        /// <param name="siblingDrawResult">Older sibling control last rendering results. For first control, it will be null.</param>
        /// <returns>Calculated bounding rectangles.</returns>
        protected virtual DrawMethodResult Draw(DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            // calculate bounding rect
            Rectangle boundingRect = CalculateBoundingRect(parentDrawResult, siblingDrawResult);

            // add extra size from stylesheet
            var extraSize = GetExtraSize();
            boundingRect.X -= extraSize.Left;
            boundingRect.Width += extraSize.Left + extraSize.Right;
            boundingRect.Y -= extraSize.Top;
            boundingRect.Height += extraSize.Top + extraSize.Bottom;

            // calculate internal bounding rect
            var padding = GetPadding();
            Rectangle internalBoundingRect = boundingRect;
            internalBoundingRect.X += padding.Left;
            internalBoundingRect.Y += padding.Top;
            internalBoundingRect.Width -= padding.Left + padding.Right;
            internalBoundingRect.Height -= padding.Top + padding.Bottom;

            // perform rendering
            DrawControlType(ref boundingRect, ref internalBoundingRect, parentDrawResult, siblingDrawResult);

            // return rendering result
            return new DrawMethodResult() 
            { 
                BoundingRect = boundingRect, 
                InternalBoundingRect = internalBoundingRect,
                WasDragged = WasDraggedToPosition
            };
        }

        /// <summary>
        /// Get scrollbars offset, if scrollbars are set.
        /// </summary>
        protected virtual Point GetScrollOffset()
        {
            return Point.Zero;
        }

        /// <summary>
        /// Get customized extra padding.
        /// </summary>
        protected virtual Sides GetScrollExtraPadding()
        {
            return Sides.Zero;
        }

        /// <summary>
        /// Implement actual control rendering.
        /// </summary>
        /// <param name="boundingRect">Self bounding rect.</param>
        /// <param name="internalBoundingRect">Self internal bounding rect.</param>
        /// <param name="parentDrawResult">Parent draw call results.</param>
        protected virtual void DrawControlType(ref Rectangle boundingRect, ref Rectangle internalBoundingRect, DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            if (DrawFillTexture)
            {
                // get current state
                var state = State;

                // draw given state fill textures
                void DrawStateFill(ControlState state, Rectangle boundingRect, float alpha)
                {
                    // get color
                    var color = StyleSheet.GetProperty("FillColor", state, Color.White, OverrideStyles)!;

                    // animate colors
                    if (ColorAnimator != null)
                    {
                        color = ColorAnimator(this, color);
                    }

                    // apply alpha
                    if (alpha <= 1f)
                    {
                        color.A = (byte)((float)color.A * alpha);
                    }

                    // not visible? skip
                    if (color.A == 0) { return; }


                    // get effect
                    var effectId = StyleSheet.GetProperty<string>("EffectIdentifier", state, null, OverrideStyles);

                    // draw stretch texture
                    var stexture = StyleSheet.GetProperty<StretchedTexture>("FillTextureStretched", state, null, OverrideStyles);
                    if (stexture != null)
                    {
                        DrawUtils.Draw(effectId, stexture, boundingRect, color);
                    }

                    // draw icon texture
                    var sicon = StyleSheet.GetProperty<IconTexture>("Icon", state, null, OverrideStyles);
                    if (sicon != null)
                    {
                        var dest = new Rectangle(boundingRect.X, boundingRect.Y, (int)(sicon.SourceRect.Width * sicon.TextureScale), (int)(sicon.SourceRect.Height * sicon.TextureScale));
                        DrawUtils.Draw(effectId, sicon, dest, color);
                    }

                    // draw framed texture
                    var ftexture = StyleSheet.GetProperty<FramedTexture>("FillTextureFramed", state, null, OverrideStyles);
                    if (ftexture != null)
                    {
                        DrawUtils.Draw(effectId, ftexture, boundingRect, color);
                    }
                }

                // draw state with interpolation
                if (InterpolateStates && (_interpolateToNextState < 1f))
                {
                    DrawStateFill(_prevState, boundingRect, 1f);
                    DrawStateFill(state, boundingRect, _interpolateToNextState);
                }
                // draw current state without interpolation
                else
                {
                    DrawStateFill(state, boundingRect, 1f);
                }
            }
        }

        /// <summary>
        /// Called for every interactable control update we finish updates loop.
        /// </summary>
        /// <param name="inputState">Current input state.</param>
        internal virtual void PostUpdate(InputState inputState)
        {
        }

        /// <summary>
        /// Do interactions with this control while its being targeted.
        /// </summary>
        /// <param name="inputState">Current input state.</param>
        internal virtual void DoInteractions(InputState inputState)
        {
            // update being pressed state
            _wasMouseDownLastInteraction = inputState.LeftMouseDown;

            // do mouse wheel actions
            if (inputState.MouseWheelChange != 0)
            {
                PerformMouseWheelScroll(inputState.MouseWheelChange);
            }

            // do events
            {
                if (IsTargeted)
                {
                    Events.WhileMouseHover?.Invoke(this);
                    UISystem.Events.WhileMouseHover?.Invoke(this);
                }
                if (inputState.MouseWheelChange < 0)
                {
                    Events.OnMouseWheelScrollUp?.Invoke(this);
                    UISystem.Events.OnMouseWheelScrollUp?.Invoke(this);
                }
                if (inputState.MouseWheelChange > 0)
                {
                    Events.OnMouseWheelScrollDown?.Invoke(this);
                    UISystem.Events.OnMouseWheelScrollDown?.Invoke(this);
                }
                if (inputState.LeftMouseDown)
                {
                    Events.OnLeftMouseDown?.Invoke(this);
                    UISystem.Events.OnLeftMouseDown?.Invoke(this);
                }
                if (inputState.LeftMousePressedNow)
                {
                    Events.OnLeftMousePressed?.Invoke(this);
                    UISystem.Events.OnLeftMousePressed?.Invoke(this);
                }
                if (inputState.LeftMouseReleasedNow)
                {
                    Events.OnLeftMouseReleased?.Invoke(this);
                    UISystem.Events.OnLeftMouseReleased?.Invoke(this);
                }

                if (inputState.RightMouseDown)
                {
                    Events.OnRightMouseDown?.Invoke(this);
                    UISystem.Events.OnRightMouseDown?.Invoke(this);
                }
                if (inputState.RightMousePressedNow)
                {
                    Events.OnRightMousePressed?.Invoke(this);
                    UISystem.Events.OnRightMousePressed?.Invoke(this);
                }
                if (inputState.RightMouseReleasedNow)
                {
                    Events.OnRightMouseReleased?.Invoke(this);
                    UISystem.Events.OnRightMouseReleased?.Invoke(this);
                }
            }

            // drag control
            if (IsDraggable)
            {
                if (inputState.LeftMouseDown)
                {
                    // drag
                    if (_dragHandlePosition.HasValue)
                    {
                        // set dragged position
                        _draggedPosition = new Point(inputState.MousePosition.X + _dragHandleOffset.X, inputState.MousePosition.Y + _dragHandleOffset.Y);

                        // confine dragging
                        if ((DraggableMode == DraggableMode.DraggableConfinedToParent) || (DraggableMode == DraggableMode.DraggableConfinedToScreen))
                        {
                            var boundingRect = ((DraggableMode == DraggableMode.DraggableConfinedToParent) && (Parent != null)) ? Parent!.LastInternalBoundingRect : inputState.ScreenBounds;
                            var pos = _draggedPosition.Value;
                            if (pos.X < boundingRect.Left) { pos.X = boundingRect.Left; }
                            if (pos.Y < boundingRect.Top) { pos.Y = boundingRect.Top; }
                            if (pos.X + LastBoundingRect.Width > boundingRect.Right) { pos.X = boundingRect.Right - LastBoundingRect.Width; }
                            if (pos.Y + LastBoundingRect.Height > boundingRect.Bottom) { pos.Y = boundingRect.Bottom - LastBoundingRect.Height; }
                            _draggedPosition = pos;
                        }

                        // set offset from parent, so that if parent moves we move with it
                        if ((Parent != null) && (DraggableMode != DraggableMode.Draggable))
                        {
                            _dragOffsetFromParent = new Point(_draggedPosition.Value.X - Parent.LastInternalBoundingRect.X, _draggedPosition.Value.Y - Parent.LastInternalBoundingRect.Y);
                        }
                        else
                        {
                            _dragOffsetFromParent = null;
                        }
                    }
                    // start dragging
                    else if (inputState.LeftMousePressedNow)
                    {
                        _dragHandlePosition = inputState.MousePosition;
                        _dragHandleOffset = new Point(LastBoundingRect.X - inputState.MousePosition.X, LastBoundingRect.Y - inputState.MousePosition.Y);
                    }
                }
                // stop dragging
                else
                {
                    _dragHandlePosition = null;
                }
            }
        }

        /// <summary>
        /// Calculate and return bounding rectangle size in pixels.
        /// </summary>
        protected virtual Point CalculateBoundingRectSize(Rectangle parentIntRect)
        {
            var ret = new Point(Size.X.GetValueInPixels(parentIntRect.Width), Size.Y.GetValueInPixels(parentIntRect.Height));
            ret.X = Math.Max(ret.X, MinWidth);
            ret.Y = Math.Max(ret.Y, MinHeight);
            return ret;
        }

        /// <summary>
        /// Get the control before this control in the children list of its parent.
        /// </summary>
        /// <returns>Control that comes before this control, or null if there's none.</returns>
        internal Control? GetControlBefore()
        {
            // no parent? null
            if (Parent == null) { return null; }

            // get self index and if first control return null
            var selfIndex = Parent._children.IndexOf(this);
            if (selfIndex <= 0) { return null; }

            // return control before
            return Parent._children[selfIndex - 1];
        }

        /// <summary>
        /// Process parent internal bounding rect before using it.
        /// </summary>
        protected virtual Rectangle ProcessParentInternalRect(Rectangle parentIntRect)
        {
            return parentIntRect;
        }

        /// <summary>
        /// Calculate bounding rect.
        /// </summary>
        /// <param name="parentDrawResult">Parent draw results.</param>
        /// <param name="siblingDrawResult">Older sibling draw results.</param>
        /// <returns>Control bounding rect.</returns>
        internal protected virtual Rectangle CalculateBoundingRect(DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            // calculated bounding rect
            Rectangle boundingRect = new Rectangle();

            // get parent internal bounding rect
            var parentIntRect = ProcessParentInternalRect(parentDrawResult.InternalBoundingRect);

            // get margin
            var margin = GetMarginBefore();

            // add older sibling margin
            if (IsAutoAnchor)
            {
                var olderSibling = GetControlBefore();
                if ((olderSibling != null) && !olderSibling.WasDraggedToPosition)
                {
                    var parentMargin = olderSibling.GetMarginAfter();
                    margin.X += parentMargin.X;
                    margin.Y += parentMargin.Y;
                }
            }

            // calculate width and height
            var rectSize = CalculateBoundingRectSize(parentIntRect);
            boundingRect.Width = rectSize.X;
            boundingRect.Height = rectSize.Y;

            // if control wasn't dragged, calculate control position based on anchors and offset
            if (_draggedPosition == null)
            {
                // calculate bounding boxes based on on anchor
                Vector2 offsetFactor = new Vector2(1, 1);
                switch (Anchor)
                {
                    case Anchor.TopLeft:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X;
                            boundingRect.Y = parentIntRect.Y;
                        }
                        break;

                    case Anchor.TopCenter:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width / 2 - boundingRect.Width / 2;
                            boundingRect.Y = parentIntRect.Y;
                        }
                        break;

                    case Anchor.TopRight:
                        {
                            offsetFactor = new Vector2(-1, 1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width - boundingRect.Width;
                            boundingRect.Y = parentIntRect.Y;
                        }
                        break;

                    case Anchor.CenterLeft:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height / 2 - boundingRect.Height / 2;
                        }
                        break;

                    case Anchor.Center:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width / 2 - boundingRect.Width / 2;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height / 2 - boundingRect.Height / 2;
                        }
                        break;

                    case Anchor.CenterRight:
                        {
                            offsetFactor = new Vector2(-1, 1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width - boundingRect.Width;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height / 2 - boundingRect.Height / 2;
                        }
                        break;

                    case Anchor.BottomLeft:
                        {
                            offsetFactor = new Vector2(1, -1);
                            boundingRect.X = parentIntRect.X;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height - boundingRect.Height;
                        }
                        break;

                    case Anchor.BottomCenter:
                        {
                            offsetFactor = new Vector2(1, -1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width / 2 - boundingRect.Width / 2;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height - boundingRect.Height;
                        }
                        break;

                    case Anchor.BottomRight:
                        {
                            offsetFactor = new Vector2(-1, -1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width - boundingRect.Width;
                            boundingRect.Y = parentIntRect.Y + parentIntRect.Height - boundingRect.Height;
                        }
                        break;

                    case Anchor.AutoLTR:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X;
                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged)
                            {
                                boundingRect.Y = siblingDrawResult.Value.BoundingRect.Bottom + margin.Y;
                            }
                            else
                            {
                                boundingRect.Y = parentIntRect.Y;
                            }
                        }
                        break;

                    case Anchor.AutoRTL:
                        {
                            offsetFactor = new Vector2(-1, 1);
                            boundingRect.X = parentIntRect.Right - boundingRect.Width;
                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged)
                            {
                                boundingRect.Y = siblingDrawResult.Value.BoundingRect.Bottom + margin.Y;
                            }
                            else
                            {
                                boundingRect.Y = parentIntRect.Y;
                            }
                        }
                        break;

                    case Anchor.AutoCenter:
                        {
                            offsetFactor = new Vector2(1, 1);
                            boundingRect.X = parentIntRect.X + parentIntRect.Width / 2 - boundingRect.Width / 2;
                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged)
                            {
                                boundingRect.Y = siblingDrawResult.Value.BoundingRect.Bottom + margin.Y;
                            }
                            else
                            {
                                boundingRect.Y = parentIntRect.Y;
                            }
                        }
                        break;

                    case Anchor.AutoInlineLTR:
                        {
                            offsetFactor = new Vector2(1, 1);
                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged)
                            {
                                boundingRect.X = siblingDrawResult.Value.BoundingRect.Right + margin.X;
                                boundingRect.Y = siblingDrawResult.Value.BoundingRect.Y;
                            }
                            else
                            {
                                boundingRect.X = parentIntRect.X;
                                boundingRect.Y = parentIntRect.Y;
                            }

                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged && (boundingRect.Right > parentIntRect.Right))
                            {
                                boundingRect.X = parentIntRect.X;
                                boundingRect.Y += siblingDrawResult.Value.BoundingRect.Height + margin.Y;
                            }
                        }
                        break;

                    case Anchor.AutoInlineRTL:
                        {
                            offsetFactor = new Vector2(-1, 1);
                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged)
                            {
                                boundingRect.X = siblingDrawResult.Value.BoundingRect.Left - boundingRect.Width - margin.X;
                                boundingRect.Y = siblingDrawResult.Value.BoundingRect.Y;
                            }
                            else
                            {
                                boundingRect.X = parentIntRect.Right - boundingRect.Width;
                                boundingRect.Y = parentIntRect.Y;
                            }

                            if (siblingDrawResult.HasValue && !siblingDrawResult.Value.WasDragged && (boundingRect.Left < parentIntRect.Left))
                            {
                                boundingRect.X = parentIntRect.Right - boundingRect.Width;
                                boundingRect.Y += siblingDrawResult.Value.BoundingRect.Height + margin.Y;
                            }
                        }
                        break;
                }

                // calculate offset x
                int offsetX = Offset.X.GetValueInPixels(parentIntRect.Width);
                int offsetY = Offset.Y.GetValueInPixels(parentIntRect.Height);

                // add offset
                boundingRect.X += (int)(offsetFactor.X * offsetX);
                boundingRect.Y += (int)(offsetFactor.Y * offsetY);
            }
            // if control was dragged, take the dragged position
            else
            {
                boundingRect.X = _draggedPosition.Value.X;
                boundingRect.Y = _draggedPosition.Value.Y;
                if (_dragOffsetFromParent.HasValue && (Parent != null))
                {
                    var distanceX = boundingRect.X - Parent.LastInternalBoundingRect.X;
                    var distanceY = boundingRect.Y - Parent.LastInternalBoundingRect.Y;
                    boundingRect.X += _dragOffsetFromParent.Value.X - distanceX;
                    boundingRect.Y += _dragOffsetFromParent.Value.Y - distanceY;
                }
            }

            // return result
            return boundingRect;
        }

        /// <summary>
        /// Update this control and its children.
        /// </summary>
        /// <param name="dt">Delta time, in seconds, since last update frame.</param>
        internal void _DoUpdate(float dt)
        {
            PreFrameUpdates();

            // reduce time to be locked in interacted state
            if (_timeToRemainInteractedState > 0f)
            {
                _timeToRemainInteractedState -= dt;
            }

            // pre update event
            Events.BeforeUpdate?.Invoke(this);
            UISystem.Events.BeforeUpdate?.Invoke(this);

            // disable dragging if not active
            if (_dragHandlePosition.HasValue && !IsTargeted)
            {
                _dragHandlePosition = null;
            }

            // update self
            Update(dt);

            // update internal children and children
            foreach (var child in _internalChildren)
            {
                child._DoUpdate(dt);
            }
            foreach (var child in _internalChildrenTopMost)
            {
                child._DoUpdate(dt);
            }
            foreach (var child in _children)
            {
                child._DoUpdate(dt);
            }

            // post update event
            Events.AfterUpdate?.Invoke(this);
            UISystem.Events.AfterUpdate?.Invoke(this);
        }

        /// <summary>
        /// Reset control states and caches before a new frame begins.
        /// </summary>
        internal void PreFrameUpdates()
        {
            _stateCached = null;
            _lockedState = null;
            _disabledState = null;
            _visibilityState = null;
        }

        /// <summary>
        /// Implement per-control updates.
        /// </summary>
        /// <param name="dt">Delta time, in seconds, since last update frame.</param>
        protected virtual void Update(float dt)
        {
            // do textures interpolation
            if (InterpolateStates)
            {
                // switch state
                ControlState state = State;
                if (state != _lastState)
                {
                    _interpolateToNextState = Math.Max(0, 1f - _interpolateToNextState);
                    _prevState = _lastState;
                    _lastState = state;
                }
                // interpolate current state
                else
                {
                    if (_interpolateToNextState < 1f)
                    {
                        _interpolateToNextState += dt * StatesInterpolationSpeed;
                        if (_interpolateToNextState > 1f) { _interpolateToNextState = 1f; }
                    }
                }
            }
        }

        /// <summary>
        /// Check if this control is currently pointed on by a mouse position.
        /// </summary>
        /// <param name="cp">Cursor position.</param>
        /// <returns>True if control is being pointed on.</returns>
        public virtual bool IsPointedOn(Point cp, bool useVisibleRect = true)
        {
            if (useVisibleRect)
            {
                return (
                (cp.X >= LastVisibleBoundingRect.X) && (cp.X <= (LastVisibleBoundingRect.X + LastVisibleBoundingRect.Width)) &&
                (cp.Y >= LastVisibleBoundingRect.Y) && (cp.Y <= (LastVisibleBoundingRect.Y + LastVisibleBoundingRect.Height)));
            }
            return (
                (cp.X >= LastBoundingRect.X) && (cp.X <= (LastBoundingRect.X + LastBoundingRect.Width)) &&
                (cp.Y >= LastBoundingRect.Y) && (cp.Y <= (LastBoundingRect.Y + LastBoundingRect.Height)));
        }

        /// <summary>
        /// Iterate over all the direct children of this control.
        /// To walk the entire tree including children of children, use Walk() instead.
        /// </summary>
        /// <param name="callback">Callback to trigger per control. Return false to break iteration.</param>
        public void IterateChildren(Func<Control, bool> callback)
        {
            foreach (var child in _children)
            {
                if (!callback(child)) { return; }
            }
        }

        /// <summary>
        /// Iterate control and all its children and their children.
        /// First call will always be this control, then it will walk over its children tree.
        /// </summary>
        /// <param name="callback">Callback to handle controls. Return false to break iteration.</param>
        public void Walk(Func<Control, bool> callback)
        {
            bool cont = true;
            _WalkInt(callback, ref cont);
        }

        /// <summary>
        /// If true, when walking this control tree it will include internal children.
        /// </summary>
        protected virtual bool WalkInternalChildren => false;

        /// <summary>
        /// Internal walk implementation.
        /// </summary>
        void _WalkInt(Func<Control, bool> callback, ref bool cont)
        {
            if (!callback(this))
            {
                cont = false;
                return;
            }

            if (WalkInternalChildren)
            {
                foreach (var child in _internalChildren)
                {
                    child._WalkInt(callback, ref cont);
                    if (!cont) { return; }
                }
            }

            foreach (var child in _children)
            {
                child._WalkInt(callback, ref cont);
                if (!cont) { return; }
            }

            if (WalkInternalChildren)
            {
                foreach (var child in _internalChildrenTopMost)
                {
                    child._WalkInt(callback, ref cont);
                    if (!cont) { return; }
                }
            }
        }
    }

    /// <summary>
    /// Define if the control is draggable or not.
    /// </summary>
    public enum DraggableMode
    {
        /// <summary>
        /// Control is not draggable.
        /// </summary>
        NotDraggable,

        /// <summary>
        /// Control can be dragged anywhere freely.
        /// </summary>
        Draggable,

        /// <summary>
        /// Control can be dragged, but confined to screen internal rectangle.
        /// </summary>
        DraggableConfinedToScreen,

        /// <summary>
        /// Control can be dragged, but confined to parent internal rectangle.
        /// </summary>
        DraggableConfinedToParent
    }

    /// <summary>
    /// An event callback we can register to.
    /// </summary>
    /// <param name="control">Control the event occurred on.</param>
    public delegate void ControlEvent(Control control);

    /// <summary>
    /// Set of events we can register to on controls.
    /// </summary>
    public struct ControlEvents
    {
        /// <summary>
        /// Called when the value of this control changes.
        /// This includes:
        ///     1. Numeric value change for sliders and scrollbars.
        ///     2. Checked / unchecked value change for checkboxes and radio buttons.
        ///     3. Text input change for text input fields.
        ///     4. Selected item change for lists and dropdowns.
        /// </summary>
        public ControlEvent OnValueChanged;

        /// <summary>
        /// Called when this control is checked.
        /// </summary>
        public ControlEvent OnChecked;

        /// <summary>
        /// Called when this control is unchecked.
        /// </summary>
        public ControlEvent OnUnchecked;

        /// <summary>
        /// Called before rendering the control.
        /// </summary>
        public ControlEvent BeforeDraw;

        /// <summary>
        /// Called after rendering the control.
        /// </summary>
        public ControlEvent AfterDraw;

        /// <summary>
        /// Called before updating the control.
        /// </summary>
        public ControlEvent BeforeUpdate;

        /// <summary>
        /// Called after updating the control.
        /// </summary>
        public ControlEvent AfterUpdate;

        /// <summary>
        /// Called once when left mouse button is released on the control.
        /// This is just sugarcoat for 'OnLeftMouseReleased' to make code more readable.
        /// </summary>
        public ControlEvent OnClick
        { 
            get => OnLeftMouseReleased;
            set => OnLeftMouseReleased = value;
        }

        /// <summary>
        /// Called when mouse wheel scroll up.
        /// </summary>
        public ControlEvent OnMouseWheelScrollUp;

        /// <summary>
        /// Called when mouse wheel scroll down.
        /// </summary>
        public ControlEvent OnMouseWheelScrollDown;

        /// <summary>
        /// Called every frame while mouse left button is down over the control.
        /// </summary>
        public ControlEvent OnLeftMouseDown;

        /// <summary>
        /// Called once when left mouse button is pressed on the control.
        /// </summary>
        public ControlEvent OnLeftMousePressed;

        /// <summary>
        /// Called once when left mouse button is released on the control.
        /// </summary>
        public ControlEvent OnLeftMouseReleased;

        /// <summary>
        /// Called every frame while mouse right button is down over the control.
        /// </summary>
        public ControlEvent OnRightMouseDown;

        /// <summary>
        /// Called once when right mouse button is pressed on the control.
        /// </summary>
        public ControlEvent OnRightMousePressed;

        /// <summary>
        /// Called once when right mouse button is released on the control.
        /// </summary>
        public ControlEvent OnRightMouseReleased;

        /// <summary>
        /// Called every frame while mouse hovers the control (even if mouse don't move).
        /// </summary>
        public ControlEvent WhileMouseHover;
    }
}
