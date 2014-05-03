using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Revenge_of_the_PNGs
{
	public class Enemy
    {
		static Texture2D enemyTexture;

		static Vector2 enemyPositionTL;
		static Vector2 enemyPositionTR;
		static Vector2 enemyPositionBL;
		static Vector2 enemyPositionBR;

		static List<Rectangle> enemySpriteMapParts;

		static Rectangle enemySpriteMapPart1;
		static Rectangle enemySpriteMapPart2;
		static Rectangle enemySpriteMapPart3;
		static Rectangle enemySpriteMapPart4;

		Vector2 position;
        public Boolean alive;
		Rectangle drawRectangle;
		public int health;
		int speed;
		public Rectangle rectangle;

		public static void Initialize (int screenHeight, int screenWidth, Texture2D enemySpriteMap)
		{
			enemySpriteMapParts = new List<Rectangle>();

			enemySpriteMapPart1 = new Rectangle(0, 0, 25, 25);
	        enemySpriteMapPart2 = new Rectangle(25, 0, 21, 21);
	        enemySpriteMapPart3 = new Rectangle(46, 0, 17, 17);
	        enemySpriteMapPart4 = new Rectangle(63, 0, 13, 13);

			enemySpriteMapParts.Add(enemySpriteMapPart1);
			enemySpriteMapParts.Add(enemySpriteMapPart2);
			enemySpriteMapParts.Add(enemySpriteMapPart3);
			enemySpriteMapParts.Add(enemySpriteMapPart4);

			enemyPositionTL = new Vector2(0,50);
	        enemyPositionTR = new Vector2(screenWidth - 50 , 50);
	        enemyPositionBL = new Vector2(50, screenHeight - 50);
	        enemyPositionBR = new Vector2(screenWidth - 50 , screenHeight - 50);

			enemyTexture = enemySpriteMap;

		}

        public Enemy(Vector2 position, Rectangle drawRectangle, int health, int speed)
        {
            this.position = position;
            this.alive = true;
            this.speed = speed;
            this.drawRectangle = drawRectangle;
            this.health = health;
			this.rectangle = new Rectangle((int)position.X, (int)position.Y, drawRectangle.Width, drawRectangle.Height);
        }

		public static Enemy newRandom()
        {

			Random random = new Random();

			int randomSpawnLocation = random.Next (0, 4);
			int randomEnemyType = random.Next (0, 4);

			Vector2 spawnPosition;

			switch (randomSpawnLocation)
			{
				case 0:
					spawnPosition = enemyPositionTL;
					break;

				case 1:
					spawnPosition = enemyPositionTR;
					break;

				case 2:
					spawnPosition = enemyPositionBL;
					break;

				case 3:
					spawnPosition = enemyPositionBR;
					break;
			}

			Enemy newEnemy = null;

			switch (randomEnemyType)
			{
				case 0:
					newEnemy = new Enemy(spawnPosition, enemySpriteMapParts[0], 4, 1);
					break;

				case 1:
					newEnemy = new Enemy(spawnPosition, enemySpriteMapParts[1], 3, 2);
					break;

				case 2:
					newEnemy = new Enemy(spawnPosition, enemySpriteMapParts[2], 2, 3);
					break;

				case 3:
					newEnemy = new Enemy(spawnPosition, enemySpriteMapParts[3], 1, 4);
					break;
			}

			return newEnemy;
        }

        public void Update(Player player)
        {
			if (alive)
			{
				Vector2 direction = new Vector2();

           		direction.X = player.position.X + ((Player.texture.Width / 21) / 2) - (position.X + rectangle.Width / 2);
                direction.Y = player.position.Y + (Player.texture.Height / 2) - (position.Y + rectangle.Height / 2);

                direction.Normalize();

                position += direction * speed;

				rectangle = new Rectangle((int)position.X, (int)position.Y, drawRectangle.Width, drawRectangle.Height);
			}            
        }

		public void Draw(SpriteBatch spriteBatch, SpriteFont enemyFont)
        {
			if (alive)
			{

				spriteBatch.Begin();
                spriteBatch.Draw(enemyTexture, position, drawRectangle, Color.White);

                spriteBatch.DrawString(enemyFont, " " + health, position, Color.Black);
                spriteBatch.End();

			}            
        }

        public void kill()
        {
            this.alive = false;
        }
    }
}