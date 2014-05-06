using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Revenge_of_the_PNGs
{
	public class Player
	{
		static List<Rectangle> playerSpriteMapParts;
		static List<Rectangle> projectileIndicatorSpriteMapParts;
		public static Texture2D texture;
		public static Texture2D overlay;
		//static TimeSpan singleBulletReloadTime;

		public Vector2 position;
		public Vector2 overlayPos;
		public Boolean alive;
		public Boolean reloading;
		public Rectangle rectangle;
		public int health;
		public int projectileCount;
		public int score;
		public int speed;

		public Player(Vector2 position, int speed) //non-static player constructor
		{
			this.position = position;
			this.rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width / 21, texture.Height);
			this.overlayPos = new Vector2(position.X + 24, position.Y + 2);
			this.alive = true;
			this.reloading = false;
			this.health = 20;
			this.projectileCount = 10;
			this.score = 0;
			this.speed = speed;
		}

		public static void Initialize (Texture2D playerTexture, Texture2D projectileIndicator)
		{

			texture = playerTexture;
			overlay = projectileIndicator;

			//singleBulletReloadTime = new TimeSpan(100);

			playerSpriteMapParts = new List<Rectangle>();

			for (int i = 0; i < 21; i++) //there are 21 different spritemap parts for the player in playerSpriteMap.png
			{
				Rectangle newSpriteMapPart = new Rectangle(i * 34, 0, 34, 34);
				playerSpriteMapParts.Add(newSpriteMapPart);
			}

			projectileIndicatorSpriteMapParts = new List<Rectangle>();

			for (int i = 0; i < 10; i++) //initialize all the bullet sprite map parts
			{
				Rectangle newProjectileSpriteMapPart = new Rectangle(i * 8, 0, 8, 30);
				projectileIndicatorSpriteMapParts.Add(newProjectileSpriteMapPart);
			}

		}

		public void Update(KeyboardState keyboard, int screenHeight, int screenWidth)
		{

			if (keyboard.IsKeyDown(Keys.Up))
			{
				position.Y -= speed;
			}

			if (keyboard.IsKeyDown(Keys.Left))
			{
				position.X -= speed;
			}

			if (keyboard.IsKeyDown(Keys.Down))
			{
				position.Y += speed;
			}

			if (keyboard.IsKeyDown(Keys.Right))
			{
				position.X += speed;
			}

			position.X = MathHelper.Clamp(position.X, 0, screenWidth - (texture.Width / 21));
			position.Y = MathHelper.Clamp(position.Y, 0, screenHeight - texture.Height);

			rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width / 21, texture.Height);

			overlayPos.X = position.X + 24;
			overlayPos.Y = position.Y + 2;

		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont smallFont)
		{
			spriteBatch.Begin();

			if (alive)
			{
				spriteBatch.Draw(texture, position, playerSpriteMapParts[health], Color.White);
				spriteBatch.Draw(overlay, overlayPos, projectileIndicatorSpriteMapParts[projectileCount - 1], Color.White);

				if(reloading)
				{
					spriteBatch.DrawString (smallFont, "Reloading!", new Vector2 (position.X - 30, position.Y - 30), new Color (89,89,171));
				}

			}
			else
			{
				spriteBatch.Draw(texture, position, playerSpriteMapParts[0], Color.White);
			}

			spriteBatch.End();
		}

		public void reset()
		{
			this.alive = true;
			this.health = 20;
			this.projectileCount = 10;
		}

		public void kill()
		{
			this.alive = false;
		}
	}
}