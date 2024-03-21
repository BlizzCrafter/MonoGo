﻿using Microsoft.Xna.Framework.Graphics;

namespace MonoGo.Engine.Drawing
{
	/// <summary>
	/// Line primitive, which can accept any array of indices. 
	/// </summary>
	public class CustomLinePrimitive : Primitive2D
	{
		protected override PrimitiveType _primitiveType => PrimitiveType.LineList;
		
		/// <summary>
		/// Array of primitive indices. They tell primitive what vertices to draw in what order.
		/// </summary>
		public short[] Indices;

		public CustomLinePrimitive(int capacity, short[] indices = null) : base(capacity)
		{
			Indices = indices;
		}

		/// <summary>
		/// Sets user-defined list of indices for a list of lines.
		/// </summary>
		protected override short[] GetIndices() =>
			Indices;
	}
}
