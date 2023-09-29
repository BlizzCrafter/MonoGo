using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.Utils.Tilemaps;

namespace Monofoxe.Extended.Samples.Misc.Tiled
{

	public class SolidTilesetTile : ITilesetTile
	{
		public Frame Frame {get; private set;}
		public bool Solid;
			
		public SolidTilesetTile(Frame frame, bool solid)
		{
			Frame = frame;
			Solid = solid;
		}
			
	}
}
