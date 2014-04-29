using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Revenge_of_the_PNGs
{
	public class Enemy
    {
        public Texture2D enemyTexture; 
        public Vector2 enemyPosition;
        public Boolean alive;
        public int enemyMoveSpeed;
        public Rectangle currEnemyRect;
        public int enemyHealth;

        public Enemy(Texture2D enemyTexture, Vector2 enemyPosition, int enemyMoveSpeed, Rectangle currEnemyRect, int enemyHealth) //Initializes a generic enemy
        {
            this.enemyTexture = enemyTexture;
            this.enemyPosition = enemyPosition;
            this.alive = true; //new enemy should be alive
            this.enemyMoveSpeed = enemyMoveSpeed;
            this.currEnemyRect = currEnemyRect;
            this.enemyHealth = enemyHealth;
        }

        public void Update(Vector2 playerPosition, Texture2D enemyTexture, Texture2D playerTexture)
        {
            if (alive)
            {
                Vector2 enemyDirection = new Vector2();
                enemyDirection.X = playerPosition.X + ((playerTexture.Width / 21) / 2) - (enemyPosition.X + currEnemyRect.Width / 2);
                enemyDirection.Y = playerPosition.Y + (playerTexture.Height / 2) - (enemyPosition.Y + currEnemyRect.Height / 2);

                enemyDirection.Normalize();

                enemyPosition += enemyDirection * enemyMoveSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont enemyHealthSpriteFont)
        {
            if (alive)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(enemyTexture, enemyPosition, currEnemyRect, Color.White);

                spriteBatch.DrawString(enemyHealthSpriteFont, " " + enemyHealth, enemyPosition, Color.Black);
                spriteBatch.End();
            }
        }

        public void killEnemy()
        {
            this.alive = false;
        }
    }
}

