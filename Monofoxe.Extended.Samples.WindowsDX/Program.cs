using System;
using Monofoxe.Extended.Engine.WindowsDX;

namespace Monofoxe.Extended.Samples.WindowsDX
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			MonofoxePlatform.Init();

			using (var game = new Game1())
			{
				game.Run();
			}
		}
	}
}
