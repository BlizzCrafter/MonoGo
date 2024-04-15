﻿using MonoGo.Engine.Utils.CustomCollections;
using System.Collections.Generic;

namespace MonoGo.Engine.Cameras
{
	/// <summary>
	/// Manages camera priorities.
	/// </summary>
	public static class CameraMgr
	{
		/// <summary>
		/// List of all cameras.
		/// </summary>
		public static List<Camera> Cameras => _cameras.ToList();

		private static SafeSortedList<Camera> _cameras = new SafeSortedList<Camera>(x => x.Priority);

		/// <summary>
		/// Removes camera from list and adds it again, taking in account its proirity.
		/// </summary>
		internal static void UpdateCameraPriority(Camera camera)
		{
			_cameras.Remove(camera);
			_cameras.Add(camera);
		}

		internal static void RemoveCamera(Camera camera) =>
			_cameras.Remove(camera);

	}
}
