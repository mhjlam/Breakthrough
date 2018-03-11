using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Breakthrough
{
	public static class Constants
	{
		public const int FieldWidth = 600;
		public const int FieldHeight = 600;

		public const int ScreenWidth = 800;
		public const int ScreenHeight = 600;

		public const int PaddleWidth = 60;
		public const int PaddleHeight = 10;

		public const int BallSize = 10;
		public const int BallWidth = 10;
		public const int BallHeight = 10;
	}

	public class Breakthrough : Game
	{
		Ball ball;
		Field field;
		Robot robot;
		Player player;

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
			ball = new Ball();
			field = new Field();
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
			else if (keyState.IsKeyDown(Keys.Space))
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

			ball.Update(field, player, robot);
			field.Update();
			robot.Update(ball);
			player.Update(keyState, mouseState);

			// Boundary check
			if (ball.Y < 0)
			{
				player.Score++;
				ball.Reset();
				robot.Reset();
			}
			else if (ball.Y > Constants.FieldHeight)
			{
				robot.Score++;
				ball.Reset();
				robot.Reset();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// Draw playing field
			FillRectangle(-10, 0, 10, Constants.FieldHeight, Color.White);
			FillRectangle(Constants.FieldWidth, 0, 10, Constants.FieldHeight, Color.White);

			// Draw field blocks
			foreach (Brick block in field.Bricks)
			{
				FillRectangle(block.X - 1, block.Y - 1, block.Width - 1, block.Height - 1, block.Color);
			}

			// Draw player paddle
			FillRectangle(player.X, player.Y, Constants.PaddleWidth, Constants.PaddleHeight, Color.White);

			// Draw robot paddle
			FillRectangle(robot.X, robot.Y, Constants.PaddleWidth, Constants.PaddleHeight, Color.White);

			// Draw ball
			FillRectangle(ball.X, ball.Y, Constants.BallSize, Constants.BallSize, Color.White);

			// Draw scores
			Vector2 robotScorePosition = new Vector2(Constants.ScreenWidth - 50, 50);
			Vector2 playerScorePosition = new Vector2(Constants.ScreenWidth - 50, Constants.ScreenHeight - 50 - spriteFont.MeasureString(player.Score.ToString()).Y);

			spriteBatch.Begin();
			spriteBatch.DrawString(spriteFont, robot.Score.ToString(), robotScorePosition, Color.White);
			spriteBatch.DrawString(spriteFont, player.Score.ToString(), playerScorePosition, Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void FillRectangle(int left, int top, int width, int height, Color color)
		{
			int offsetX = (Constants.ScreenWidth - Constants.FieldWidth) / 2;
			int offsetY = (Constants.ScreenHeight - Constants.FieldHeight) / 2;

			surface.SetData(new Color[] { color });
			Rectangle rect = new Rectangle(left + offsetX, top + offsetY, width, height);

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
