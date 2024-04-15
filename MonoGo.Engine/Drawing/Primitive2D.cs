﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGo.Engine.Drawing
{
	/// <summary>
	/// Base 2D primitive class. Can be used to create other types of primitives.
	/// </summary>
	public abstract class Primitive2D
	{
		public Vector2 Position;

		/// <summary>
		/// List of all primitive's vertices. 
		/// NOTE: all vertices treat position as an origin point;
		/// </summary>
		public Vertex[] Vertices;
		
		/// <summary>
		/// Graphics mode which will be used while drawing primitive.
		/// </summary>
		protected abstract PrimitiveType _primitiveType {get;}
		
		/// <summary>
		/// Frame texture.
		/// NOTE: Frame and be only a small part of a big texture.
		/// </summary>
		protected Texture2D _texture;
		/// <summary>
		/// Offset of a texture region.
		/// </summary>
		protected Vector2 _textureOffset;
		/// <summary>
		/// Ratio between texture size and frame size.
		/// </summary>
		protected Vector2 _textureRatio;


		public Primitive2D(int capacity)
		{
			Vertices = new Vertex[capacity];
		}


		/// <summary>
		/// Sets texture for a primitive.
		/// </summary>
		public void SetTexture(Texture2D texture)
		{
			_texture = texture;
			_textureOffset = Vector2.Zero;
			_textureRatio = Vector2.One;
		}

		/// <summary>
		/// Sets texture for a primitive.
		/// </summary>
		public void SetTextureFromFrame(Frame frame)
		{
			if (frame != null)
			{
				_texture = frame.Texture;
				
				_textureOffset = new Vector2(
					frame.TexturePosition.X / (float)frame.Texture.Width, 
					frame.TexturePosition.Y / (float)frame.Texture.Height
				);
			
				_textureRatio = new Vector2(
					frame.TexturePosition.Width / (float)frame.Texture.Width, 
					frame.TexturePosition.Height / (float)frame.Texture.Height
				);
			}
			else
			{
				SetTexture(null);
			}
		}
		

		/// <summary>
		/// Returns an array of vertex indices which essentially determine how primitive will be drawn.
		/// </summary>
		protected abstract short[] GetIndices();

		/// <summary>
		/// Converts list of MonoGo Vertex objects to Monogame's vertices.
		/// </summary>
		protected VertexPositionColorTexture[] GetConvertedVertices()
		{
			var convertedVertices = new VertexPositionColorTexture[Vertices.Length];
			for(var i = 0; i < Vertices.Length; i += 1)
			{
				convertedVertices[i].Position = Vertices[i].Position + Position.ToVector3();
				convertedVertices[i].Color = Vertices[i].Color;

				// Since we may work with sprites, which are only little parts of whole texture atlas,
				// we need to convert local sprite coordinates to global atlas coordinates.
				convertedVertices[i].TextureCoordinate = _textureOffset + Vertices[i].TexturePosition * _textureRatio;
			}

			return convertedVertices;
		}

		
		public void Draw()
		{
			GraphicsMgr.VertexBatch.Texture = _texture;
			GraphicsMgr.VertexBatch.AddPrimitive(_primitiveType, GetConvertedVertices(), GetIndices());
		}

		
	}
}
