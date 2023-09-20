
namespace Monofoxe.Extended.Engine.WindowsDX
{
	public class TextInputBinderWindowsDX : ITextInputBinder
	{
		public void Init()
		{
			GameMgr.Game.Window.TextInput += Input.TextInput;
		}
	}
}
