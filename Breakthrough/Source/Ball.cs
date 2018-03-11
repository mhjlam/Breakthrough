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

		public Ball()
		{
			Reset();
		}

		public void Reset()
		{
			X = Constants.FieldWidth / 2 - Constants.BallSize / 2;
			Y = Constants.FieldHeight / 2 - Constants.BallSize / 2;
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

		private void Launch()
		{
			float angle = random.Next(-60, 61);
			int sign = random.Next(0, 2) * 2 - 1;

			dX = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f));
			dY = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f) * sign);

			Status = BallStatus.Launched;
		}

		private bool WallCollision()
		{
			return (X + dX < 0) || (X + Constants.BallSize + dX >= Constants.FieldWidth);
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
				if (Y > paddle.Y + Constants.PaddleHeight) return false;
			}
			else
			{
				if (Y > paddle.Y + Constants.PaddleHeight) return false;
				if (Y + Constants.BallSize < paddle.Y) return false;
			}

			return (X + Constants.BallSize > paddle.X && X < paddle.X + Constants.PaddleWidth);
		}

		private void BounceOffPaddle(Paddle paddle)
		{
			float angle = (2.0f * (X + (Constants.BallSize / 2) - paddle.X) - Constants.PaddleWidth);
			int sign = (paddle.Y < Constants.FieldHeight / 2) ? 1 : -1;

			dX = (int)(Speed * Math.Sin(angle * Math.PI / 180.0f));
			dY = (int)(Speed * Math.Cos(angle * Math.PI / 180.0f) * sign);

			Bounces++;
		}
	}
}
