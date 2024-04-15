﻿using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine.Utils
{
	/// <summary>
	/// Represents an angle measured in degrees. 
	/// NOTE: Angle will auto-wrap in 0..359 range.
	/// </summary>
	public struct Angle : IEquatable<Angle>
	{
		public static readonly Angle Up = new Angle(270);
		public static readonly Angle Down = new Angle(90);
		public static readonly Angle Left = new Angle(180);
		public static readonly Angle Right = new Angle(0);

		public double Degrees 
		{
			get => _degrees; 
			set
			{
				_degrees = value;
				Normalize();
			}
		}

		public double Radians 
		{
			get => (_degrees / 360.0) * Math.PI * 2.0;
			set 
			{
				Degrees = (value / (Math.PI * 2.0)) * 360.0;
			}
		}

		public float DegreesF => (float)Degrees;
		public float RadiansF => (float)Radians;
		

		private double _degrees;


		public Angle(double degrees)
		{
			_degrees = degrees;
			Normalize();
		}

		public Angle(Vector2 vector)
		{
			_degrees = (Math.Atan2(vector.Y, vector.X) * 360.0) / (Math.PI * 2.0);
			Normalize();
		}

		public Angle(Vector2 point1, Vector2 point2) : this(point2 - point1) {}

		/// <summary>
		/// Creates Angle object from an angle measured in radians.
		/// </summary>
		public static Angle FromRadians(double radians)
		{
			var angle = new Angle();
			angle.Radians = radians;
			return angle;
		}

		/// <summary>
		/// Returns the difference between two angles in the range of -180..180.
		/// Negative angle represents going counter-clockwise, 
		/// positive - clockwise.
		/// </summary>
		public double Difference(Angle other) =>
			((Degrees - other.Degrees + 180.0) % 360.0 + 360.0) % 360.0 - 180.0;

		
		public Vector2 ToVector2() =>
			new Vector2((float)Math.Cos(Radians), (float)Math.Sin(Radians));
	
		public static Angle Lerp(Angle angle1, Angle angle2, double value)
		{
			var diff = angle1.Difference(angle2);
			return angle1 - diff * value;
		}

		public static double ToDegrees(double radians) =>
			(radians / (Math.PI * 2.0)) * 360.0;

		public static double ToRadians(double radians) =>
			(radians / 360.0) * Math.PI * 2.0;


		public bool Equals(Angle other) =>
			_degrees == other._degrees;

		public override bool Equals(object obj)
		{
			if (obj is Angle)
			{
				return Equals(this);
			}
			return false;
		}

		#region Operators.

		public static Angle operator +(Angle a1, Angle a2) =>
			new Angle(a1._degrees + a2._degrees);

		public static Angle operator +(Angle a, double num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(Angle a, float num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(Angle a, int num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(double num, Angle a) =>
			new Angle(a._degrees + num);

		public static Angle operator +(float num, Angle a) =>
			new Angle(a._degrees + num);

		public static Angle operator +(int num, Angle a) =>
			new Angle(a._degrees + num);



		public static Angle operator -(Angle a1, Angle a2) =>
			new Angle(a1._degrees - a2._degrees);

		public static Angle operator -(Angle a, double num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(Angle a, float num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(Angle a, int num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(double num, Angle a) =>
			new Angle(num - a._degrees);

		public static Angle operator -(float num, Angle a) =>
			new Angle(num - a._degrees);

		public static Angle operator -(int num, Angle a) =>
			new Angle(num - a._degrees);



		public static Angle operator *(Angle a, double num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(Angle a, float num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(Angle a, int num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(double num, Angle a) =>
			new Angle(a._degrees * num);

		public static Angle operator *(float num, Angle a) =>
			new Angle(a._degrees * num);

		public static Angle operator *(int num, Angle a) =>
			new Angle(a._degrees * num);



		public static Angle operator /(Angle a, double num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(Angle a, float num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(Angle a, int num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(double num, Angle a) =>
			new Angle(num / a._degrees);

		public static Angle operator /(float num, Angle a) =>
			new Angle(num / a._degrees);

		public static Angle operator /(int num, Angle a) =>
			new Angle(num / a._degrees);



		public static bool operator >(Angle a1, Angle a2) =>
			a1._degrees > a2._degrees;

		public static bool operator <(Angle a1, Angle a2) =>
			a1._degrees < a2._degrees;

		public static bool operator >=(Angle a1, Angle a2) =>
			a1._degrees >= a2._degrees;

		public static bool operator <=(Angle a1, Angle a2) =>
			a1._degrees <= a2._degrees;

		public static bool operator ==(Angle a1, Angle a2) =>
			a1._degrees == a2._degrees;

		public static bool operator !=(Angle a1, Angle a2) =>
			a1._degrees != a2._degrees;

		#endregion Operators.
			
		public override int GetHashCode() =>
			_degrees.GetHashCode();

		public override string ToString() =>
			_degrees.ToString();
		

		void Normalize() =>
			_degrees = (_degrees % 360.0 + 360.0) % 360.0;
	}
}
