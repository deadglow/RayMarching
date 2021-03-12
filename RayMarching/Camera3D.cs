using System;
using MathLib;
using ConsoleRender;

namespace RayMarching
{
	class Camera3D
	{
		public Vector3 position = new Vector3();
		private Vector2 resolution;
		public float AspectRatio { get; private set; }
		private float vFov = 100f;
		public float VFov 
		{
			get
			{
				return vFov;
			}
			set
			{
				vFov = value;
				HFov = vFov / AspectRatio;
			}
		}
		public float HFov { get; private set; }
		public float ClipPlaneDistance { get; set; } = 0.1f;
		public int MaxMarchSteps { get; set; } = 500;
		public float MaxRayDistance { get; set; } = 1000f;
		public float CollisionDistance { get; set; } = 1f;

		public Camera3D(Vector2 resolution)
		{
			this.resolution = resolution;
			AspectRatio = resolution.x / resolution.y;
			VFov = vFov;
		}

		public void Render(Scene scene)
		{
			Vector3 clipPlaneBorder = new Vector3(ClipPlaneDistance / (float)Math.Cos(HFov), ClipPlaneDistance / (float)Math.Sin(VFov), ClipPlaneDistance);
			Vector3 clipPlaneSize = clipPlaneBorder * 2;
			Vector3 point;
			Vector3 heading;
			Geometry hitObject;

			for (int x = 0; x < resolution.x; ++x)
			{
				for (int y = 0; y < resolution.y; ++y)
				{
					Renderer.DrawPixel((short)x, (short)y, ' ', ConsoleColor.Green, ConsoleColor.White);
					point = position - clipPlaneBorder + new Vector3(clipPlaneSize.x * x / resolution.x, clipPlaneSize.y * y / resolution.y, 1);
					heading = (point - position).Normalised();

					hitObject = CastRay(point, heading, scene);

					if (hitObject != null)
						Renderer.DrawPixel(x, y, ' ', hitObject.color, ConsoleColor.Black);
				}
			}
		}

		//Gets distance to closest object, use out for closest object reference
		public float GetDistanceToScene(Vector3 point, Scene scene, out Geometry hitObject)
		{
			Geometry closestObject = null;
			float dist = MaxRayDistance;
			float newDistance;

			for (int i = 0; i < scene.geometries.Count; ++i)
			{
				newDistance = scene.geometries[i].SignedDist(point);

				if (newDistance < dist)
				{
					closestObject = scene.geometries[i];
					dist = newDistance;
				}

			}

			hitObject = closestObject;
			return dist;
		}
		public float GetDistanceToScene(Vector3 point, Scene scene)
		{
			return GetDistanceToScene(point, scene, out _);
		}

		//Fires a ray, iterating until an object is hit or max distance is reached
		public Geometry CastRay(Vector3 start, Vector3 heading, Scene scene)
		{
			float rayDist = 0;
			Geometry hitObject;

			for (int i = 0; i < MaxMarchSteps; ++i)
			{
				float distToScene = GetDistanceToScene(start, scene, out hitObject);

				if (distToScene < CollisionDistance)
				{
					return hitObject;
				}

				start += heading * distToScene;
				rayDist += distToScene;

				if (rayDist > MaxRayDistance)
					break;
			}

			return null;
		}
	}
}
