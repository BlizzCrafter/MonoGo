using Microsoft.Xna.Framework.Content;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using System.Collections.Generic;
using System.IO;

namespace MonoGo.Resources
{
	public class SpriteGroupResourceBox : ResourceBox<Sprite>
	{

		private ContentManager _content = new ContentManager(GameMgr.Game.Services);

		private readonly string _resourcePath;

		public SpriteGroupResourceBox(string name, string spriteGroupPath) : base(name)
		{
			_resourcePath = spriteGroupPath;
		}

		public override void Load()
		{
			if (Loaded)
			{
				return;
			}
			Loaded = true;
			var graphicsPath = Path.Combine(ResourceInfoMgr.ContentDir, _resourcePath);
			var sprites = _content.Load<Dictionary<string, Sprite>>(graphicsPath);

			foreach (var spritePair in sprites)
			{
				try
				{
					spritePair.Value.Name = Name + "/" + spritePair.Value.Name;
					AddResource(Path.GetFileNameWithoutExtension(spritePair.Key), spritePair.Value);
				}
				catch { }
			}

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
