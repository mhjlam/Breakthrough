using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Breakthrough
{
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

		public List<Brick> Bricks = new List<Brick>();

		public Field(string levelFile = "001")
		{
			LoadMap($"{levelFile:###}");

			//JsonConvert.DeserializeObject()

			//Bricks.Add(new Brick(Constants.FieldWidth / 2 - 60, 60));
			//Bricks.Add(new Brick(Constants.FieldWidth / 2 - 20, 60));
			//Bricks.Add(new Brick(Constants.FieldWidth / 2 + 20, 60));
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

			int brickWidth = Constants.FieldWidth / columns;
			int brickHeight = Constants.FieldHeight / 4 / rows;

			int startHeight = Constants.FieldHeight * 1 / 5;

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

		public bool Collision(Ball ball)
		{
			// TODO: Optimize
			Rectangle ballRect = new Rectangle(ball.X, ball.Y, Constants.BallSize, Constants.BallSize);

			foreach (Brick block in Bricks)
			{
				Rectangle blockRect = new Rectangle(block.X, block.Y, block.Width, block.Height);
				if (!Rectangle.Intersect(ballRect, blockRect).IsEmpty)
				{
					collisions.Add(block);
					return true; // TODO: handle case where ball hits two bricks simultaneously
				}
			}

			return false;
		}

		public void Update()
		{
			// TODO: Play brick destruction / invulnerable animation
			//       and spawn power-ups if they carry them
			foreach (Brick block in collisions)
			{
				Bricks.Remove(block);
			}

			collisions.Clear();
		}
	}
}
