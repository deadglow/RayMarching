using System;
using MathLib;

namespace RayMarching
{
	class GeoUnion : Geometry
	{
		public Geometry[] shapes;

		public GeoUnion(Vector3 position, ConsoleColor col, Geometry[] shapes) : base(position, col)
		{
			this.shapes = shapes;
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			float finalDist = shapes[0].SignedDist(point);

			for (int i = 1; i < shapes.Length; ++i)
			{
				finalDist = Math.Max(finalDist, shapes[i].SignedDist(point) * -1);
			}

			return finalDist;
		}
	}
}
