﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGo.Tiled.MapStructure
{
	public class TiledMapImageLayer : TiledMapLayer
	{
		public string TexturePath;
		public Texture2D Texture;

		/// <summary>
		/// Tiled will treat this color as transparent.
		/// Ah, blast right from 1998.
		/// </summary>
		public Color TransparentColor;
	}
}
