using System;
using System.Threading.Tasks;
using MathLib;
using ConsoleRender;

namespace RayMarching
{
	class Camera3D
	{
		public Vector3 position = new Vector3();
		public Vector3 forward = new Vector3();
		public Vector3 right = new Vector3();
		public Vector3 up = new Vector3();

		private Vector2 resolution;
		public float AspectRatio { get; private set; }
		private float vFov = 70f;
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
		public int MaxMarchSteps { get; set; } = 200;
		public float MaxRayDistance { get; set; } = 100f;
		public float CollisionThreshold { get; set; } = 0.3f;

		public Camera3D(Vector2 resolution)
		{
			this.resolution = resolution;
			AspectRatio = resolution.x / resolution.y;
			VFov = vFov;
			SetForward(new Vector3(0, 0, 1));
		}

		public void SetForward(Vector3 vect)
		{
			forward = vect;
			right = Vector3.Cross(forward, new Vector3(0, -1, 0));
			up = Vector3.Cross(forward, right);
		}

		public void Render(Scene scene)
		{
			float fovRad = (float)Math.PI * (vFov / 2) / 180;
			float halfHeight = (float)Math.Tan(fovRad);
			float halfWidth = halfHeight * AspectRatio;
			Vector2 camSize = new Vector2(halfWidth, halfHeight) * 2;
			Vector2 pixelSize = new Vector2(camSize.x / (resolution.x - 1), camSize.y / (resolution.y - 1));

			//for (int y = 0; y < resolution.y; ++y)
			Parallel.For(0, (int)resolution.y,
				y =>
				{
					for (int x = 0; x < resolution.x; ++x)
					{
						Vector3 xComp = right * (x * pixelSize.x - halfWidth);
						Vector3 yComp = up * (y * pixelSize.y - halfHeight);
						Vector3 heading = (forward + xComp + yComp).Normalised();

						Vector3 point = position;
						float rayDist = 0;

						for (int i = 0; i < MaxMarchSteps; ++i)
						{
							float distToScene = GetDistanceToScene(point, scene, out Geometry hitObject);

							if (distToScene < CollisionThreshold)
							{
								Renderer.DrawPixel(x, y, ' ', hitObject.color, ConsoleColor.Black);
							}

							point += heading * distToScene;
							rayDist += distToScene;

							if (rayDist > MaxRayDistance)
								break;
						}
					}
				});
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
	}
}
