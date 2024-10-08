﻿using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Tiled.MapStructure.Objects;

namespace MonoGo.Tiled
{
	/// <summary>
	/// Factory interface for entities. 
	/// Use it to convert Tiled object structs to actual entities.
	/// </summary>
	public interface ITiledEntityFactory
	{
		/// <summary>
		/// Identifying tag.
		/// 
		/// NOTE: All factory tags should be unique!
		/// </summary>
		string Tag {get;}

		/// <summary>
		/// Creates entity from Tiled Object on a given layer.
		/// </summary>
		Entity Make(TiledObject obj, Layer layer, MapBuilder map);
	}
}
