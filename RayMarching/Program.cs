using System;
using System.Collections.Generic;
using MathLib;
using ConsoleRender;
namespace RayMarching
{
	class Program
	{
		public static ulong tick;
		public static float Deg2Rad = 0.017453f;
		public static Vector2 resolution = new Vector2(200, 160);
		public static Camera3D cam;
		public static Scene mainScene = new Scene();
		public static float moveSpeed = 0.5f;
		public static float rotationSpeed = 15f;

		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Renderer.InitialiseRenderer((short)resolution.x, (short)resolution.y, 2, "Consolas", 6);
			cam = new Camera3D(resolution);
			mainScene.light = new Light(new Vector3(0, 10, 0));
			cam.simulateShadows = false;

			cam.position.z -= 4;
			cam.position.y += 1;

			Geometry[] bodyparts =
			{
			new Ellipsoid(Vector3.Zero, Vector3.Zero, ConsoleColor.Red, new Vector3(1, 2, 1)),
			new Ellipsoid(new Vector3(0, -1.5f, 0), Vector3.Zero, ConsoleColor.Red, new Vector3(0.3f, 0.7f, 2f))
			};

			GeoUnion body = new GeoUnion(Vector3.Zero, Vector3.Zero, ConsoleColor.Red, bodyparts, GeoUnion.CalculationType.Cutout, GeoUnion.ModificationType.Base);

			Geometry face = new Ellipsoid(new Vector3(0f, 1f, -0.15f), Vector3.Zero, ConsoleColor.White, new Vector3(0.7f, 0.5f, 0.9f));

			mainScene.geometries.Add(body);
			mainScene.geometries.Add(face);


			while (true)
			{
				if (!Console.KeyAvailable)
				{
					Vector3 oldPos = cam.position;
					cam.position = Vector3.Zero;
					cam.SetForward(Vector3.Forward);
					cam.RotatePitch(cam.rotation.x);
					cam.RotateYaw(cam.rotation.y);
					cam.position = oldPos;


					cam.Render(mainScene);
					Renderer.Render();
					tick++;
				}
				else //input
				{
					switch(Console.ReadKey(true).Key)
					{
						case ConsoleKey.W:
							cam.position += cam.forward * moveSpeed;
							break;
						case ConsoleKey.S:
							cam.position -= cam.forward * moveSpeed;
							break;
						case ConsoleKey.A:
							cam.position -= cam.right * moveSpeed;
							break;
						case ConsoleKey.D:
							cam.position += cam.right * moveSpeed;
							break;
						case ConsoleKey.NumPad6:
							cam.rotation.y -= rotationSpeed;
							break;
						case ConsoleKey.NumPad4:
							cam.rotation.y += rotationSpeed;
							break;
						case ConsoleKey.NumPad8:
							cam.rotation.x -= rotationSpeed;
							break;
						case ConsoleKey.NumPad5:
							cam.rotation.x += rotationSpeed;
							break;
						case ConsoleKey.Q:
							cam.position -= cam.up *  moveSpeed;
							break;
						case ConsoleKey.E:
							cam.position += cam.up * moveSpeed;
							break;
						case ConsoleKey.F:
							cam.simulateShadows = !cam.simulateShadows;
							break;
						case ConsoleKey.G:
							cam.VFov -= 1f;
							break;
						case ConsoleKey.H:
							cam.VFov += 1f;
							break;
						case ConsoleKey.Add:
							cam.CollisionThreshold += 0.001f;
							break;
						case ConsoleKey.Subtract:
							cam.CollisionThreshold -= 0.001f;
							break;

						default:
							break;
					}
				}
			}

			
		}
	}
}
