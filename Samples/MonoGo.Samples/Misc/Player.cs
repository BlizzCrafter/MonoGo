using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Samples.Misc
{
	public class Player : Entity
	{
		public const Buttons UpButton = Buttons.W;
		public const Buttons DownButton = Buttons.S;
		public const Buttons LeftButton = Buttons.A;
		public const Buttons RightButton = Buttons.D;

        public bool AttractParticles { get; set; } = true;
        public bool InsideParticles { get; set; } = true;

        private Sprite _playerSprite;

        // The player uses hybrid EC - it's a derived entity with components inside.
        // You also can ditch components entirely and only use entities. 

        // I recommend using hybrid entities in places,
        // where you know that this entity's code will not be reused anywhere else.

        private ActorComponent _actor;
        private PositionComponent _position;

        public Player(Layer layer, Vector2 position) : base(layer)
		{
			_playerSprite = ResourceHub.GetResource<Sprite>("DemoSprites", "Player");
			
			// You can add components right in the constructor.
			_position = AddComponent(new PositionComponent(position));
			_actor = AddComponent(new ActorComponent(_playerSprite));
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void Update()
		{
			base.Update();

			// Very basic controls.
			var movement = new Vector2(
				Input.CheckButton(RightButton).ToInt() - Input.CheckButton(LeftButton).ToInt(),
				Input.CheckButton(DownButton).ToInt() - Input.CheckButton(UpButton).ToInt()
			);

			// Telling our actor component to move in a specific direction.	
			_actor.Move = movement != Vector2.Zero;
			_actor.Direction = movement.ToAngle();
		}

		public override void Draw()
		{
			base.Draw();

			// Layers and scenes have methods for searching entities/components.
			foreach(var bot in Layer.GetEntityList<Bot>())
			{
				var botPosition = bot.GetComponent<PositionComponent>();

				LineShape.Draw(_position.Position, botPosition.Position, Color.Transparent, Color.White * 0.2f);
			}			
		}
	}
}
