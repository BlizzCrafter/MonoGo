﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Monofoxe.Engine
{
	/// <summary>
	/// Provides convenient resource paths and lists all content assets.
	/// </summary>
	public static class ResourceInfoMgr
	{
		/// <summary>
		/// Root directory of the game content.
		/// </summary>
		public static string ContentDir = "Content";


		/// <summary>
		/// List of all game ssets.
		/// </summary>
		private static string[] _assetPaths;



		internal static void Init()
		{
			try
			{
				_assetPaths = GameMgr.Game.Content.Load<string[]>("Content");
			}
			catch (Exception) { }
		}


		/// <summary>
		/// Returns list of resource paths matching the given wildcard pattern. * and ? are supported.
		/// </summary>
		public static string[] GetResourcePaths(string pattern)
		{
			if (_assetPaths == null)
			{
				return null;
			}

			if (pattern == "")
			{
				var assetPathsCopy = new string[_assetPaths.Length];
				_assetPaths.CopyTo(assetPathsCopy, 0);
				return assetPathsCopy;
			}

			var pattenRegex = WildCardToRegular(pattern.Replace("\\", "/"));

			var list = new List<string>();
			foreach (var info in _assetPaths)
			{
				if (Regex.IsMatch(info, pattenRegex))
				{
					list.Add(info);
				}
			}

			return list.ToArray();
		}

		private static string WildCardToRegular(string value) =>
			"^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
	}
}
