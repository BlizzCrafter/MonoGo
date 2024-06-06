using System.Text.Json;
using static MonoGo.Engine.Axis;
using static MonoGo.Engine.Vector;
using static MonoGo.Engine.HSL;
using static MonoGo.Engine.HSLRange;
using static MonoGo.Engine.Range;
using static MonoGo.Engine.RangeF;
using static MonoGo.Engine.Particles.Modifiers.ColourInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.DragModifier;
using static MonoGo.Engine.Particles.Modifiers.FollowObjectModifier;
using static MonoGo.Engine.Particles.Modifiers.HueInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.LinearGravityModifier;
using static MonoGo.Engine.Particles.Modifiers.NoModifier;
using static MonoGo.Engine.Particles.Modifiers.OpacityFastFadeModifier;
using static MonoGo.Engine.Particles.Modifiers.OpacityInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.RotationModifier;
using static MonoGo.Engine.Particles.Modifiers.ScaleInterpolator2;
using static MonoGo.Engine.Particles.Modifiers.VelocityColourModifier;
using static MonoGo.Engine.Particles.Modifiers.VelocityHueModifier;
using static MonoGo.Engine.Particles.Modifiers.VortexModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.CircleContainerModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.RectContainerModifier;
using static MonoGo.Engine.Particles.Modifiers.Container.RectLoopContainerModifier;

namespace MonoGo.Engine
{
    internal static class JsonConverters
    {
        internal static JsonSerializerOptions SerializerOptions;

        internal static bool Initialized = false;

        internal static void Init()
        {
            if (Initialized) return;

            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true,
                Converters =
                {
                    new VectorConverter(),
                    new AxisConverter(),
                    new RangeConverter(),
                    new RangeFConverter(),
                    new HSLConverter(),
                    new ColourRangeConverter(),
                    new ColourInterpolator2Converter(),
                    new DragModifierConverter(),
                    new FollowObjectModifierConverter(),
                    new HueInterpolator2Converter(),
                    new LinearGravityModifierConverter(),
                    new NoModifierConverter(),
                    new OpacityFastFadeModifierConverter(),
                    new OpacityInterpolator2Converter(),
                    new RotationModifierConverter(),
                    new ScaleInterpolator2Converter(),
                    new VelocityColourModifierConverter(),
                    new VelocityHueModifierConverter(),
                    new VortexModifierConverter(),
                    new CircleContainerModifierConverter(),
                    new RectContainerModifierConverter(),
                    new RectLoopContainerModifierConverter()
                }
            };
            Initialized = true;
        }
    }
}
