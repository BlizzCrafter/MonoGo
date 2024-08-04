using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;

namespace MonoGo.Engine
{
    public interface IHaveGUI
    {
        void CreateUI();

        void Init()
        {
            Clear();

            UISystem._ownerStack.Push(this);
            var rootOwner = UISystem.Root.AddChild(new Panel(stylesheet: null!) { Identifier = $"Owner:{GetType().Name}", Anchor = UI.Defs.Anchor.Center }, true);
            rootOwner.Size.SetPercents(100, 100);
            rootOwner.IgnoreInteractions = true;
            CreateUI();
            UISystem._ownerStack.Pop();
        }

        void Clear()
        {
            UISystem.FindRootOwner(this)?.RemoveSelf(true);
            UISystem.Root.ToString();
        }
    }
}
