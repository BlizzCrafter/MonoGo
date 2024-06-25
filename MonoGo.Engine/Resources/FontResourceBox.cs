using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using System.Diagnostics;
using System;
using System.IO;

namespace MonoGo.Engine.Resources
{
    public class FontResourceBox : ResourceBox<IFont>
    {
        private ContentManager _content;

        private readonly string _resourceDir;

        public FontResourceBox(string name, string resourceDir) : base(name)
        {
            _resourceDir = resourceDir;
        }

        public override void Load()
        {
            if (Loaded)
            {
                return;
            }
            Loaded = true;

            _content = new ContentManager(GameMgr.Game.Services);
            _content.RootDirectory = ResourceInfoMgr.ContentDir;

            var paths = ResourceInfoMgr.GetResourcePaths(_resourceDir + "/*");

            foreach (var path in paths)
            {
                try
                {
                    if (Path.HasExtension(path))
                    {
                        continue;
                    }
                    AddResource(Path.GetFileNameWithoutExtension(path), new Font(_content.Load<SpriteFont>(path)));
                }
                catch (InvalidCastException)
                {
                    Debug.WriteLine("Failed to load " + path + ". It has a different type.");
                }
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
