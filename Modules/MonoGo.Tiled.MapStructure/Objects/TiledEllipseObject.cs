﻿using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
	public class TiledEllipseObject : TiledObject
	{
		public Vector2 Center => Position + Size / 2f;

		public TiledEllipseObject() {}
		public TiledEllipseObject(TiledObject obj) : base(obj) {}
	}
}
