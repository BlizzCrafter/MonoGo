using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;

namespace MonoGo.Samples.Misc
{
	/// <summary>
	/// Basic position component. 
	/// </summary>
	public class PositionComponent : Component, IMovable
	{
		/// <summary>
		/// Entity position on the scene.
		/// </summary>
		public Vector2 Position 
		{
			get { return _position; }
			set
			{
				PreviousPosition = _position;
				_position = value;
			}
		}
		private Vector2 _position;

		/// <summary>
		/// Starting entity position on the scene.
		/// </summary>
		public Vector2 StartingPosition { get; set; }

		/// <summary>
		/// Previous position of the entity.
		/// </summary>
		public Vector2 PreviousPosition { get; private set; }

		public PositionComponent(Vector2 position)
		{
			Position = position;
			PreviousPosition = position;
			StartingPosition = position;
        }
    }
}
