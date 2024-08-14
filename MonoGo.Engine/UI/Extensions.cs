using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine.UI
{
    internal static class Extensions
    {
        internal static Rectangle MergeRectangles(Rectangle rect1, Rectangle rect2)
        {
            int x1 = Math.Max(rect1.X, rect2.X);
            int y1 = Math.Max(rect1.Y, rect2.Y);
            int x2 = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width);
            int y2 = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height);

            int width = x2 - x1;
            int height = y2 - y1;

            if (width > 0 && height > 0)
            {
                return new Rectangle(x1, y1, width, height);
            }
            else
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }
    }
}
