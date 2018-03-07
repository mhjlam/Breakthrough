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
				else if (x + Constants.PaddleWidth > Constants.FieldWidth)
				{
					x = Constants.FieldWidth - Constants.PaddleWidth;
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
		public int Speed = 8;

		MouseState prevMouseState;
		KeyboardState prevKeyState;

		public Player(int x = Constants.FieldWidth / 2 - Constants.PaddleWidth / 2, int y = Constants.FieldHeight - 40 + Constants.PaddleHeight) : base(x, y)
		{
			prevMouseState = Mouse.GetState();
			prevKeyState = Keyboard.GetState();
		}
		
		public void Update(KeyboardState keyState, MouseState mouseState)
		{
			if (mouseState.Position != prevMouseState.Position)
			{
				X = mouseState.X;
			}

			if (keyState.IsKeyDown(Keys.Right))
			{
				X += Speed;
			}
			else if (keyState.IsKeyDown(Keys.Left))
			{
				X -= Speed;
			}
			
			prevMouseState = mouseState;
			prevKeyState = keyState;
		}
	}

	public class Robot : Paddle
	{
		// Predicted position of ball
		public int pX;
		public int pY;

		public Robot(int x = Constants.FieldWidth / 2 - Constants.PaddleWidth / 2, int y = 40) : base(x, y)
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
			int reflections = pX / Constants.FieldWidth;

			if (reflections % 2 == 0)
			{
				pX = pX % Constants.FieldWidth;
			}
			else
			{
				pX = Constants.FieldWidth - (pX % Constants.FieldWidth);
			}

			// Ball is going down
			if (ball.dY >= 0)
			{
				// Move robot paddle slowly to screen center
				if (x + Constants.PaddleWidth / 2 < Constants.FieldWidth / 2)
				{
					X += 2;
				}
				else if (x + Constants.PaddleWidth / 2 > Constants.FieldWidth / 2)
				{
					X -= 2;
				}
			}
			// Ball is going up and approaching robot paddle
			else if (ball.dY < 0 && ball.Y < Constants.FieldHeight * 4 / 5)
			{
				// Follow the ball
				if (x + (Constants.PaddleWidth - Constants.BallSize) / 2 < pX - 2)
				{
					X += ball.Speed / 8 * 6;
				}
				else if (x + (Constants.PaddleWidth - Constants.BallSize) / 2 > pX + 2)
				{
					X -= ball.Speed / 8 * 6;
				}
			}
		}
	}
}

