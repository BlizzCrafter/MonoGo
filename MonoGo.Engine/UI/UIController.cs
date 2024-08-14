using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Engine.UI
{
    internal class UIController : Entity
    {
        internal UIController(Layer layer) : base(layer) { }

        public override void Update()
        {
            base.Update();

            UISystem.Update();
        }

        public override void Draw()
        {
            base.Draw();

            UISystem.Draw();
        }
    }
}
