using System.Reflection;

namespace Monofoxe.Extended.Engine.WindowsDX
{
	public class AlphaBlendEffectLoaderWindowsDX : AlphaBlendEffectLoader
	{
		protected override string _effectName => "Monofoxe.Extended.Engine.WindowsDX.AlphaBlend_dx.mgfxo";

		protected override Assembly _assembly => Assembly.GetAssembly(typeof(AlphaBlendEffectLoaderWindowsDX));
	}
}
