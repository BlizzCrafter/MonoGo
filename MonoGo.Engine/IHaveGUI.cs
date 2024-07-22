using MonoGo.Engine.EC;
using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;

namespace MonoGo.Engine
{
    public interface IHaveGUI
    {
        void CreateUI();

        void Init(Entity owner)
        {
            Clear();

            UISystem._currentOwner = owner;
            var rootOwner = UISystem.Root.AddChild(new Panel(stylesheet: null!) { Identifier = $"Owner:{owner.GetType().Name}", Anchor = UI.Defs.Anchor.Center });
            rootOwner.Size.SetPercents(100, 100);
            rootOwner.IgnoreInteractions = true;
            CreateUI();
        }

        void Clear()
        {
            UISystem.Root._children.Find(x => x.Owner == this)?.RemoveSelf();
        }
    }
}
