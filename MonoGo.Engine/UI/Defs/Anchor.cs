

namespace MonoGo.Engine.UI.Defs
{
    /// <summary>
    /// Positions in parent control to position the control from.
    /// The anchor also affect the UI control's offset.
    /// </summary>
    public enum Anchor
    {
        /// <summary>
        /// Auto placement with one control per row.
        /// Going left-to-right.
        /// </summary>
        AutoLTR,

        /// <summary>
        /// Auto placement in the same row, until exceeding parent width.
        /// Going left-to-right.
        /// </summary>
        AutoInlineLTR,

        /// <summary>
        /// Auto placement with one control per row.
        /// Going right-to-left.
        /// </summary>
        AutoRTL,

        /// <summary>
        /// Auto placement in the same row, until exceeding parent width.
        /// Going right-to-left.
        /// </summary>
        AutoInlineRTL,

        /// <summary>
        /// Auto placement with one control per row.
        /// Aligned to center.
        /// </summary>
        AutoCenter,

        /// <summary>
        /// Control is aligned to parent top-left internal corner.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Control is aligned to parent top-center internal point.
        /// </summary>
        TopCenter,

        /// <summary>
        /// Control is aligned to parent top-right internal corner.
        /// </summary>
        TopRight,

        /// <summary>
        /// Control is aligned to parent bottom-left internal corner.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Control is aligned to parent bottom-center internal point.
        /// </summary>
        BottomCenter,

        /// <summary>
        /// Control is aligned to parent bottom-right internal corner.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Control is aligned to parent center-left internal point.
        /// </summary>
        CenterLeft,

        /// <summary>
        /// Control is aligned to parent center internal point.
        /// </summary>
        Center,

        /// <summary>
        /// Control is aligned to parent center-right internal point.
        /// </summary>
        CenterRight,
    }
}
