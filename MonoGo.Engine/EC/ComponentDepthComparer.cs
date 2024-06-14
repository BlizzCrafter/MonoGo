using System.Collections.Generic;

namespace MonoGo.Engine.EC
{
    internal class ComponentDepthComparer : IComparer<Component>
    {
        public int Compare(Component x, Component y) =>
            y.Depth - x.Depth;
    }
}
