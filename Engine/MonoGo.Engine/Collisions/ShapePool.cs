using MonoGo.Engine.Collisions.Shapes;
using MonoGo.Engine.Utils.CustomCollections;

namespace MonoGo.Engine.Collisions
{
	public static class ShapePool
	{
		private static Pool<Circle> _circlePool = new Pool<Circle>();
		private static Pool<Polygon> _polygonPool = new Pool<Polygon>();


		public static Circle GetCircle() =>
			_circlePool.Get();

		
		public static Polygon GetPolygon() =>
			_polygonPool.Get();

		
		public static void Return(IShape shape)
		{
			if (shape.Type == ShapeType.Circle)
			{ 
				_circlePool.Return((Circle)shape);
			}
			else
			{
				_polygonPool.Return((Polygon)shape);
			}
		}
	}
}
