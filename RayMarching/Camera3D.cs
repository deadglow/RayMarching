using System;
using System.Threading.Tasks;
using MathLib;
using ConsoleRender;

namespace RayMarching
{
	class Camera3D
	{
		public const float epsilon = 0.001f;
		public static readonly Vector2 epsilonVec = new Vector2(1, -1);
		private static readonly Vector3 epsXYY = new Vector3(epsilonVec.x, epsilonVec.y, epsilonVec.y);
		private static readonly Vector3 epsYYX = new Vector3(epsilonVec.y, epsilonVec.y, epsilonVec.x);
		private static readonly Vector3 epsYXY = new Vector3(epsilonVec.y, epsilonVec.x, epsilonVec.y);
		private static readonly Vector3 epsXXX = new Vector3(epsilonVec.x, epsilonVec.x, epsilonVec.x);

		public static readonly char[] shading =
		{
			'@',
			'#',
			'+',
			'=',
			'-',
			'.',
			' '
		};

		public Vector3 position = new Vector3();
		//Facing vectors
		public Vector3 forward = new Vector3();
		public Vector3 right = new Vector3();
		public Vector3 up = new Vector3();

		private Vector2 resolution;
		//Width / height
		public float AspectRatio { get; private set; }
		//Vertical and horizontal FOV
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
		public int MaxMarchSteps { get; set; } = 30;
		public float MaxRayDistance { get; set; } = 30f;
		public float CollisionThreshold { get; set; } = 0.02f;

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

		public void RotateYaw(float angle)
		{
			float rad = angle * Program.Deg2Rad;
			Vector3 newForward = new Vector3(
				forward.x * (float)Math.Cos(rad) + forward.z * (float)Math.Sin(rad),
				forward.y,
				-forward.x * (float)Math.Sin(rad) + forward.z * (float)Math.Cos(rad));
			SetForward(newForward);
		}
		public void RotatePitch(float angle)
		{
			float rad = angle * Program.Deg2Rad;
			Vector3 newForward = new Vector3(
				forward.x,
				forward.y * (float)Math.Cos(rad) - forward.z * (float)Math.Sin(rad),
				forward.y * (float)Math.Sin(rad) + forward.z * (float)Math.Cos(rad));
			SetForward(newForward);
		}

		public void Render(Scene scene)
		{
			//Converts vFov to radians
			float fovRad = (float)Math.PI * (vFov / 2) / 180;
			//Used to move the topleft of the rect away from the center
			float halfHeight = (float)Math.Tan(fovRad);
			float halfWidth = halfHeight * AspectRatio;
			Vector2 camSize = new Vector2(halfWidth, halfHeight) * 2;
			//Size of pixels in world space
			Vector2 pixelSize = new Vector2(camSize.x / (resolution.x - 1), camSize.y / (resolution.y - 1));
			//Ray origin
			Vector3 point = position;

			for (int y = 0; y < resolution.y; ++y)
			{
				for (int x = 0; x < resolution.x; ++x)
				{
					//Caculate which direction the ray will travel, based on which pixel is currently selected
					Vector3 xComp = right * (x * pixelSize.x - halfWidth);
					Vector3 yComp = up * (y * pixelSize.y - halfHeight);
					Vector3 heading = (forward + xComp + yComp).Normalised();

					Collision col;
					Geometry hitObject = CastRay(scene, point, heading, MaxRayDistance, MaxMarchSteps, out col, new Vector2(x, y));
					
					if (hitObject != null)
					{
						//Gets pointing vector from
						Vector3 pointToLight = scene.lights[0].position - col.point;
						float pointToLightDist = pointToLight.Magnitude();
						pointToLight /= pointToLightDist;
						float ratio = (Vector3.Dot(col.normal, pointToLight) + 1) / 2;

						char character = shading[(int)Math.Round(ratio * (shading.Length - 1))];

						if (CastRay(scene, col.point + col.normal, pointToLight, pointToLightDist, 30, out _, new Vector2(-1, -1), col.hitObject) != null)
							character = shading[0];

						Renderer.DrawPixel(x, y, character, hitObject.color, ConsoleColor.Black);
					}
				}
			}
		}

		//Gets distance to closest object, use out for closest object reference
		public float GetDistanceToScene(Vector3 point, Scene scene, out Geometry hitObject, Geometry ignoredGeometry = null)
		{
			hitObject = null;
			float dist = MaxRayDistance;
			float newDistance;

			for (int i = 0; i < scene.geometries.Count; ++i)
			{
				if (scene.geometries[i] == ignoredGeometry)
					continue;

				newDistance = scene.geometries[i].SignedDist(point);

				if (newDistance < dist)
				{
					hitObject = scene.geometries[i];
					dist = newDistance;
				}
			}

			return dist;
		}
		public float GetDistanceToScene(Vector3 point, Scene scene, Geometry ignoredGeometry = null)
		{
			return GetDistanceToScene(point, scene, out _, ignoredGeometry);
		}

		public Vector3 GetNormal(Vector3 point, Geometry geometry)
		{
			//Trapezoidal stuff, idk what any of this does
			return 
				(
					epsXYY * geometry.SignedDist(point + epsXYY * epsilon) +
					epsYYX * geometry.SignedDist(point + epsYYX * epsilon) +
					epsYXY * geometry.SignedDist(point + epsYXY * epsilon) +
					epsXXX * geometry.SignedDist(point + epsXXX * epsilon)
				).Normalised();
		}

		public struct Collision
		{
			public Vector3 point;
			public Vector3 normal;
			public Geometry hitObject;
			public float distance;

			public Collision (Vector3 point, Vector3 normal, Geometry hitObject, float distance)
			{
				this.point = point;
				this.normal = normal;
				this.hitObject = hitObject;
				this.distance = distance;
			}
		}

		public Geometry CastRay(Scene scene, Vector3 origin, Vector3 heading, float maxDist, float maxIterations, out Collision col, Vector2 coord, Geometry ignoredGeometry = null)
		{
			col = new Collision();
			float rayDist = 0;

			for (int i = 0; i < maxIterations; ++i)
			{
				//Get closest distance to the scene
				float distToScene = GetDistanceToScene(origin, scene, out Geometry hitObject, ignoredGeometry);

				if (coord.x > -1 && distToScene < CollisionThreshold + 0.05f)
					Renderer.DrawPixel((int)coord.x, (int)coord.y, ' ', ConsoleColor.White, ConsoleColor.White);

				if (distToScene < CollisionThreshold)
				{
					col = new Collision(origin, GetNormal(origin, hitObject), hitObject, distToScene);

					return hitObject;
				}

				//Moves point by the closest distance to ensure it doesn't hit anything
				origin += heading * distToScene;
				rayDist += distToScene;

				//Cancels if too long
				if (rayDist > MaxRayDistance)
					break;
			}

			return null;
		}
	}
}
