using Microsoft.Xna.Framework;

namespace Monofoxe.Extended.Engine
{
	public static class RectangleExtensions
	{
		public static RectangleF ToRectangleF(this Rectangle rectagle) =>
			new RectangleF(rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
	}
}
