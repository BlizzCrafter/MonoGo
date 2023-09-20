using Monofoxe.Extended.Engine.Drawing;

namespace Monofoxe.Extended.Engine.Utils.Tilemaps
{
	public class BasicTilesetTile : ITilesetTile
	{
		public Frame Frame {get; private set;}

		public BasicTilesetTile(Frame frame) =>
			Frame = frame;
	}
}
