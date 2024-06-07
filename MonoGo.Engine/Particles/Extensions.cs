using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Particles
{
    public static class Extensions
    {
        /// <summary>
        /// Copies the X and Y components of the vector to the specified memory location.
        /// </summary>
        /// <param name="value">The value of the Vector2 coordinate.</param>
        /// <param name="destination">The memory location to copy the coordinate to.</param>
        public static unsafe void CopyTo(this Vector2 value, float* destination)
        {
            destination[0] = value.X;
            destination[1] = value.Y;
        }

        /// <summary>
        /// Gets the Vector2 of the Axis.
        /// </summary>
        public static Vector2 ToVector2(this Axis value, float magnitude = 1.0f)
        {
            return new Vector2(value.X * magnitude, value.Y * magnitude);
        }

        /// <summary>
        /// Gets the Axis of the Vector2.
        /// </summary>
        public static Axis Axis(this Vector2 value)
        {
            return new Axis(value.X, value.Y);
        }

        /// <summary>
        /// Gets the squared length of the Vector2.
        /// </summary>
        public static float LengthSq(this Vector2 value)
        {
            return value.X * value.X + value.Y * value.Y;
        }
    }
}
