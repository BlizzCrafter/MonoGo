using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Defs;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// An Control that doesn't do anything except for creating vertical space between entities that are set with auto anchor.
    /// </summary>
    public class RowsSpacer : Control
    {
        /// <summary>
        /// Create the spacer.
        /// </summary>
        /// <param name="rowsCount">How many empty 'rows' to to create. The height of each row is defined by the UI system stylesheet.</param>
        public RowsSpacer(int rowsCount = 1) : base(null)
        {
            IgnoreInteractions = true;
            Size.Y.SetPixels(UISystem.SystemStyleSheet.RowSpaceHeight * rowsCount);
        }

        /// <inheritdoc/>
        protected override Anchor GetDefaultEntityTypeAnchor()
        {
            return Anchor.AutoCenter;
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultEntityTypeSize()
        {
            var ret = new MeasureVector();
            ret.X.SetPercents(100f);
            ret.Y.SetPixels(8);
            return ret;
        }

        /// <inheritdoc/>
        internal override void DebugDraw(bool debugDrawChildren)
        {
            if (!Visible) { return; }

            Renderer.DrawRectangle(LastBoundingRect, new Color(255, 0, 0, 65));

            if (debugDrawChildren)
            {
                IterateChildren((Control child) => { 
                    child.DebugDraw(debugDrawChildren); 
                    return true; 
                });
            }
        }
    }
}
