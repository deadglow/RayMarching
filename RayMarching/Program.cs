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
						case ConsoleKey.UpArrow:
							cam.position.y -= moveSpeed;
							break;
						case ConsoleKey.DownArrow:
							cam.position.y += moveSpeed;
							break;
						case ConsoleKey.LeftArrow:
							cam.position.x -= moveSpeed;
							break;
						case ConsoleKey.RightArrow:
							cam.position.x += moveSpeed;
							break;
						case ConsoleKey.NumPad4:
							cam.position.z += moveSpeed;
							break;
						case ConsoleKey.NumPad1:
							cam.position.z -= moveSpeed;
							break;
						case ConsoleKey.Add:
							cam.VFov += 1f;
							break;
						case ConsoleKey.Subtract:
							cam.VFov-= 1f;
							break;

						default:
							break;
					}
				}
			}
		}
	}
}
