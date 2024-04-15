using System.Collections.Generic;

namespace MonoGo.Engine.EC
{
	internal class EntityDepthComparer : IComparer<Entity>
	{
		public int Compare(Entity x, Entity y) =>
			y.Depth - x.Depth;
	}
}
