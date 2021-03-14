using System;
using MathLib;

namespace RayMarching
{
	class Torus : Geometry
	{
		Vector2 dimensions;

		public Torus(Vector3 position, ConsoleColor col, Vector2 dim) : base(position, col)
		{
			dimensions = dim;
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			Vector2 qx = new Vector2(point.x, point.z);
			Vector2 q = new Vector2(qx.Magnitude() - dimensions.x, point.y);
			return q.Magnitude() - dimensions.y;
		}
	}
}
