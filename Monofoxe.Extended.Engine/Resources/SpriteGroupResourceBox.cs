using Microsoft.Xna.Framework.Content;
using Monofoxe.Extended.Engine;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.Resources;
using System.Collections.Generic;
using System.IO;

namespace Monofoxe.Extended.Resources
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
