using System.Reflection;

namespace Monofoxe.Extended.Engine.DesktopGL
{
	public class AlphaBlendEffectLoaderDesktopGl: AlphaBlendEffectLoader
	{
		protected override string _effectName => "Monofoxe.Extended.Engine.DesktopGL.AlphaBlend_gl.mgfxo";

		protected override Assembly _assembly => Assembly.GetAssembly(typeof(AlphaBlendEffectLoaderDesktopGl));

	}
}
