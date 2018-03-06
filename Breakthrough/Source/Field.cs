using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Breakthrough
{
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
		public List<Brick> Bricks = new List<Brick>();
		private List<Brick> collisions = new List<Brick>();

		public Field(int level = 0)
		{
			// TODO: Load level layout from file
			Bricks.Add(new Brick(Constants.FieldWidth / 2 - 61, 60));
			Bricks.Add(new Brick(Constants.FieldWidth / 2 - 20, 60));
			Bricks.Add(new Brick(Constants.FieldWidth / 2 + 21, 60));
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
			// TODO: Play brick destruction/invulnerable animation
			//       and spawn power-ups if they carry them
			foreach (Brick block in collisions)
			{
				Bricks.Remove(block);
			}

			collisions.Clear();
		}
	}
}
