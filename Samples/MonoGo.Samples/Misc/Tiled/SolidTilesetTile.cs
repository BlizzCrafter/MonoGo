using MonoGo.Engine.Drawing;
using MonoGo.Engine.Utils.Tilemaps;

namespace MonoGo.Samples.Misc.Tiled
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
