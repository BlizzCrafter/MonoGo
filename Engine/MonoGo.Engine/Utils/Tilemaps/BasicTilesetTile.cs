using MonoGo.Engine.Drawing;

namespace MonoGo.Engine.Utils.Tilemaps
{
	public class BasicTilesetTile : ITilesetTile
	{
		public Frame Frame {get; private set;}

		public BasicTilesetTile(Frame frame) =>
			Frame = frame;
	}
}
