using Microsoft.Xna.Framework;
using MonoGo.Tiled.MapStructure.Objects;

namespace MonoGo.Tiled.MapStructure
{
	public class TiledMapObjectLayer : TiledMapLayer
	{
		public TiledObject[] Objects;
		public TiledMapObjectDrawingOrder DrawingOrder;
		public Color Color;
	}
}
