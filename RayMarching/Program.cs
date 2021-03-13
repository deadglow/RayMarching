using System;
using System.Collections.Generic;
using MathLib;
using ConsoleRender;
namespace RayMarching
{
	class Program
	{
		public static Vector2 resolution = new Vector2(200, 160);
		public static Camera3D cam;
		public static Scene mainScene = new Scene();
		public static float moveSpeed = 0.5f;

		static void Main(string[] args)
		{
			Renderer.InitialiseRenderer((short)resolution.x, (short)resolution.y, 2, "Consolas", 6);
			cam = new Camera3D(resolution);
			mainScene.lights.Add(new Light(new Vector3(1, -10, 1)));

			//mainScene.geometries.Add(new Sphere(new Vector3(0, 0, 0), ConsoleColor.Red, 1));
			mainScene.geometries.Add(new Sphere(new Vector3(6, 0, 5), ConsoleColor.Red, 3));
			mainScene.geometries.Add(new Box(new Vector3(1, 0, 5), ConsoleColor.Cyan, new Vector3(1, 1, 1)));

			while (true)
			{
				if (!Console.KeyAvailable)
				{
					cam.Render(mainScene);
					Renderer.Render();
				}
				else
				{
					switch(Console.ReadKey(true).Key)
					{
						case ConsoleKey.NumPad8:
							cam.position.y -= moveSpeed;
							break;
						case ConsoleKey.NumPad5:
							cam.position.y += moveSpeed;
							break;
						case ConsoleKey.NumPad4:
							cam.position.x -= moveSpeed;
							break;
						case ConsoleKey.NumPad6:
							cam.position.x += moveSpeed;
							break;
						case ConsoleKey.NumPad7:
							cam.position.z += moveSpeed;
							break;
						case ConsoleKey.NumPad1:
							cam.position.z -= moveSpeed;
							break;
						case ConsoleKey.F:
							cam.VFov -= 1f;
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
