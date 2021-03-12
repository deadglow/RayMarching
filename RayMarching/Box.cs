using MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayMarching
{
	class Box : Geometry
	{
		public Vector3 size = new Vector3(1f, 1f, 1f);

		public Box(Vector3 position, ConsoleColor color, Vector3 size) : base(position, color)
		{
			this.size = size;
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			Vector3 pointToSize = new Vector3(Math.Abs(point.x), Math.Abs(point.y), Math.Abs(point.z)) - size;
			pointToSize.x = Math.Max(pointToSize.x, 0);
			pointToSize.y = Math.Max(pointToSize.y, 0);
			pointToSize.z = Math.Max(pointToSize.z, 0);

			return pointToSize.Magnitude();
		}
	}
}
