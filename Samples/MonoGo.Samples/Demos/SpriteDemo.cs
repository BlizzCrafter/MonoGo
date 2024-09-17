﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.Utils;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using System;

namespace MonoGo.Samples.Demos
{
	public class SpriteDemo : Entity
	{
		Color _mainColor = Color.White;
		Color _secondaryColor = Color.Violet;

		double _animation = 0;
		double _animationSpeed = 0.25;

		SpriteBatch _batch;
		Surface _surface;
		Effect _seizure;

        Sprite _test;
		Sprite _fireSprite;

		public SpriteDemo(Layer layer) : base(layer)
		{
            _test = ResourceHub.GetResource<Sprite>("DemoSprites", "Test");
			_fireSprite = ResourceHub.GetResource<Sprite>("DemoSprites", "Fire");

			_batch = new SpriteBatch(GraphicsMgr.Device);
			_seizure = ResourceHub.GetResource<Effect>("Effects", "Seizure");
			InitSurface();
		}

		public override void Update()
		{
			base.Update();

			// Basic animation code.
			_animation += TimeKeeper.Global.Time(_animationSpeed);
			if (_animation > 1)
			{
				_animation -= 1;
			}
		}

		public override void Draw()
		{
			base.Draw();

			var startingPosition = new Vector2(100, 100);
			var position = startingPosition;
			var spacing = 100;

			
			// Sprites can't have static methods.
			_test.Draw(position);

			position += Vector2.UnitX * spacing * 2;

			// Setting a shader for the sprite.
			GraphicsMgr.VertexBatch.Effect = _seizure;

			// If you want to animate the sprite, you must pass a value from 0 to 1 to it.
			_fireSprite.Draw(position, _animation);
			GraphicsMgr.VertexBatch.Effect = null;

			position += Vector2.UnitX * spacing;

			// You can also access sprite's frame array, if you want to draw a specific frame.
			_fireSprite[2].Draw(position, _fireSprite.Origin);

			position += Vector2.UnitX * spacing;

			// You can scale, rotate srites and set custom origin point.

			_fireSprite.Draw(
				position,
				0.4f, 
				_fireSprite.Size / 2, 
				new Vector2(1, 2) * (float)Math.Sin(_animation * Math.PI * 2 * 2), 
				new Angle(360 * _animation), 
				Color.Red
			);

			position += Vector2.UnitX * spacing;

            // You also can draw only a part of the sprite.
            _test.Draw(
                new RectangleF(position.X, position.Y, 64, 64),
                0,
                new RectangleF(64, 64, 64, 64),
                Angle.Right,
                Color.White
            );

            position += Vector2.UnitY * spacing * 1.5f;
			position.X = 0;

			// You can extract raw texture from the frames. Note that you will get the whole texture atlas.
			var texture = _test[0].Texture;
			var texturePosition = _test[0].TexturePosition; // This will give you texture's position on the atlas.

			// We can also use default Monogame's SpriteBatch (or anything, for that matter).

			// But beforehand we must flush MonoGo's own batcher.
			// This method draws all batched graphics. 
			GraphicsMgr.VertexBatch.FlushBatch();

			// After that, you can draw anything you like using any method.

			_batch.Begin(
				SpriteSortMode.Deferred,
				null, 
				SamplerState.PointWrap, 
				null, 
				null, 
				null,
				GraphicsMgr.VertexBatch.View 
			);
			_batch.Draw(texture, position, GraphicsMgr.CurrentColor);

			_batch.End();
			
			// After you're done, you can draw anything you like without switching graphics mode again.
			RectangleShape.Draw(position, position + new Vector2(texture.Width, texture.Height), ShapeFill.Outline);

			position += Vector2.UnitX * 512;

			GraphicsMgr.CurrentColor = Color.Red;
			Surface.SetTarget(_surface);

			var po = _surface.Size / 2 + new Angle(GameMgr.ElapsedTimeTotal * 10).ToVector2() * 64;
			RectangleShape.DrawBySize(po, Vector2.One * 8, ShapeFill.Solid);

			Surface.ResetTarget();

			_surface.Draw(position);

			position += new Vector2(16, 150);

			GraphicsMgr.CurrentColor = Color.White;
			Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Default");
			Text.Draw("This text is drawn using default" + Environment.NewLine + "MonoGame spritefont.", position);
			position += Vector2.UnitY * 48;
			Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "FancyFont");
			Text.Draw("This text is drawn using custom" + Environment.NewLine + "font made from a sprite.", position, Vector2.One * 1.1f, Vector2.Zero, new Angle(-10));
		}

		/// <summary>
		/// Creates a new surface and draws test stuff on it.
		/// </summary>
		void InitSurface()
		{
			_surface = new Surface(new Vector2(128, 128));

			Surface.SetTarget(_surface);

			GraphicsMgr.Device.Clear(_secondaryColor);

			GraphicsMgr.CurrentColor = _mainColor;
			CircleShape.Draw(new Vector2(64, 64), 64, ShapeFill.Solid);

			Surface.ResetTarget();
		}

		public override void Destroy()
		{
			_surface.Dispose(); // Don't forget to dispose surfaces.
		}
	}
}
