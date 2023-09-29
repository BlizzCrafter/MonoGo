using Microsoft.Xna.Framework;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.EC;
using Monofoxe.Extended.Engine.Resources;
using Monofoxe.Extended.Engine.SceneSystem;
using Monofoxe.Extended.Engine.Utils;

namespace Monofoxe.Extended.Samples.Misc
{
	/// <summary>
	/// Basic position component. 
	/// </summary>
	public class Bot : Entity
	{
		public float TurningSpeed = 60;

		private readonly ActorComponent _actor;

		public Bot(Layer layer) : base(layer)
		{
			var botSprite = ResourceHub.GetResource<Sprite>("DefaultSprites", "Bot");

			AddComponent(new PositionComponent(Vector2.Zero));
			_actor = AddComponent(new ActorComponent(botSprite));

			// It is recommended to reuse random objects.
			TurningSpeed = GameController.Random.Next(120, 240);

		}


		public override void Update()
		{
			base.Update();
			_actor.Move = true;
			_actor.Direction += TimeKeeper.Global.Time(TurningSpeed); // ni-ni-ni-ni-ni-ni-ni-ni-ni-ni-ni-ni-ni-ni
		}
	}
}
