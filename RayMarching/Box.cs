using MathLib;
using System;

namespace RayMarching
{
	class Box : Geometry
	{
		public Vector3 halfSize = new Vector3(1f, 1f, 1f);

		public Box(Vector3 position, ConsoleColor color, Vector3 halfSize) : base(position, color)
		{
			this.halfSize = halfSize;
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			Vector3 pointToSize = new Vector3(Math.Abs(point.x), Math.Abs(point.y), Math.Abs(point.z)) - halfSize;
			pointToSize.x = Math.Max(pointToSize.x, 0);
			pointToSize.y = Math.Max(pointToSize.y, 0);
			pointToSize.z = Math.Max(pointToSize.z, 0);

			return pointToSize.Magnitude();
		}
	}
}
