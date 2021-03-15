using System;
using MathLib;

namespace RayMarching
{
	class GeoUnion : Geometry
	{
		public delegate float PerformCalculation(float a, float b, float t);
		public delegate float ModifyDistance(float a);
		public static PerformCalculation[] Calculations =
		{
			new PerformCalculation(Min),
			new PerformCalculation(Intersection),
			new PerformCalculation(Cutout),
			new PerformCalculation(Abs),
			new PerformCalculation(Lerp),
			new PerformCalculation(Smooth)
		};
		public static ModifyDistance[] Modifications =
		{
			new ModifyDistance(ModBase),
			new ModifyDistance(Abs)
		};

		public Geometry[] shapes;
		public CalculationType calculationType = CalculationType.Min;
		public ModificationType modType = ModificationType.Base;
		public float t = 0.3f;

		public GeoUnion(Vector3 position, Vector3 rotation, ConsoleColor col, Geometry[] shapes, CalculationType calcType, ModificationType modType) : base(position, rotation, col)
		{
			this.shapes = shapes;
			calculationType = calcType;
			this.modType = modType;
		}

		public enum CalculationType
		{
			Min,
			Intersection,
			Cutout,
			Abs,
			Lerp,
			Smooth
		}
		public enum ModificationType
		{
			Base,
			Abs
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			float finalDist = shapes[0].SignedDist(point);

			for (int i = 1; i < shapes.Length; ++i)
			{
				finalDist = Calculations[(int)calculationType](finalDist, shapes[i].SignedDist(point), t);
			}

			return Modifications[(int)modType](finalDist);
		}

		public static float Min(float a, float b, float t)
		{
			return Math.Min(a, b);
		}
		public static float Intersection(float a, float b, float t)
		{
			return Math.Max(a, b);
		}
		public static float Cutout(float a, float b, float t)
		{
			return Math.Max(a, b * -1);
		}
		public static float Abs(float a, float b, float t)
		{
			return Math.Max(a, Math.Abs(b));
		}
		public static float Smooth(float a, float b, float t)
		{
			float h = Math.Min(1.0f, Math.Max(0.5f * 0.5f * (a - b) / t, 0.0f));
			return (a + (b - a) * h) - t * h * (1 - h);
		}
		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}


		public static float ModBase(float a)
		{
			return a;
		}

		public static float Abs(float a)
		{
			return (float)Math.Abs(a);
		}
	}
}
