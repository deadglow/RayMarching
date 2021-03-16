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
			mainScene.light = new Light(Vector3.Zero);

			Geometry[] roomShapes =
			{
				new Sphere(Vector3.Zero, Vector3.Zero, ConsoleColor.White, 8),
				new Sphere(Vector3.Zero, Vector3.Zero, ConsoleColor.White, 7)
			};
			mainScene.geometries.Add(new GeoUnion(Vector3.Zero, Vector3.Zero, ConsoleColor.Green, roomShapes, GeoUnion.CalculationType.Cutout, GeoUnion.ModificationType.Base));

			Geometry[] geoUnionShapes =
			{
				new Torus(Vector3.Zero, Vector3.Zero, ConsoleColor.White, new Vector3(1, 0.5f, 1)),
				new Sphere(Vector3.Zero, Vector3.Zero, ConsoleColor.White, 1)
				//new Box(Vector3.Zero, Vector3.Zero, ConsoleColor.White, Vector3.One)

			};
			GeoUnion geo = new GeoUnion(new Vector3(0, 0, 0), Vector3.Zero, ConsoleColor.White, geoUnionShapes, GeoUnion.CalculationType.Lerp, GeoUnion.ModificationType.Base)
			{
				t = 0
			};

			Geometry[] obamaShape =
			{
				new Box(Vector3.Zero, Vector3.Zero, ConsoleColor.White, Vector3.One * 0.8f),
				geo
			};
			mainScene.geometries.Add(new GeoUnion(new Vector3(0, 0, 3), Vector3.Zero, ConsoleColor.Red, obamaShape, GeoUnion.CalculationType.Abs, GeoUnion.ModificationType.Base));

			while (true)
			{
				if (!Console.KeyAvailable)
				{
					cam.SetForward(Vector3.Forward);
					cam.RotatePitch(cam.rotation.x);
					cam.RotateYaw(cam.rotation.y);

					//mainScene.light.position.x = (float)Math.Sin(tick * 0.1f) / 2;
					//mainScene.light.position.y = (float)Math.Cos(tick * 0.1f) / 2;

					geo.t = (float)(Math.Sin(tick * 0.1f) + 1) / 2;

					//geo.shapes[1].position.x = (float)Math.Sin(tick * 0.1f) * 2;
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
