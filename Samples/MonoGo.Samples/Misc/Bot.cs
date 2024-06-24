using Microsoft.Xna.Framework;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;

namespace MonoGo.Samples.Misc
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
			var botSprite = ResourceHub.GetResource<Sprite>("DemoSprites", "Bot");

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
