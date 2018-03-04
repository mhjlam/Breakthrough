using System;

namespace Breakthrough
{
	public enum BallStatus
	{
		Ready,
		Launch,
		Launched
	}

	public class Ball
	{
		private Random random = new Random();
		
		public int X, Y;
		public int dX, dY;
		public int Speed;
		private int bounces;
		public BallStatus Status;

		public Ball()
		{
			Reset();
		}

		public void Reset()
		{
			X = Constants.ScreenWidth / 2 - Constants.BallSize / 2;
			Y = Constants.ScreenHeight / 2 - Constants.BallSize / 2;
			dX = 0;
			dY = 0;
			Speed = 8;
			bounces = 0;
			Status = BallStatus.Ready;
		}

		public void Update(Player player, Robot robot)
		{
			if (Status == BallStatus.Launch)
			{
				Launch();
			}

			// Check for collision
			if (WallCollision())
			{
				BounceOffWall();
			}
			else if (PaddleCollision(robot))
			{
				BounceOffPaddle(robot);
			}
			else if (PaddleCollision(player))
			{
				BounceOffPaddle(player);
			}

			// Increase ball speed based on bounces
			if (bounces == 5)
			{
				Speed++;
				bounces = 0;
			}

			// Update ball position
			X += dX;
			Y += dY;
		}

		private void Launch()
		{
			float angle = random.Next(-60, 61);
			int dirx = random.Next(0, 2) * 2 - 1;

			dX = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f) * dirx);
			dY = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f));

			Status = BallStatus.Launched;
		}

		private bool WallCollision()
		{
			return (Y + dY < 0) || (Y + Constants.BallSize + dY >= Constants.ScreenHeight);
		}

		private void BounceOffWall()
		{
			// Reflect off wall
			dY *= -1;
			bounces++;
		}

		private bool PaddleCollision(Paddle paddle)
		{
			if (dX < 0)
			{
				if (X < paddle.X) return false;
				if (X > paddle.X + Constants.PaddleWidth) return false;
			}
			else
			{
				if (X > paddle.X + Constants.PaddleWidth) return false;
				if (X + Constants.BallSize < paddle.X) return false;
			}

			return (Y + Constants.BallSize > paddle.Y && Y < paddle.Y + Constants.PaddleHeight);
		}

		private void BounceOffPaddle(Paddle paddle)
		{
			float angle = (2.14f * (Y - paddle.Y + Constants.BallSize) - 75.0f);
			int sign = (paddle.X < Constants.ScreenWidth / 2) ? 1 : -1;

			dX = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f) * sign);
			dY = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f));

			bounces++;
		}
	}
}
