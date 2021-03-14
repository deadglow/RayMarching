using System;
using System.Collections.Generic;
using MathLib;
using ConsoleRender;
namespace RayMarching
{
	class Program
	{
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
			mainScene.lights.Add(new Light(new Vector3(30, 50, 20)));

			cam.position.y = 3;
			cam.position.z = -2;
			mainScene.geometries.Add(new Box(new Vector3(0, -2, 0), ConsoleColor.Green, new Vector3(30, 1, 30)));
			mainScene.geometries.Add(new Box(new Vector3(0, 1, 2), ConsoleColor.Blue, new Vector3(1, 5, 1)));
			//mainScene.geometries.Add(new Sphere(new Vector3(3, 5, 3), ConsoleColor.Red, 1f));
			//mainScene.geometries.Add(new Torus(new Vector3(-3, 3, 2), ConsoleColor.Magenta, new Vector2(3, 1)));
			Geometry[] geoUnionShapes =
			{
				new Box(Vector3.Zero, ConsoleColor.White, Vector3.One / 2),
				new Sphere(Vector3.Zero, ConsoleColor.White, 0.7f)
			};
			mainScene.geometries.Add(new GeoUnion(new Vector3(0, 10, 0), ConsoleColor.White, geoUnionShapes));		

			while (true)
			{
				if (!Console.KeyAvailable)
				{
					//mainScene.lights[0].position = cam.position;
					cam.Render(mainScene);
					Renderer.Render();
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
							cam.RotateYaw(-rotationSpeed);
							break;
						case ConsoleKey.NumPad4:
							cam.RotateYaw(rotationSpeed);
							break;
						case ConsoleKey.NumPad8:
							cam.RotatePitch(rotationSpeed);
							break;
						case ConsoleKey.NumPad5:
							cam.RotatePitch(-rotationSpeed);
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
							cam.VFov += 1f;
							break;
						case ConsoleKey.Add:
							cam.CollisionThreshold += 0.01f;
							break;
						case ConsoleKey.Subtract:
							cam.CollisionThreshold -= 0.01f;
							break;

						default:
							break;
					}
				}
			}

			
		}
	}
}
