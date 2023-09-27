using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.EC;
using Monofoxe.Extended.Engine.SceneSystem;

namespace Monofoxe.Extended.Samples.Misc
{
    public class SurfaceEntity : Entity
    {
        public Surface RenderTargetSurface { get; private set; }

        public SurfaceEntity(Layer layer) : base(layer)
        {
            RenderTargetSurface = new Surface(GraphicsMgr.Device.Viewport.Width, GraphicsMgr.Device.Viewport.Height);

            layer.OnPreDraw += Layer_OnPreDraw;
            layer.OnPostDraw += Layer_OnPostDraw;
        }

        protected virtual void Scene_OnPreDraw(Scene scene)
        {
            Surface.SetTarget(RenderTargetSurface, GraphicsMgr.VertexBatch.View);
            GraphicsMgr.Device.Clear(GraphicsMgr.CurrentCamera.BackgroundColor);
        }

        protected virtual void Scene_OnPostDraw(Scene scene)
        {
            Surface.ResetTarget();
        }

        protected virtual void Layer_OnPreDraw(Layer layer)
        {
            Surface.SetTarget(RenderTargetSurface, GraphicsMgr.VertexBatch.View);
            GraphicsMgr.Device.Clear(GraphicsMgr.CurrentCamera.BackgroundColor);
        }

        protected virtual void Layer_OnPostDraw(Layer layer)
        {
            Surface.ResetTarget();
        }

        public override void Destroy()
        {
            RenderTargetSurface.Dispose();
        }
    }
}
