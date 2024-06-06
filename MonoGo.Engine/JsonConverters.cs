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

            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);

            SerializerOptions.Converters.Add(new VectorConverter());
            SerializerOptions.Converters.Add(new AxisConverter());
            SerializerOptions.Converters.Add(new RangeConverter());
            SerializerOptions.Converters.Add(new RangeFConverter());
            SerializerOptions.Converters.Add(new HSLConverter());
            SerializerOptions.Converters.Add(new ColourRangeConverter());
            SerializerOptions.Converters.Add(new ColourInterpolator2Converter());
            SerializerOptions.Converters.Add(new DragModifierConverter());
            SerializerOptions.Converters.Add(new FollowObjectModifierConverter());
            SerializerOptions.Converters.Add(new HueInterpolator2Converter());
            SerializerOptions.Converters.Add(new LinearGravityModifierConverter());
            SerializerOptions.Converters.Add(new NoModifierConverter());
            SerializerOptions.Converters.Add(new OpacityFastFadeModifierConverter());
            SerializerOptions.Converters.Add(new OpacityInterpolator2Converter());
            SerializerOptions.Converters.Add(new RotationModifierConverter());
            SerializerOptions.Converters.Add(new ScaleInterpolator2Converter());
            SerializerOptions.Converters.Add(new VelocityColourModifierConverter());
            SerializerOptions.Converters.Add(new VelocityHueModifierConverter());
            SerializerOptions.Converters.Add(new VortexModifierConverter());
            SerializerOptions.Converters.Add(new CircleContainerModifierConverter());
            SerializerOptions.Converters.Add(new RectContainerModifierConverter());
            SerializerOptions.Converters.Add(new RectLoopContainerModifierConverter());

            Initialized = true;
        }
    }
}
