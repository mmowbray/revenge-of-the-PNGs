package com.mmowbray.RotP;

import java.util.ArrayList;
import java.util.Random;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;

public class Enemy
{
	static Texture enemyTexture;

	static Vector2 enemyPositionTL;
	static Vector2 enemyPositionTR;
	static Vector2 enemyPositionBL;
	static Vector2 enemyPositionBR;

	static ArrayList<TextureRegion> enemySpriteMapParts;

	static TextureRegion enemySpriteMapPart1;
	static TextureRegion enemySpriteMapPart2;
	static TextureRegion enemySpriteMapPart3;
	static TextureRegion enemySpriteMapPart4;

	Vector2 position;
	public Boolean alive;
	TextureRegion drawRectangle;
	public int health;
	int speed;
	public Rectangle rectangle;

	public static void Initialize (int screenHeight, int screenWidth, Texture enemySpriteMap)
	{
		enemyTexture = enemySpriteMap;
		
		enemySpriteMapParts = new ArrayList<TextureRegion>();

		enemySpriteMapPart1 = new TextureRegion(enemyTexture, 0, 0, 25, 25);
		enemySpriteMapPart2 = new TextureRegion(enemyTexture, 25, 0, 21, 21);
		enemySpriteMapPart3 = new TextureRegion(enemyTexture, 46, 0, 17, 17);
		enemySpriteMapPart4 = new TextureRegion(enemyTexture, 63, 0, 13, 13);

		enemySpriteMapParts.add(enemySpriteMapPart1);
		enemySpriteMapParts.add(enemySpriteMapPart2);
		enemySpriteMapParts.add(enemySpriteMapPart3);
		enemySpriteMapParts.add(enemySpriteMapPart4);

		enemyPositionTL = new Vector2(0,50);
		enemyPositionTR = new Vector2(screenWidth - 50 , 50);
		enemyPositionBL = new Vector2(50, screenHeight - 50);
		enemyPositionBR = new Vector2(screenWidth - 50 , screenHeight - 50);	

	}

	public Enemy(Vector2 position, TextureRegion drawRectangle, int health, int speed)
	{
		this.position = position;
		this.alive = true;
		this.speed = speed;
		this.drawRectangle = drawRectangle;
		this.health = health;
		this.rectangle = new Rectangle(position.x, position.y, drawRectangle.getRegionWidth(), drawRectangle.getRegionHeight());
	}

	public static Enemy newRandom()
	{

		Random random = new Random();

		int randomSpawnLocation = random.nextInt (4); //0 to 3 (inclusive)
		int randomEnemyType = random.nextInt (4);

		Vector2 spawnPosition = null;

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
				newEnemy = new Enemy(spawnPosition, enemySpriteMapParts.get(0), 4, 1);
				break;

			case 1:
				newEnemy = new Enemy(spawnPosition, enemySpriteMapParts.get(1), 3, 2);
				break;

			case 2:
				newEnemy = new Enemy(spawnPosition, enemySpriteMapParts.get(2), 2, 3);
				break;

			case 3:
				newEnemy = new Enemy(spawnPosition, enemySpriteMapParts.get(3), 1, 4);
				break;
		}

		return newEnemy;
	}

	public void Update(Player player)
	{
		if (alive)
		{
			Vector2 direction = new Vector2();

			direction.x = player.position.x + ((Player.texture.getWidth() / 21) / 2) - (position.x + rectangle.width / 2);
			direction.y = player.position.y + (Player.texture.getHeight() / 2) - (position.y + rectangle.height / 2);
			
			direction.nor(); //normalizes

			position.x += direction.x * speed;
			position.y += direction.y * speed;

			rectangle = new Rectangle(position.x, position.y, drawRectangle.getRegionWidth(), drawRectangle.getRegionHeight());
		}
	}

	public void Draw(SpriteBatch spriteBatch/*, SpriteFont enemyFont*/)
	{
		if (alive)
		{
			spriteBatch.begin();
			spriteBatch.draw(drawRectangle, position.x, position.y);

			//spriteBatch.DrawString(enemyFont, " " + health, position, Color.Black);
			spriteBatch.end();
		}
	}

	public void kill()
	{
		this.alive = false;
	}
}
