using Microsoft.Xna.Framework;
using Monofoxe.Extended.Tiled.MapStructure.Objects;

namespace Monofoxe.Extended.Tiled.MapStructure
{
	public class TiledMapObjectLayer : TiledMapLayer
	{
		public TiledObject[] Objects;
		public TiledMapObjectDrawingOrder DrawingOrder;
		public Color Color;
	}
}
