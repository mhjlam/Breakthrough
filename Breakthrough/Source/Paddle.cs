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
			set
			{
				x = value;

				if (x < 0)
				{
					x = 0;
				}
				else if (x + Constants.PaddleWidth > Constants.ScreenWidth)
				{
					x = Constants.ScreenWidth - Constants.PaddleWidth;
				}
			}
		}

		public int Y
		{
			get { return y; }
			set { y = value; }
			//set
			//{
			//	y = value;

			//	if (y < 0)
			//	{
			//		y = 0;
			//	}
			//	else if (y + Constants.PaddleHeight > Constants.ScreenHeight)
			//	{
			//		y = Constants.ScreenHeight - Constants.PaddleHeight;
			//	}
			//}
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
		public Player(int x = Constants.ScreenWidth / 2 - Constants.PaddleWidth / 2, int y = Constants.ScreenHeight - 40 + Constants.PaddleHeight) : base(x, y)
		{
		}
		
		public void Update(KeyboardState keyState, MouseState mouseState)
		{
			X = mouseState.X;
		}
	}

	public class Robot : Paddle
	{
		// Predicted position of ball
		public int pX;
		public int pY;

		public Robot(int x = Constants.ScreenWidth / 2 - Constants.PaddleWidth / 2, int y = 40) : base(x, y)
		{
			pX = 0;
			pY = 0;
		}

		public void Update(Ball ball)
		{
			// Prediction without considering wall collisions
			float slope = (float)(ball.X - ball.X + ball.dX) / (ball.Y - ball.Y + ball.dY);
			int paddleDistance = ball.Y - y;
			pX = (int)Math.Abs(slope * -paddleDistance + ball.X);

			// Prediction while considering wall collisions
			int reflections = pX / Constants.ScreenWidth;

			if (reflections % 2 == 0)
			{
				pX = pX % Constants.ScreenWidth;
			}
			else
			{
				pX = Constants.ScreenWidth - (pX % Constants.ScreenWidth);
			}

			// Ball is going down
			if (ball.dY >= 0)
			{
				// Move robot paddle slowly to screen center
				if (x + Constants.PaddleWidth / 2 < Constants.ScreenWidth / 2)
				{
					X += 2;
				}
				else if (x + Constants.PaddleWidth / 2 > Constants.ScreenWidth / 2)
				{
					X -= 2;
				}
			}
			// Ball is going up and approaching robot paddle
			else if (ball.dY < 0 && ball.Y < Constants.ScreenHeight * 4 / 5)
			{
				// Follow the ball
				if (x + (Constants.PaddleWidth - Constants.BallSize) / 2 < pX - 2)
				{
					X += ball.Speed / 8 * 5;
				}
				else if (x + (Constants.PaddleWidth - Constants.BallSize) / 2 > pX + 2)
				{
					X -= ball.Speed / 8 * 5;
				}
			}
		}
	}
}

