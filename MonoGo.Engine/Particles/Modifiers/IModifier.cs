namespace MonoGo.Engine.Particles.Modifiers {
    public interface IModifier 
    {        
        void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);
        void UpdateReferences(ref object _object);
    }
}