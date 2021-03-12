using System;
using MathLib;

namespace RayMarching
{
	public abstract class Geometry
	{
		public Vector3 position = new Vector3();
		public ConsoleColor color = ConsoleColor.White;

		public Geometry(Vector3 position, ConsoleColor color)
		{
			this.position = position;
			this.color = color;
		}

		public abstract float SignedDist(Vector3 point);
	}
}
