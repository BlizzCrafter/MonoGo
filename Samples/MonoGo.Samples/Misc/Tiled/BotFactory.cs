using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Tiled;
using MonoGo.Tiled.MapStructure.Objects;
using System.Globalization;

namespace MonoGo.Samples.Misc.Tiled
{
	public class BotFactory : ITiledEntityFactory
	{
		public string Tag => "Bot";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			var entity = new Bot(layer);
			var position = entity.GetComponent<PositionComponent>();
			position.StartingPosition = tile.Position;
			position.Position = tile.Position;
			
			var actor = entity.GetComponent<ActorComponent>();

			// Retrieving a custom parameter.
			actor.Speed = float.Parse(tile.Properties["Speed"], CultureInfo.InvariantCulture);

			return entity;
		}
	}
}
