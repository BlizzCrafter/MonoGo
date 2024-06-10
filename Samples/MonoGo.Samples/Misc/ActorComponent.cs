using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Utils;

namespace MonoGo.Samples.Misc
{
	/// <summary>
	/// Actor component. Actors can move in some direction.
	/// NOTE: PositionComponent is required.
	/// </summary>
	public class ActorComponent : Component
	{
		public bool Move = false;

		public Angle Direction;
		public float Speed = 150;

		public Sprite Sprite;

		public ActorComponent(Sprite sprite)
		{
			Visible = true; // Components are not visible by default.

			Sprite = sprite;
		}

		public override void Update()
		{
			base.Update();
			if (Move)
			{
				// Retrieving the position component from entity.
				var position = Owner.GetComponent<PositionComponent>();
				position.Position += TimeKeeper.Global.Time(Speed) * Direction.ToVector2();
			}
		}

		public override void Draw()
		{
			base.Draw();
			var position = Owner.GetComponent<PositionComponent>();

			GraphicsMgr.CurrentColor = Color.White;

			Sprite.Draw(position.Position);
		}

	}
}
