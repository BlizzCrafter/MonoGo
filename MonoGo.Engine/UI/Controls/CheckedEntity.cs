using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Control type that have 'Checked' property that can be checked / unchecked.
    /// </summary>
    public abstract class CheckedControl : Control
    {
        /// <summary>
        /// Set / get if this entity is in 'checked' state.
        /// Useful for buttons anc checkboxes, but can be set for any type of entity.
        /// </summary>
        public virtual bool Checked
        {
            get => _isChecked;
            set
            {
                // change checked state
                if (value != _isChecked)
                {
                    // uncheck siblings
                    if (ExclusiveSelection)
                    {
                        Parent?.IterateChildren((Control entity) =>
                        {
                            var siblingAsCheckable = entity as CheckedControl;
                            if ((siblingAsCheckable != null) && (siblingAsCheckable != this) && (siblingAsCheckable.ExclusiveSelection))
                            {
                                siblingAsCheckable.Checked = false;
                            }
                            return true;
                        });
                    }

                    // set value
                    _isChecked = value;

                    // invoke value change callbacks
                    Events.OnValueChanged?.Invoke(this);
                    UISystem.Events.OnValueChanged?.Invoke(this);

                    // invoke checked / unchecked callbacks
                    if (_isChecked)
                    {
                        Events.OnChecked?.Invoke(this);
                        UISystem.Events.OnChecked?.Invoke(this);
                    }
                    else
                    {
                        Events.OnUnchecked?.Invoke(this);
                        UISystem.Events.OnUnchecked?.Invoke(this);
                    }
                }
            }
        }

        /// <summary>
        /// If true, this entity will check / uncheck itself when clicked on.
        /// </summary>
        public bool ToggleCheckOnClick = false;

        /// <summary>
        /// If true, when this entity is checked, all its direct siblings with this property will be automatically unchecked.
        /// This is used for things like radio button where only one option can be checked at any given moment.
        /// </summary>
        public bool ExclusiveSelection = false;

        /// <summary>
        /// If false, it will be impossible to uncheck this entity once its checked by clicking on it.
        /// However, if the 'ExclusiveSelection' is set, you can still uncheck it by checking another sibling.
        /// </summary>
        public bool CanClickToUncheck = true;

        /// <inheritdoc/>
        public CheckedControl(StyleSheet? stylesheet) : base(stylesheet)
        {
        }

        /// <inheritdoc/>
        internal override void DoInteractions(InputState inputState)
        {
            // call base class to trigger events
            base.DoInteractions(inputState);

            // check / uncheck
            if (ToggleCheckOnClick)
            {
                if (inputState.LeftMouseReleasedNow)
                {
                    // disable unchecking
                    if (!CanClickToUncheck && Checked)
                    {
                        return;
                    }

                    // toggle checked mode
                    Checked = !Checked;
                }
            }
        }
    }
}
