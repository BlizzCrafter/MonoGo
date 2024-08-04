

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// Different states a control can be in.
    /// </summary>
    public enum ControlState
    {
        /// <summary>
        /// Default inactive state.
        /// </summary>
        Default,

        /// <summary>
        /// Control is currently being targeted. For example, the mouse points on the Control.
        /// </summary>
        Targeted,

        /// <summary>
        /// Control is currently being interacted with, for example text input Control we're currently typing into, or a button that is being pressed.
        /// </summary>
        Interacted,

        /// <summary>
        /// Control is checked. For checkbox, radio button, or other controls that have a 'checked' state.
        /// </summary>
        Checked,

        /// <summary>
        /// Control is currently being targeted, and also checked. For example, the mouse points on the Control.
        /// </summary>
        TargetedChecked,

        /// <summary>
        /// Control is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// Control is disabled but also checked.
        /// </summary>
        DisabledChecked,
    }
}
