using Microsoft.Xna.Framework;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Engine.Utils.Tilemaps
{
	/// <summary>
	/// Component for Tiled image layers.
	/// </summary>
	public class ImageLayerRenderer : Entity
	{
		public Frame Frame;

		public Vector2 Offset;

		public ImageLayerRenderer(Layer layer, Vector2 offset, Frame frame) : base(layer)
		{
			Visible = true;
			Frame = frame;
			Offset = offset;
		}

		public override void Draw()
		{
			Frame.Draw(Offset, Vector2.Zero);
		}

	}
}
