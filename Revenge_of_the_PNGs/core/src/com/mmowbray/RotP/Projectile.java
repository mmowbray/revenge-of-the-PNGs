package com.mmowbray.RotP;

import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.math.Vector2;

public class Projectile
{
	static Texture texture;
	//static SoundEffect gunFire;
	static int speed = 9;

	Vector2 position;
	public Boolean active;
	public Rectangle rectangle;
	int dX, dY;

	public Projectile(Vector2 position, int dX, int dY)
	{
		this.position = position;
		this.active = true;
		this.rectangle = new Rectangle(position.x, position.y, texture.getWidth(), texture.getHeight());
		this.dX = dX;
		this.dY = dY;
	}

	public static void Initialize (Texture projectileTexture/*, SoundEffect gunFireSound*/)
	{
		texture = projectileTexture;
		//gunFire = gunFireSound;
	}

	public static Projectile fire(int direction, Player player)
	{

		Projectile newProjectile = null;

		switch (direction)
		{
			case 0:
				newProjectile = new Projectile (new Vector2 (player.position.x + (Player.texture.getWidth() / 42) - (texture.getWidth() / 2), player.position.y - texture.getHeight()), 0, -1);
				break;

			case 1:
				newProjectile = new Projectile (new Vector2 (player.position.x + (Player.texture.getWidth()/ 21), player.position.y + (Player.texture.getHeight() / 2) - (texture.getHeight() / 2)), 1, 0);
				break;

			case 2:
				newProjectile = new Projectile (new Vector2 (player.position.x + (Player.texture.getWidth() / 42) - (texture.getWidth() / 2), player.position.y + Player.texture.getHeight()), 0, 1);
				break;

			case 3:
				newProjectile = new Projectile (new Vector2 (player.position.x - texture.getWidth(), player.position.y + (Player.texture.getHeight() / 2) - (texture.getHeight() / 2)), -1, 0);
				break;
		}

		//gunFire.Play ();
		return newProjectile;

	}

	public void Update()
	{

		if (active)
		{
			position.x += dX * speed;
			position.y += dY * speed;

			rectangle = new Rectangle(position.x, position.y, texture.getWidth(), texture.getHeight());
		}

	}

	public void Draw(SpriteBatch spriteBatch)
	{
		if (active)
		{
			spriteBatch.begin();
			spriteBatch.draw(texture, position.x, position.y);
			spriteBatch.end();
		}
	}

	public void kill()
	{
		this.active = false;
	}
}