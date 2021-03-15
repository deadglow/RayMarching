using MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarching
{
	class Sphere : Geometry
	{
		public float radius;

		public Sphere(Vector3 position, Vector3 rotation, ConsoleColor color, float radius) : base(position, rotation, color)
		{
			this.radius = radius;
		}

		public override float SignedDist(Vector3 point)
		{
			return (point - position).Magnitude() - radius;
		}
	}
}
