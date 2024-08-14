namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// Define how to handle child controls that go out of the controls's bounds.
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>
        /// Child controls can overflow bounding rectangle freely.
        /// </summary>
        AllowOverflow,

        /// <summary>
        /// Child controls rendering will be cut off if they go out of bounding rectangle.
        /// </summary>
        HideOverflow,
    }
}
