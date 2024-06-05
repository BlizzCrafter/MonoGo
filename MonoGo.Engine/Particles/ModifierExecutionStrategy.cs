using System.Collections.Generic;

namespace MonoGo.Engine.Particles
{
    using Modifiers;
    using TPL = System.Threading.Tasks;

    public abstract class ModifierExecutionStrategy {
        internal abstract void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);

        public static ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        public static ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
                foreach (var modifier in modifiers)
                    modifier.Update(elapsedSeconds, iterator.Reset());
            }
        }

        internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
            {
                TPL.Parallel.ForEach(modifiers, modifier => modifier.Update(elapsedSeconds, iterator.Reset()));
            }
        }
    }
}