using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using System.IO;

namespace MonoGo.Engine.Resources
{
    public class FontResourceBox : ResourceBox<IFont>
    {
        private ContentManager _content = new(GameMgr.Game.Services);

        public FontResourceBox(string name, string fontPath) : base(name)
        {
            _content.RootDirectory = Path.Combine(ResourceInfoMgr.ContentDir, fontPath);
        }

        public override void Load()
        {
            if (Loaded)
            {
                return;
            }
            Loaded = true;

            string[] fonts = Directory.GetFiles(_content.RootDirectory);
            foreach (string font in fonts)
            {
                var name = Path.GetFileNameWithoutExtension(font);
                try
                {
                    var spriteFont = _content.Load<SpriteFont>(name);
                    AddResource(name, new Font(spriteFont));
                }
                catch { continue; }
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
