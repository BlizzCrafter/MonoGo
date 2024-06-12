using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using System.Linq;

namespace MonoGo.Engine.Particles
{
    public class ParticleEffectEntity : Entity
    {
        public ParticleEffect ParticleEffect { get; set; }
        
        public IMovable Moveable { get; set; }
        public IMovable FollowMoveable { get; set; }

        public ParticleEffectEntity(
            Layer layer, 
            ParticleEffect particleEffect,
            IMovable followable = default,
            IMovable position = default) 
            : base(layer)
        {
            Moveable = position;
            FollowMoveable = followable;
            ParticleEffect = particleEffect;
            UpdateFollowMoveable();
        }

        public ParticleEffectEntity(
            Layer layer,
            string particleEffectFilePath,
            IMovable followable = default,
            IMovable position = default)
            : base(layer)
        {
            Moveable = position;
            FollowMoveable = followable;
            ParticleEffect = new ParticleEffect(particleEffectFilePath);
            UpdateFollowMoveable();
        }

        public override void Update()
        {
            base.Update();

            ParticleEffect.Update((float)GameMgr.ElapsedTime);
            ParticleEffect.Trigger(Moveable.Position);
        }

        public void UpdateFollowMoveable()
        {
            ParticleEffect.Modifiers<IFollowable>().ToList().ForEach(x => x.Followable = FollowMoveable);
        }

        public override void Draw()
        {
            base.Draw();

            ParticleEffect.Draw();
        }

        public void Serialize(string filePath)
        {
            ParticleEffect.Serialize(filePath);
        }

        public void Deserialize(string filePath)
        {
            ParticleEffect = new ParticleEffect(filePath);
        }
    }
}
