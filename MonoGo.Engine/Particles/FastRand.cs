﻿using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine.Particles
{
    /// <summary>
    /// Defines a random number generator which uses the FastRand algorithm to generate random values.
    /// </summary>
    internal static class FastRand {
        static int _state = 1;

        public static void Seed(int seed) {
            if (seed < 1)
                throw new ArgumentOutOfRangeException("seed", "seed must be greater than zero");
            
            _state = seed;
        }
        
        /// <summary>
        /// Gets the next random integer value.
        /// </summary>
        /// <returns>A random positive integer.</returns>
        public static int NextInteger() {
            _state = 214013 * _state + 2531011;
            return (_state >> 16) & 0x7FFF;
        }

        /// <summary>
        /// Gets the next random integer value which is greater than zero and less than or equal to
        /// the specified maxmimum value.
        /// </summary>
        /// <param name="max">The maximum random integer value to return.</param>
        /// <returns>A random integer value between zero and the specified maximum value.</returns>
        public static int NextInteger(int max) {
            return (int)(max * NextSingle() + 0.5f);
        }

        /// <summary>
        /// Gets the next random integer between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        public static int NextInteger(int min, int max) {
            return (int)((max - min) * NextSingle() + 0.5f) + min;
        }

        /// <summary>
        /// Gets the next random integer between the specified range of values.
        /// </summary>
        /// <param name="range">A range representing the inclusive minimum and maximum values.</param>
        /// <returns>A random integer between the specified minumum and maximum values.</returns>
        public static int NextInteger(Range range) {
            return NextInteger(range.Min, range.Max);
        }

        /// <summary>
        /// Gets the next random single value.
        /// </summary>
        /// <returns>A random single value between 0 and 1.</returns>
        public static float NextSingle() {
            return NextInteger() / (float)short.MaxValue;
        }

        /// <summary>
        /// Gets the next random single value which is greater than zero and less than or equal to
        /// the specified maxmimum value.
        /// </summary>
        /// <param name="max">The maximum random single value to return.</param>
        /// <returns>A random single value between zero and the specified maximum value.</returns>
        public static float NextSingle(float max) {
            return max * NextSingle();
        }

        /// <summary>
        /// Gets the next random single value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random single value between the specified minimum and maximum values.</returns>
        public static float NextSingle(float min, float max) {
            return (max - min) * NextSingle() + min;
        }

        /// <summary>
        /// Gets the next random single value between the specified range of values.
        /// </summary>
        /// <param name="range">A range representing the inclusive minimum and maximum values.</param>
        /// <returns>A random single value between the specified minimum and maximum values.</returns>
        public static float NextSingle(RangeF range) {
            return NextSingle(range.Min, range.Max);
        }

        /// <summary>
        /// Gets the next random angle value.
        /// </summary>
        /// <returns>A random angle value.</returns>
        public static float NextAngle() {
            return NextSingle((float)Math.PI * -1f, (float)Math.PI);
        }

        public static void NextUnitVector(out Axis axis)
        {
            var angle = NextAngle();

            axis = new Axis((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static void NextUnitVector(out Vector2 vector) {
            var angle = NextAngle();

            vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static void NextColour(out HSLColor colour, HSLRange range)
        {
            var maxH = range.Max.H >= range.Min.H
                ? range.Max.H
                : range.Max.H + 360;
            colour = new HSLColor(NextSingle(range.Min.H, maxH),
                                 NextSingle(range.Min.S, range.Max.S),
                                 NextSingle(range.Min.L, range.Max.L));
        }
    }
}