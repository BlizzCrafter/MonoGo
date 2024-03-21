namespace MonoGo.Engine.WindowsDX
{
	public static class MonoGoPlatform
	{
		public static void Init()
		{
			GameMgr.CurrentPlatform = Platform.Windows;
			GameMgr.CurrentGraphicsBackend = GraphicsBackend.DirectX;

			StuffResolver.AddStuffAs<IAlphaBlendEffectLoader>(new AlphaBlendEffectLoaderWindowsDX());
			StuffResolver.AddStuffAs<ITextInputBinder>(new TextInputBinderWindowsDX());
		}
	}
}
