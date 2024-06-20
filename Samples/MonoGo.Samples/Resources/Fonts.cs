using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;

namespace MonoGo.Samples.Resources
{
	public class Fonts : ResourceBox<IFont>
	{
		private ContentManager _content;

		public Fonts() : base("Fonts")
		{
			_content = new ContentManager(GameMgr.Game.Services);
			_content.RootDirectory = ResourceInfoMgr.ContentDir + "/Fonts";
		}

		public override void Load()
		{
			if (Loaded)
			{
				return;
			}
			Loaded = true;

            AddResource("Arial", new Font(_content.Load<SpriteFont>("Arial")));
            AddResource("Regular", new Font(_content.Load<SpriteFont>("Regular")));
            AddResource("Italic", new Font(_content.Load<SpriteFont>("Italic")));
            AddResource("Bold", new Font(_content.Load<SpriteFont>("Bold")));

            var fontSprite = ResourceHub.GetResource<Sprite>("DemoSprites", "Font");

			AddResource("FancyFont", new TextureFont(fontSprite, 1, 1, TextureFont.Ascii, false));
		}

		public override void Unload()
		{
			if (!Loaded)
			{
				return;
			}
			Loaded = false;
			_content.Unload();
		}
	}
}
