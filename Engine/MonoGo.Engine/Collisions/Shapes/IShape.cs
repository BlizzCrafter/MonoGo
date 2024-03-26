using MonoGo.Engine.Utils;

namespace MonoGo.Engine.Collisions.Shapes
{
	/// <summary>
	/// Defines a concave shape for the collision system to use.
	/// </summary>
	public interface IShape
	{
		ShapeType Type { get; }

		AABB GetBoundingBox();
	}
}
