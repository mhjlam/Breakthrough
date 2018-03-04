using Microsoft.Xna.Framework.Input;
using System;

namespace Breakthrough
{
	public class Paddle
	{
		protected int x, y;
		protected int ox, oy;

		public int X
		{
			get { return x; }
			set { x = value; }
			//set
			//{
			//	x = value;

			//	if (x < 0)
			//	{
			//		x = 0;
			//	}
			//	else if (x + Constants.PaddleWidth > Constants.ScreenWidth)
			//	{
			//		x = Constants.ScreenWidth - Constants.PaddleWidth;
			//	}
			//}
		}

		public int Y
		{
			get { return y; }
			set
			{
				y = value;

				if (y < 0)
				{
					y = 0;
				}
				else if (y + Constants.PaddleHeight > Constants.ScreenHeight)
				{
					y = Constants.ScreenHeight - Constants.PaddleHeight;
				}
			}
		}

		public int Score;

		public Paddle(int x, int y)
		{
			X = x;
			Y = y;
			ox = X;
			oy = Y;
			Score = 0;
		}

		public void Reset()
		{
			X = ox;
			Y = oy;
		}
	}

	public class Player : Paddle
	{
		public Player(int x = Constants.ScreenWidth - (40 + Constants.PaddleWidth), int y = Constants.ScreenHeight / 2 - Constants.PaddleHeight / 2) : base(x, y)
		{
		}
		
		public void Update(KeyboardState keyState, MouseState mouseState)
		{
			Y = mouseState.Y;
		}
	}

	public class Robot : Paddle
	{
		// Predicted position of ball
		public int pX;
		public int pY;

		public Robot(int x = 40, int y = Constants.ScreenHeight / 2 - Constants.PaddleHeight / 2) : base(x, y)
		{
			pX = 0;
			pY = 0;
		}

		public void Update(Ball ball)
		{
			// Prediction without considering wall collisions
			float slope = (float)(ball.Y - ball.Y + ball.dY) / (ball.X - ball.X + ball.dX);
			int paddleDistance = ball.X - x;
			pY = (int)Math.Abs(slope * -paddleDistance + ball.Y);

			// Prediction while considering wall collisions
			int reflections = pY / Constants.ScreenHeight;

			if (reflections % 2 == 0)
			{
				pY = pY % Constants.ScreenHeight;
			}
			else
			{
				pY = Constants.ScreenHeight - (pY % Constants.ScreenHeight);
			}

			// Ball is going right
			if (ball.dX >= 0)
			{
				// Move robot paddle slowly to screen center
				if (y + Constants.PaddleHeight / 2 < Constants.ScreenHeight / 2)
				{
					Y += 2;
				}
				else if (y + Constants.PaddleHeight / 2 > Constants.ScreenHeight / 2)
				{
					Y -= 2;
				}
			}
			// Ball is going left and in close range to robot paddle
			else if (ball.X < Constants.ScreenWidth * 3 / 5 && ball.dX < 0)
			{
				// Follow the ball
				if (y + (Constants.PaddleHeight - Constants.BallSize) / 2 < pY - 2)
				{
					Y += ball.Speed / 8 * 5;
				}
				else if (y + (Constants.PaddleHeight - Constants.BallSize) / 2 > pY + 2)
				{
					Y -= ball.Speed / 8 * 5;
				}
			}
		}
	}
}

