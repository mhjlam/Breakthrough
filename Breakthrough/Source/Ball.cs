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
		public int Bounces;
		public BallStatus Status;

		public const int Width = 10;
		public const int Height = 10;

		public Ball()
		{
			Reset();
		}

		public void Reset()
		{
			X = 0;
			Y = 0;
			dX = 0;
			dY = 0;
			Speed = 8;
			Bounces = 0;
			Status = BallStatus.Ready;
		}

		public void Update(Field field, Player player, Robot robot)
		{
			if (Status == BallStatus.Launch)
			{
				float angle = random.Next(30, 150);

				dX = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f));
				dY = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f));

				Status = BallStatus.Launched;
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
			else
			{
				BrickCollision collision = field.Collision(this);
				BounceOffBrick(collision);
			}

			// Increase ball speed based on bounces
			if (Bounces == 5)
			{
				Speed++;
				Bounces = 0;
			}

			// Update ball position
			X += dX;
			Y += dY;
		}
		
		private bool WallCollision()
		{
			return (X + dX < 0) || (X + Ball.Width + dX >= Field.Width);
		}

		private void BounceOffWall()
		{
			// Reflect off wall
			dX *= -1;
			Bounces++;
		}

		private void BounceOffBrick(BrickCollision collision)
		{
			if (collision == BrickCollision.None) return;

			if (collision == BrickCollision.Horizontal)
			{
				dX *= -1;
			}
			else if (collision == BrickCollision.Vertical)
			{
				dY *= -1;
			}

			Bounces++;
		}

		private bool PaddleCollision(Paddle paddle)
		{
			if (dY < 0)
			{
				if (Y < paddle.Y) return false;
				if (Y > paddle.Y + Paddle.Height) return false;
			}
			else
			{
				if (Y > paddle.Y + Paddle.Height) return false;
				if (Y + Ball.Height < paddle.Y) return false;
			}

			return (X + Ball.Width > paddle.X && X < paddle.X + Paddle.Width);
		}

		private void BounceOffPaddle(Paddle paddle)
		{
			float bxc = X + Ball.Width / 2;
			float angle = 30 + (1 - (bxc - paddle.X) / Paddle.Width) * 120; // angle from 30 to 150
			int sign = (paddle is Player) ? -1 : 1;

			dX = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f));
			dY = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f) * sign);

			Bounces++;
		}
	}
}
