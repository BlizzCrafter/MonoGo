using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A list of items users can select items from, that collapses while not being interacted with.
    /// </summary>
    public class DropDown : ListBox
    {
        /// <summary>
        /// Is the dropdown currently opened?
        /// </summary>
        public bool IsOpened { get; private set; }

        /// <inheritdoc/>
        protected override bool ShowListScrollbar => base.ShowListScrollbar && IsOpened;

        /// <summary>
        /// Text to show when no value is selected and dropdown is collapsed.
        /// </summary>
        public string? DefaultSelectedText = null;

        /// <inheritdoc/>
        internal override bool TopMostInteractions => IsOpened;

        /// <summary>
        /// If defined, this will be the text to display when the dropdown is collapsed, regardless of the currently selected item or default text.
        /// </summary>
        public string? OverrideSelectedText = null;

        /// <summary>
        /// Styles to override stylesheet defaults, regardless of Control state, for the paragraph showing the selected value in closed state.
        /// </summary>
        public StyleSheetState OverrideClosedStateTextStyles
        {
            get => _selectedValueParagraph.OverrideStyles;
            set => _selectedValueParagraph.OverrideStyles = value;
        }

        // panel for selected value text
        Panel _selectedValuePanel;
        Paragraph _selectedValueParagraph;

        // icon control, if stylesheet is defined for it
        Control _icon = null!;
        /// <summary>
        /// Create the drop down.
        /// </summary>
        /// <param name="stylesheet">Drop down panel stylesheet.</param>
        /// <param name="itemsStylesheet">Drop down box items stylesheet. If not set, will use the same as base stylesheet.</param>
        /// <param name="arrowIconStylesheet">Stylesheet for an arrow icon to add to the dropdown to reflect its state. If null, will not add this Control.</param>
        public DropDown(StyleSheet? stylesheet, StyleSheet? itemsStylesheet = null, StyleSheet? arrowIconStylesheet = null) : base(stylesheet, itemsStylesheet)
        {
            // set as auto-height by default
            AutoHeight = true;

            // create panel with selected value paragraph
            _selectedValuePanel = new Panel(stylesheet);
            _selectedValueParagraph = _selectedValuePanel.AddChild(new Paragraph(itemsStylesheet ?? stylesheet, string.Empty, true));
            _selectedValuePanel.Size.X.SetPercents(100f);
            _selectedValuePanel.Size.Y.SetPixels(GetClosedStateHeight());
            _selectedValuePanel.IgnoreScrollOffset = true;
            _selectedValuePanel._overrideInteractableState = true;
            var padding = GetPadding();
            _selectedValuePanel.Offset.X.Value = -padding.Left;
            _selectedValuePanel.Offset.Y.Value = -padding.Top;
            _selectedValuePanel.OverrideStyles.ExtraSize = new Sides() { Right = padding.Left + padding.Right };
            _selectedValuePanel.ExtraMarginForInteractions = new Sides(padding.Left, padding.Right, padding.Top, 0);
            AddChildInternal(_selectedValuePanel);

            // create dropdown icon
            if (arrowIconStylesheet != null)
            {
                _icon = new Control(arrowIconStylesheet);
                _icon.CopyStateFrom = this;
                _icon.IgnoreInteractions = true;
                _icon.IgnoreScrollOffset = true;
                AddChildInternal(_icon);
            }

            // clicking on selected value panel will open / close dropdown
            _selectedValuePanel.Events.OnClick += (Control Control) =>
            {
                ToggleList();
            };
        }

        /// <summary>
        /// Show / hide arrow icon.
        /// </summary>
        /// <param name="show">Should we show or hide the dropdown arrow icon.</param>
        public void ShowArrowIcon(bool show)
        {
            if (_icon != null)
            {
                _icon.Visible = show;
            }
        }
        /// <summary>
        /// Create the drop down with default stylesheets.
        /// </summary>
        public DropDown() : this(
            UISystem.DefaultStylesheets.DropDownPanels ?? UISystem.DefaultStylesheets.ListPanels ?? UISystem.DefaultStylesheets.Panels,
            UISystem.DefaultStylesheets.DropDownItems ?? UISystem.DefaultStylesheets.ListItems ?? UISystem.DefaultStylesheets.Paragraphs,
            UISystem.DefaultStylesheets.DropDownIcon)
        {
        }

        /// <inheritdoc/>
        protected override void SetAutoSizes(int maxWidth, int maxHeight)
        {
            if (AutoWidth)
            {
                Size.X.SetPixels(maxWidth);
            }
            if (AutoHeight)
            {
                Size.Y.SetPixels(ItemHeight * (ItemsCount + 3)); // +3 to compensate top panel
            }
        }
        /// <summary>
        /// Get the dropdown height, in pixels, when its closed.
        /// </summary>
        /// <returns>Drop down height in pixels when closed.</returns>
        public int GetClosedStateHeight(bool includePadding = true, bool includeExtraSize = true)
        {
            var padding = includePadding ? GetPadding() : Sides.Zero;
            var extra = includeExtraSize ? GetExtraSize() : Sides.Zero;
            return ItemHeight + padding.Top + padding.Bottom + extra.Top + extra.Bottom;
        }
        /// <summary>
        /// Set bounding rectangles size to its closed state.
        /// </summary>
        private void SetSizeToClosedState(ref Rectangle boundingRect, ref Rectangle internalBoundingRect)
        {
            internalBoundingRect.Height = GetClosedStateHeight();
            boundingRect.Height = internalBoundingRect.Height;
        }

        /// <inheritdoc/>
        protected override int GetExtraParagraphsCount()
        {
            return -2; // -2 to compensate top panel that shows selected value
        }

        /// <inheritdoc/>
        public override void SetVisibleItemsCount(int items)
        {
            AutoHeight = false;
            Size.Y.SetPixels(ItemHeight * (items + 2)); // +2 to compensate top panel that shows selected value
        }
        /// <inheritdoc/>
        protected override void DrawControlType(ref Rectangle boundingRect, ref Rectangle internalBoundingRect, DrawMethodResult parentDrawResult, DrawMethodResult? siblingDrawResult)
        {
            // special - if we are rendering in open mode, top most
            if (_isCurrentlyDrawingOpenedListTopMost)
            {
                base.DrawControlType(ref boundingRect, ref internalBoundingRect, parentDrawResult, siblingDrawResult);
                Rectangle rect = boundingRect;
                rect.Height = GetClosedStateHeight();
                DrawFillTextures(rect);
                return;
            }

            // closed? resize to close state BEFORE rendering
            if (!IsOpened)
            {
                SetSizeToClosedState(ref boundingRect, ref internalBoundingRect);
            }

            // dropdown is opened - render as list top-most
            if (IsOpened)
            {
                // move the scrollbar under the selected value box
                {
                    if (VerticalScrollbar != null)
                    {
                        var extra = GetExtraSize();
                        var scrollbarOffset = (int)((GetClosedStateHeight() - extra.Bottom) * 0.85f);
                        VerticalScrollbar.Offset.Y.Value = scrollbarOffset;
                        VerticalScrollbar.OverrideStyles.ExtraSize = new Sides(0, 0, 0, -scrollbarOffset);
                    }
                }
                DrawMethodResult parentResults = parentDrawResult;
                DrawMethodResult? siblingResults = siblingDrawResult;
                UISystem.RunAfterDrawingControls(() =>
                {
                    _isCurrentlyDrawingOpenedListTopMost = true;
                    _DoDraw(parentResults, siblingResults, false);
                    _isCurrentlyDrawingOpenedListTopMost = false;
                });
            }
            // dropdown is closed - render normally in close state
            else
            {
                base.DrawControlType(ref boundingRect, ref internalBoundingRect, parentDrawResult, siblingDrawResult);
            }

            // opened? resize to close state AFTER rendering
            // this way we render the entire open list, but without pushing down auto anchors below (ie the Control only takes the size of its close state when positioning).
            if (IsOpened)
            {
                SetSizeToClosedState(ref boundingRect, ref internalBoundingRect);
            }

        }
        bool _isCurrentlyDrawingOpenedListTopMost = false;

        /// <summary>
        /// Close the dropdown list.
        /// </summary>
        public void CloseList()
        {
            IsOpened = false;
        }

        /// <summary>
        /// Open the dropdown list.
        /// </summary>
        public void OpenList()
        {
            IsOpened = true;
        }

        /// <summary>
        /// Toggle dropdown list state.
        /// </summary>
        public void ToggleList()
        {
            if (IsOpened)
            {
                CloseList();
            }
            else
            {
                OpenList();
            }
        }
        /// <inheritdoc/>
        protected override void OnItemClicked(Control Control)
        {
            // if closed, open the list
            if (!IsOpened)
            {
                OpenList();
            }
            // if opened, call default action and close the list
            else
            {
                base.OnItemClicked(Control);
                CloseList();
            }
        }

        /// <inheritdoc/>
        protected override void SetParagraphs(int scrollOffset, int startIndex = 0)
        {
            if (IsOpened)
            {
                if (_paragraphs.Count > 0)
                {
                    _paragraphs[0].Offset.Y.Value = ItemHeight / 2;
                }
                base.SetParagraphs(scrollOffset, startIndex);
            }
            else
            {
                foreach (var p in _paragraphs)
                {
                    p.Visible = false;
                }
            }
        }
        /// <inheritdoc/>
        internal override void PostUpdate(InputState inputState)
        {
            // if opened and clicked outside, close list
            _selectedValueParagraph.Text = OverrideSelectedText ?? SelectedText ?? SelectedValue ?? DefaultSelectedText ?? string.Empty;
            _selectedValueParagraph.UseEmptyValueTextColor = (SelectedValue == null);
        
            // set icon state
            if (_icon != null)
            {
                _icon.LockedState = IsOpened ? ControlState.Interacted : null;
            }

            // if opened and click outside, close the list
            if (IsOpened && inputState.LeftMousePressedNow)
            {
                // clicked on closed state box? skip
                if (_selectedValuePanel.IsPointedOn(inputState.MousePosition))
                {
                    return;
                }

                // if got here and point outside the list, close.
                if (!IsPointedOn(inputState.MousePosition))
                {
                    CloseList();
                }
            }
        }
    }
}
