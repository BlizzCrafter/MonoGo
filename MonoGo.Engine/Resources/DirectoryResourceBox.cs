﻿using Microsoft.Xna.Framework.Content;
using MonoGo.Engine;
using MonoGo.Engine.Resources;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MonoGo.Resources
{
	/// <summary>
	/// Loads all content from a specified directory.
	/// NOTE: All content files in the directory should be
	/// of the same type!!!
	/// </summary>
	public class DirectoryResourceBox<T> : ResourceBox<T>
	{
		private static ContentManager _content;

		private readonly string _resourceDir;

		public DirectoryResourceBox(string name, string resourceDir) : base(name)
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

			// TODO: Add Recursive flag to GetResourcePaths.
			var paths = ResourceInfoMgr.GetResourcePaths(_resourceDir + "/*");

			foreach (var path in paths)
			{
				try
				{
					if (Path.HasExtension(path))
					{
						continue;
					}
					AddResource(Path.GetFileNameWithoutExtension(path), _content.Load<T>(path));
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
			_content.Unload();
		}

	}
}
