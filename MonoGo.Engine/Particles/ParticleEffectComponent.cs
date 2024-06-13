using System.Linq;
using Microsoft.Xna.Framework;
using MonoGo.Engine.EC;

namespace MonoGo.Engine.Particles
{
    public class ParticleEffectComponent : Component, IMovable
    {
        public ParticleEffect ParticleEffect { get; set; }
        
        public Vector2 Position { get; set; }
        public IMovable OwnerMovable { get; private set; }
        public bool FollowOwner { get; private set; } = true;

        public ParticleEffectComponent(
            ParticleEffect particleEffect,
            Vector2 position)
        {
            Visible = true;

            ParticleEffect = particleEffect;
            Position = position;
        }

        public ParticleEffectComponent(
            string particleEffectFilePath,
            Vector2 position)
        {
            Visible = true;

            ParticleEffect = new ParticleEffect(particleEffectFilePath);
            Position = position;
        }

        public override void Initialize()
        {
            base.Initialize();

            OwnerMovable = Owner.FindComponent<IMovable>();
        }

        public override void Update()
        {
            base.Update();

            ParticleEffect.Update((float)GameMgr.ElapsedTime);
            ParticleEffect.Trigger(FollowOwner ? OwnerMovable.Position : Position);
        }

        public void ToggleFollowOwner()
        {
            FollowOwner = !FollowOwner;
        }

        public void AttractParticlesTo(IMovable movable)
        {
            ParticleEffect.Modifiers<IFollowable>().ToList().ForEach(
                x => x.Followable = movable);
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
