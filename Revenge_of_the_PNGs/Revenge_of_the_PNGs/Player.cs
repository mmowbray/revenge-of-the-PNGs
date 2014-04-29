using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Revenge_of_the_PNGs
{
	public class Player
	{
        public Texture2D playerTexture;
        public Vector2 playerPosition;
        public Boolean alive;
        public Boolean reloading;
        public int dX, dY;
        public int health;
        public int projectileCount;
        public int score;
        TimeSpan singleBulletReloadTime;

        public Player(Texture2D playerTexture, Vector2 playerPosition, int dX, int dY) //Initializes our player
        {
            this.playerTexture = playerTexture;
            this.playerPosition = playerPosition;
            this.alive = true; //new player should be alive
            this.dX = dX;
            this.dY = dY;
            this.health = 20;
            this.projectileCount = 10;
            this.singleBulletReloadTime = new TimeSpan(100);
            this.reloading = false;
            this.score = 0;
        }

        public void Draw(SpriteBatch spriteBatch, List<Rectangle> thePlayerSpriteMapParts)
        {
            if (this.alive)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(playerTexture, playerPosition, thePlayerSpriteMapParts[health], Color.White);
                spriteBatch.End();
            }
            else //draw the player in its dead state
            {
                spriteBatch.Begin();
                spriteBatch.Draw(playerTexture, playerPosition, thePlayerSpriteMapParts[0], Color.White);
                spriteBatch.End();
            }
        }

        public void reset()
        {
            this.health = 20;
            this.projectileCount = 10;
            this.alive = true;
        }

        public void kill()
        {
            this.alive = false;
        }
    }
}

