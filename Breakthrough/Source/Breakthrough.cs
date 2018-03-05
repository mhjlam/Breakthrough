using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Breakthrough
{
	public static class Constants
	{
		public const int ScreenWidth = 640;
		public const int ScreenHeight = 480;

		public const int PaddleWidth = 60;
		public const int PaddleHeight = 10;

		public const int BallSize = 10;
	}

	public class Breakthrough : Game
	{
		Ball ball;
		Robot robot;
		Player player;

		MouseState prevMouseState;
		KeyboardState prevKeyState;

		Texture2D surface;
		SpriteFont spriteFont;
		SpriteBatch spriteBatch;

		GraphicsDeviceManager graphicsDevice;

		public Breakthrough()
		{
			Content.RootDirectory = "Content";

			graphicsDevice = new GraphicsDeviceManager(this)
			{
				IsFullScreen = false,
				PreferredBackBufferWidth = Constants.ScreenWidth,
				PreferredBackBufferHeight = Constants.ScreenHeight
			};

			Window.AllowUserResizing = false;
			Window.Position = new Point(
				(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - graphicsDevice.PreferredBackBufferWidth) / 2 - 30,
				(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - graphicsDevice.PreferredBackBufferHeight) / 2);
		}

		public void Reset()
		{
			prevMouseState = Mouse.GetState();
			prevKeyState = Keyboard.GetState();

			ball = new Ball();
			robot = new Robot();
			player = new Player();
		}

		protected override void Initialize()
		{
			Reset();
			base.Initialize();
		}

		protected override void LoadContent()
		{
			surface = new Texture2D(graphicsDevice.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			spriteFont = Content.Load<SpriteFont>("fonts/Font");
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			surface.Dispose();
			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyState = Keyboard.GetState();

			if (keyState.IsKeyDown(Keys.Escape))
			{
				Exit();
			}
			else if (keyState.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
			{
				if (ball.Status == BallStatus.Ready)
				{
					ball.Status = BallStatus.Launch;
				}
			}

			if (ball.Status == BallStatus.Ready)
			{
				return;
			}

			ball.Update(player, robot);
			robot.Update(ball);
			player.Update(keyState, mouseState);

			// Boundary check
			if (ball.Y < 0)
			{
				player.Score++;
				ball.Reset();
				robot.Reset();
			}
			else if (ball.Y > Constants.ScreenHeight)
			{
				robot.Score++;
				ball.Reset();
				robot.Reset();
			}

			prevMouseState = mouseState;
			prevKeyState = keyState;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// Draw player paddle
			FillRectangle(player.X, player.Y, Constants.PaddleWidth, Constants.PaddleHeight, Color.White);

			// Draw robot paddle
			FillRectangle(robot.X, robot.Y, Constants.PaddleWidth, Constants.PaddleHeight, Color.White);

			// Draw ball
			FillRectangle(ball.X, ball.Y, Constants.BallSize, Constants.BallSize, Color.White);

			// Draw scores
			Vector2 robotScorePosition = new Vector2(Constants.ScreenWidth - 100, 100);
			Vector2 playerScorePosition = new Vector2(Constants.ScreenWidth - 100, Constants.ScreenHeight - 100 - spriteFont.MeasureString(player.Score.ToString()).Y);

			spriteBatch.Begin();
			spriteBatch.DrawString(spriteFont, robot.Score.ToString(), robotScorePosition, Color.White);
			spriteBatch.DrawString(spriteFont, player.Score.ToString(), playerScorePosition, Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void FillRectangle(int left, int top, int width, int height, Color color)
		{
			surface.SetData(new Color[] { color });
			Rectangle rect = new Rectangle(left, top, width, height);

			spriteBatch.Begin();
			spriteBatch.Draw(surface, rect, color);
			spriteBatch.End();
		}
	}

	public static class Program
	{
		[STAThread]
		static void Main()
		{
			using (Breakthrough breakthrough = new Breakthrough())
			{
				breakthrough.Run();
			}
		}
	}
}
