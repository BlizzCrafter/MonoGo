using Microsoft.Xna.Framework;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.EC;
using Monofoxe.Extended.Engine.SceneSystem;

namespace Monofoxe.Extended.Engine.Utils.Tilemaps
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
