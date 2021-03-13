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
			Renderer.InitialiseRenderer((short)resolution.x, (short)resolution.y, 2, "Consolas", 6);
			cam = new Camera3D(resolution);
			mainScene.lights.Add(new Light(new Vector3(-30, -100, 0)));

			//mainScene.geometries.Add(new Sphere(new Vector3(0, 0, 0), ConsoleColor.Red, 1));
			mainScene.geometries.Add(new Sphere(new Vector3(6, 2, 5), ConsoleColor.Red, 3));
			mainScene.geometries.Add(new Sphere(new Vector3(-3, -5, 5), ConsoleColor.Red, 1f));
			mainScene.geometries.Add(new Box(new Vector3(1, 0, 5), ConsoleColor.Cyan, new Vector3(1, 1, 1)));

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
							cam.RotateYaw(rotationSpeed);
							break;
						case ConsoleKey.NumPad4:
							cam.RotateYaw(-rotationSpeed);
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
