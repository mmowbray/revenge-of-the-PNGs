using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Revenge_of_the_PNGs
{
	public class Projectile
	{
		static Texture2D texture;
		static SoundEffect gunFire;
		static int speed = 5;

		Vector2 position;
		public Boolean active;
		public Rectangle rectangle;
		int dX, dY;

		public Projectile(Vector2 position, int dX, int dY)
		{
			this.position = position;
			this.active = true;
			this.rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
			this.dX = dX;
			this.dY = dY;
		}

		public static void Initialize (Texture2D projectileTexture, SoundEffect gunFireSound)
		{
			texture = projectileTexture;
			gunFire = gunFireSound;
		}

		public static Projectile fire(int direction, Player player)
		{

			Projectile newProjectile = null;

			switch (direction)
			{
				case 0:
					newProjectile = new Projectile (new Vector2 (player.position.X + (Player.texture.Width / 42) - (texture.Width / 2), player.position.Y - texture.Height), 0, -1);
					break;

				case 1:
					newProjectile = new Projectile (new Vector2 (player.position.X + (Player.texture.Width/ 21), player.position.Y + (Player.texture.Height / 2) - (texture.Height / 2)), 1, 0);
					break;

				case 2:
					newProjectile = new Projectile (new Vector2 (player.position.X + (Player.texture.Width / 42) - (texture.Width / 2), player.position.Y + Player.texture.Height), 0, 1);
					break;

				case 3:
					newProjectile = new Projectile (new Vector2 (player.position.X - texture.Width, player.position.Y + (Player.texture.Height / 2) - (texture.Height / 2)), -1, 0);
					break;
			}

			gunFire.Play ();
			return newProjectile;

		}

		public void Update()
		{

			if (active)
			{
				position.X += dX * speed;
				position.Y += dY * speed;

				rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
			}

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (active)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(texture, position, Color.White);
				spriteBatch.End();
			}
		}

		public void kill()
		{
			this.active = false;
		}
	}
}