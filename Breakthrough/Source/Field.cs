using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Breakthrough
{
	public enum BrickCollision
	{
		None,
		Horizontal,
		Vertical
	}

	public struct BrickType
	{
		public int Width;
		public int Height;
		public int[] Color;
		public int Durability;
	}

	public struct Level
	{
		public BrickType[] BrickTypes;
		public int[,] BrickLayout;
	}

	public struct Brick
	{
		public int X, Y;
		public int Width;
		public int Height;

		public Color Color;
		public int Durability;

		public Brick(int x, int y, Color? color = null, int width = 40, int height = 20, int durability = 1)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			Color = color ?? Color.White;
			Durability = durability;
		}
	}

	public class Field
	{
		private Level level;
		private List<Brick> collisions = new List<Brick>();

		public const int Width = 600;
		public const int Height = 600;

		public List<Brick> Bricks = new List<Brick>();

		public Field(string levelFile = "001")
		{
			LoadMap($"{levelFile:###}");
		}

		private void LoadMap(string levelFile)
		{
			if (levelFile == "") return;
			if (!levelFile.EndsWith(".json")) levelFile += ".json";

			string path = $"Content/levels/{levelFile}";
			if (!File.Exists(path)) return;

			string contents = File.ReadAllText(path);
			level = (Level)JsonConvert.DeserializeObject(contents, typeof(Level));

			int rows = level.BrickLayout.GetLength(0);
			int columns = level.BrickLayout.GetLength(1);

			int brickWidth = Field.Width / columns;
			int brickHeight = Field.Height / 4 / rows;

			int startHeight = Field.Height * 1 / 5;

			for (int r = 0; r < rows; ++r)
			{
				int x = 0;

				for (int c = 0; c < columns; ++c)
				{
					int value = level.BrickLayout[r, c] - 1;

					if (value >= 0 && value < level.BrickTypes.Length)
					{
						BrickType type = level.BrickTypes[value];

						Bricks.Add(new Brick(x, startHeight + r * brickHeight, new Color(type.Color[0], type.Color[1], type.Color[2]), brickWidth, brickHeight, type.Durability));
					}

					x += brickWidth;
				}
			}
		}

		public BrickCollision Collision(Ball ball)
		{
			foreach (Brick brick in Bricks)
			{
				// TODO: handle case where ball hits multiple bricks simulataneously

				if (ball.X + Ball.Width + ball.dX > brick.X && ball.X + ball.dX < brick.X + brick.Width && ball.Y + Ball.Height > brick.Y && ball.Y < brick.Y + brick.Height)
				{
					collisions.Add(brick);
					return BrickCollision.Horizontal;
				}
				else if (ball.X + Ball.Width > brick.X && ball.X < brick.X + brick.Width && ball.Y + Ball.Height + ball.dY > brick.Y && ball.Y + ball.dY < brick.Y + brick.Height)
				{
					collisions.Add(brick);
					return BrickCollision.Vertical;
				}
			}

			return BrickCollision.None;
		}

		public void Update()
		{
			// TODO: Play brick destruction / invulnerable animation and spawn power-ups if they carry them
			foreach (Brick block in collisions)
			{
				Bricks.Remove(block);
			}

			collisions.Clear();
		}
	}
}
