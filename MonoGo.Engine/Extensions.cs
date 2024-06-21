using System;

namespace MonoGo.Engine
{
    public static class Extensions
    {
        public static T Next<T>(this T currentValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, currentValue);

            if (index == values.Length - 1)
            {
                return values[0];
            }

            return values[index + 1];
        }

        public static T Previous<T>(this T currentValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, currentValue);

            if (index == 0)
            {
                return values[^1];
            }

            return values[index - 1];
        }
    }
}
