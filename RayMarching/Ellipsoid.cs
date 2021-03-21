using System;
using MathLib;

namespace RayMarching
{
	class Ellipsoid : Geometry
	{
		Vector3 dimensions;

		public Ellipsoid (Vector3 position, Vector3 rotation, ConsoleColor color, Vector3 dims) : base(position, rotation, color)
		{
			dimensions = dims;
		}

		public override float SignedDist(Vector3 point)
		{
			point -= position;
			Vector3 tempVec = new Vector3(point.x / dimensions.x, point.y / dimensions.y, point.z / dimensions.z);
			float k0 = Vector3.Magnitude(tempVec);
			float k1 = Vector3.Magnitude(new Vector3(tempVec.x / dimensions.x, tempVec.y / dimensions.y, tempVec.z / dimensions.z));

			return k0 * (k0 - 1) / k1;
		}
	}
}
