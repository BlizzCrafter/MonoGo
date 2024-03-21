using System.Reflection;

namespace MonoGo.Engine.DesktopGL
{
	public class AlphaBlendEffectLoaderDesktopGl: AlphaBlendEffectLoader
	{
		protected override string _effectName => "MonoGo.Engine.DesktopGL.AlphaBlend_gl.mgfxo";

		protected override Assembly _assembly => Assembly.GetAssembly(typeof(AlphaBlendEffectLoaderDesktopGl));

	}
}
