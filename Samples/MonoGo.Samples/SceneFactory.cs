using MonoGo.Engine.SceneSystem;
using System;

namespace MonoGo.Samples
{
	public class SceneFactory
	{
		public Scene Scene;
		public Type Type { get; private set; }

		public SceneFactory(Type type, string description = "")
		{
			Type = type;
			Description = description;
		}

		public readonly string Description;

		public void CreateScene()
		{
			Scene = SceneMgr.CreateScene(Type.Name);
			Scene.CreateLayer("default");
			Activator.CreateInstance(Type, Scene["default"]);
		}

		public void DestroyScene() =>
			SceneMgr.DestroyScene(Scene);

		public void RestartScene()
		{
			DestroyScene();
			CreateScene();
		}
	}
}
