using System;
using MonoGo.Engine.WindowsDX;

namespace MonoGo.Samples.WindowsDX
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
