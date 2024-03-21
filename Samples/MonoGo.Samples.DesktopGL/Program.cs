using MonoGo.Engine.DesktopGL;
using System;

namespace MonoGo.Samples.DesktopGL
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
            MonoGoPlatform.Init();
			using (var game = new Game1())
			{
				game.Run();
			}
		}
	}
}
