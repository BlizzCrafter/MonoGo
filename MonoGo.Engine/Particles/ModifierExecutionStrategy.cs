using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using MonoGo.Engine.Particles.Modifiers;

namespace MonoGo.Engine.Particles
{
    using TPL = System.Threading.Tasks;

    public abstract class ModifierExecutionStrategy 
    {
        public class ModifierExecutionStrategyConverter : JsonConverter<ModifierExecutionStrategy>
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ModifierExecutionStrategy);
            }

            public override ModifierExecutionStrategy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, ModifierExecutionStrategy value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        internal abstract void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);

        public static ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        public static ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy 
        {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
                foreach (var modifier in modifiers)
                    modifier.Update(elapsedSeconds, iterator.Reset());
            }

            public override string ToString()
            {
                return nameof(Serial);
            }
        }

        internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy 
        {
            internal override void ExecuteModifiers(IEnumerable<IModifier> modifiers, float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
            {
                TPL.Parallel.ForEach(modifiers, modifier => modifier.Update(elapsedSeconds, iterator.Reset()));
            }

            public override string ToString()
            {
                return nameof(Parallel);
            }
        }

        public static ModifierExecutionStrategy Parse(string value)
        {
            if (string.Equals(nameof(Parallel), value, StringComparison.OrdinalIgnoreCase))
                return Parallel;

            if (string.Equals(nameof(Serial), value, StringComparison.OrdinalIgnoreCase))
                return Serial;

            throw new InvalidOperationException($"Unknown particle modifier execution strategy '{value}'");
        }
    }
}