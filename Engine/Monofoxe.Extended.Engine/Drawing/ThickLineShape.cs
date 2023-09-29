﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Extended.Engine.Utils;

namespace Monofoxe.Extended.Engine.Drawing
{
	/// <summary>
	/// Drawable thick line shape. Can be drawn by using static methods or be instantiated.
	/// </summary>
	public class ThickLineShape : Drawable
	{
		/// <summary>
		/// First line point. 
		/// NOTE: all line points treat position as an origin point;
		/// </summary>
		public Vector2 Point1;

		/// <summary>
		/// Second line point. 
		/// NOTE: all line points treat position as an origin point;
		/// </summary>
		public Vector2 Point2;

		/// <summary>
		/// Line thickness.
		/// </summary>
		public float Thickness = 1;
		
		public Color Color = Color.White;

		public float ZDepth = 0;

		public override void Draw() =>
			Draw(Point1 + Position, Point2 + Position, Thickness, Color, Color, ZDepth);


		private static VertexPositionColorTexture[] _thickLineVertices = new VertexPositionColorTexture[4];

		private static readonly short[] _thickLineIndices = new short[]{0, 1, 3, 1, 2, 3};
		
		/// <summary>
		/// Draws a line with specified width.
		/// </summary>
		public static void Draw(Vector2 pt1, Vector2 pt2, float thickness) =>
			Draw(pt1, pt2, thickness, GraphicsMgr.CurrentColor, GraphicsMgr.CurrentColor);

		/// <summary>
		/// Draws a line with specified width and colors.
		/// </summary>
		public static void Draw(Vector2 pt1, Vector2 pt2, float thickness, Color c1, Color c2, float zDepth = 0)
		{
			var normal = (pt2 - pt1).Rotate90();

			normal = normal.GetSafeNormalize(); // The result is a unit vector rotated by 90 degrees.
			normal *= thickness / 2;
			
			_thickLineVertices[0].Position = new Vector3(pt1 - normal, zDepth);
			_thickLineVertices[0].Color = c1;
			_thickLineVertices[1].Position = new Vector3(pt1 + normal, zDepth);
			_thickLineVertices[1].Color = c1;
			_thickLineVertices[2].Position = new Vector3(pt2 + normal, zDepth);
			_thickLineVertices[2].Color = c2;
			_thickLineVertices[3].Position = new Vector3(pt2 - normal, zDepth);
			_thickLineVertices[3].Color = c2;


			// Thick line is in fact just a rotated rectangle.

			GraphicsMgr.VertexBatch.Texture = null;
			GraphicsMgr.VertexBatch.AddPrimitive(PrimitiveType.TriangleList, _thickLineVertices, _thickLineIndices);
		}
		
	}
}
