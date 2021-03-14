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
			'=',
			'-',
			'.',
			' '
		};

		public bool simulateShadows = true;
		public Vector3 position = new Vector3();
		//Facing vectors
		public Vector3 forward = new Vector3();
		public Vector3 right = new Vector3();
		public Vector3 up = new Vector3();
		public Vector2 rotation = new Vector2(0, 0);

		private Vector2 resolution;
		//Width / height
		public float AspectRatio { get; private set; }
		//Vertical and horizontal FOV
		private float vFov = 88f;
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
		public int MaxMarchSteps { get; set; } = 50;
		public float MaxRayDistance { get; set; } = 150f;
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
			right = Vector3.Cross(forward, new Vector3(0, 1, 0));
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

			Parallel.For(0, (int)resolution.y,
				y =>
				{
					for (int x = 0; x < resolution.x; ++x)
					{
						//Caculate which direction the ray will travel, based on which pixel is currently selected
						Vector3 xComp = right * (x * pixelSize.x - halfWidth);
						Vector3 yComp = up * (y * pixelSize.y - halfHeight);
						Vector3 heading = (forward + xComp + yComp).Normalised();

						Collision col;
						Geometry hitObject = CastRay(scene, point, heading, MaxRayDistance, MaxMarchSteps, CollisionThreshold, out col, new Vector2(x, y));
					
						if (hitObject != null)
						{
							//Gets pointing vector from collision point to light source
							Vector3 pointToLight = scene.lights[0].position - col.point;
							float pointToLightDist = pointToLight.Magnitude();
							//Normalise pointToLight
							pointToLight /= pointToLightDist;
							//Ratio of shadow amount, maps dot product of 2 normalised vectors into a 0-1 value. 0 being full shadow, 1 being lit.
							float ratio = (Vector3.Dot(col.normal, pointToLight) + 1) / 2;
							//Default character (shaded), this overcomes shading bug when normal is NaN
							char character = '@';

							if (!float.IsNaN(ratio))
							{
								character = shading[(int)Math.Round(ratio * (shading.Length - 1))];

								if (simulateShadows)
								{
									if (ratio > 0.1f)
									{
										//Changes character to darkest character if theres an object between the collision point and the light source
										Collision shadowCol = new Collision();
										CastRay(scene, scene.lights[0].position, -pointToLight, pointToLightDist, 30, CollisionThreshold, out shadowCol, new Vector2(-1, -1));
										if (pointToLightDist - shadowCol.distanceTravelled > 0.1f)
											character = shading[0];
									}
								}
							}

							Renderer.DrawPixel(x, y, character, hitObject.color, ConsoleColor.Black);
						}
					}

				});
		}

		//Gets distance to closest object, use out for closest object reference
		public float GetDistanceToScene(Vector3 point, Scene scene, out Geometry hitObject)
		{
			hitObject = null;
			float dist = MaxRayDistance;
			float newDistance;

			for (int i = 0; i < scene.geometries.Count; ++i)
			{
				newDistance = scene.geometries[i].SignedDist(point);

				if (newDistance < dist)
				{
					hitObject = scene.geometries[i];
					dist = newDistance;
				}
			}

			return dist;
		}
		public float GetDistanceToScene(Vector3 point, Scene scene)
		{
			return GetDistanceToScene(point, scene, out _);
		}

		public Vector3 GetNormal(Vector3 point, Geometry geometry)
		{
			//Trapezoidal stuff, idk what any of this does

			return (epsXYY * geometry.SignedDist(point + epsXYY * epsilon) +
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
			public float signedDist;
			public float distanceTravelled;

			public Collision (Vector3 point, Vector3 normal, Geometry hitObject, float signedDist, float distanceTravelled)
			{
				this.point = point;
				this.normal = normal;
				this.hitObject = hitObject;
				this.signedDist = signedDist;
				this.distanceTravelled = distanceTravelled;
			}
		}

		public Geometry CastRay(Scene scene, Vector3 origin, Vector3 heading, float maxDist, float maxIterations, float collisionThreshold, out Collision col, Vector2 coord)
		{
			col = new Collision();
			float rayDist = 0;
			float distToSceneLimited;

			for (int i = 0; i < maxIterations; ++i)
			{
				//Get closest distance to the scene
				float distToScene = GetDistanceToScene(origin, scene, out Geometry hitObject);

				if (coord.x > -1 && distToScene < collisionThreshold + 0.05f)
					Renderer.DrawPixel((int)coord.x, (int)coord.y, ' ', ConsoleColor.White, ConsoleColor.White);

				if (distToScene < CollisionThreshold)
				{
					col = new Collision(origin, GetNormal(origin, hitObject), hitObject, distToScene, rayDist);

					return hitObject;
				}
				
				//Cancels if too long
				if (rayDist >= maxDist)
					break;

				//Moves point by the closest distance to ensure it doesn't hit anything
				distToSceneLimited = Math.Min(distToScene, maxDist - rayDist);
				origin += heading * distToSceneLimited;
				rayDist += distToSceneLimited;

				
			}

			col.distanceTravelled = rayDist;
			return null;
		}
	}
}
