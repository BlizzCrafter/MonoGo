﻿using Microsoft.Xna.Framework;
using Monofoxe.Extended.Engine;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.EC;
using Monofoxe.Extended.Engine.SceneSystem;
using Monofoxe.Extended.Engine.Utils;
using System.Diagnostics;

namespace Monofoxe.Extended.Samples.Demos
{
	public class ShapeDemo : Entity
	{
		
		Color _mainColor = Color.White;
		HsvColor _secondaryColor = new HsvColor(new Color(34, 65, 250));
		TriangleShape _triangle; 

		public ShapeDemo(Layer layer) : base(layer)
		{
            GameMgr.Game.IsMouseVisible = true;

            // You can instantiate shapes instead of using static methods.
            _triangle = new TriangleShape();
			_triangle.Point1 = new Vector2(32, 32);
			_triangle.Point2 = new Vector2(-32, 32);
			_triangle.Point3 = new Vector2(-32, -32);
			_triangle.IsOutline = true;
		}

		public override void Draw()
		{
			base.Draw();

			_secondaryColor.H += TimeKeeper.Global.Time(360 / 4f);
			if (_secondaryColor.H >= 360f)
			{ 
				_secondaryColor.H -= 360;
			}
			Debug.WriteLine(_secondaryColor.H);

			// This code shows how to draw shapes using static methods and instanced objects.
			
			var startingPosition = new Vector2(100, 100);
			var position = startingPosition;
			var spacing = 100;

			// Circles.
			GraphicsMgr.CurrentColor = _mainColor; // Setting current color. It's active for all shapes and sprites.
			CircleShape.Draw(position, 24, false); // Filled circle.

			GraphicsMgr.CurrentColor = _secondaryColor.ToColor();
			CircleShape.Draw(position, 32, true); // Outline.


			position += Vector2.UnitX * spacing;


			CircleShape.CircleVerticesCount = 8; // Changing the amount of circle vertices.

			GraphicsMgr.CurrentColor = _mainColor; 
			CircleShape.Draw(position, 24, false); 

			GraphicsMgr.CurrentColor = _secondaryColor.ToColor();
			CircleShape.Draw(position, 32, true);

			CircleShape.CircleVerticesCount = 32;
			// Circles.


			position += Vector2.UnitY * spacing;
			position.X = startingPosition.X;


			// Rectangles.
			
			// You can draw rectangle using its top left and bottom right point...
			RectangleShape.Draw(position - Vector2.One * 24, position + Vector2.One * 24, false);

			GraphicsMgr.CurrentColor = _mainColor;
			// ...or its center position and size!
			RectangleShape.DrawBySize(position, Vector2.One * 64, true);

			position += Vector2.UnitX * spacing;

			RectangleShape.Draw( // We can also manually set colors for each vertex.
				position - Vector2.One * 24, 
				position + Vector2.One * 24, 
				false, 
				_mainColor, 
				_mainColor, 
				_mainColor, 
				_secondaryColor.ToColor()
			);

			RectangleShape.DrawBySize(
				position, 
				Vector2.One * 64, 
				true,
				_mainColor,
				_secondaryColor.ToColor(),
				_mainColor,
				_mainColor
			);
			// Rectangles.


			position += Vector2.UnitY * spacing;
			position.X = startingPosition.X;


			// Triangles.

			_triangle.Position = position;
			_triangle.Draw(); // Drawing an instantiated triangle.

			GraphicsMgr.CurrentColor = _mainColor;

			TriangleShape.Draw(
				position + new Vector2(-24, -24), 
				position + new Vector2(24, -24), 
				position + new Vector2(24, 24), 
				false
			);
			
			// Be aware of culling. This triangle, for example, will be culled.
			// You can disable culling, if you don't want to deal with it.
			TriangleShape.Draw(new Vector2(-24, -24), new Vector2(24, 24), new Vector2(24, -24), false);

			// Triangles.



			// Lines.

			position += Vector2.UnitX * spacing;
			LineShape.Draw(position - Vector2.One * 24, position + Vector2.One * 24);

			position += Vector2.UnitX * spacing / 2f;
			ThickLineShape.Draw(position - Vector2.One * 24, position + Vector2.One * 24, 5);

			// Lines.


		}

	}
}
