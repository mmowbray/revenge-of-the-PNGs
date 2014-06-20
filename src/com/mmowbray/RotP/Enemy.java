package com.mmowbray.RotP;

import java.util.ArrayList;
import java.util.Random;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer.ShapeType;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.math.Vector2;

public class Enemy
{
	static Texture enemyTexture;

	static ArrayList<TextureRegion> enemySpriteMapParts;

	static TextureRegion enemySpriteMapPart1;
	static TextureRegion enemySpriteMapPart2;
	static TextureRegion enemySpriteMapPart3;
	static TextureRegion enemySpriteMapPart4;

	Vector2 position;
	public Boolean alive;
	TextureRegion drawRectangle;
	public int health;
	public int speed;
	public Rectangle rectangle;
	static Random random;

	public Enemy(Vector2 position, TextureRegion drawRectangle, int health, int speed)
	{
		this.position = position;
		this.alive = true;
		this.speed = speed;
		this.drawRectangle = drawRectangle;
		this.health = health;
		this.rectangle = new Rectangle(position.x, position.y, drawRectangle.getRegionWidth(), drawRectangle.getRegionHeight());
	}
	
	public static void Initialize ()
	{
		enemyTexture = new Texture("enemySpriteMap.png");
		
		enemySpriteMapParts = new ArrayList<TextureRegion>();

		enemySpriteMapPart1 = new TextureRegion(enemyTexture, 0, 0, 25, 25);
		enemySpriteMapPart2 = new TextureRegion(enemyTexture, 25, 0, 21, 21);
		enemySpriteMapPart3 = new TextureRegion(enemyTexture, 46, 0, 17, 17);
		enemySpriteMapPart4 = new TextureRegion(enemyTexture, 63, 0, 13, 13);

		enemySpriteMapParts.add(enemySpriteMapPart1);
		enemySpriteMapParts.add(enemySpriteMapPart2);
		enemySpriteMapParts.add(enemySpriteMapPart3);
		enemySpriteMapParts.add(enemySpriteMapPart4);
		
		random = new Random();
	}

	public static Enemy newRandom()
	{

		int randomSpawnLocation = random.nextInt (4); //0 to 3 (inclusive)
		int randomEnemyType = random.nextInt (4);

		Vector2 spawnPosition = null;

		switch (randomSpawnLocation)
		{
			case 0:
				spawnPosition = new Vector2(0, Gdx.graphics.getHeight()); //top-left
				break;

			case 1:
				spawnPosition = new Vector2(Gdx.graphics.getWidth(), Gdx.graphics.getHeight()); //top-right
				break;

			case 2:
				spawnPosition = new Vector2(0, 0); //bottom-left
				break;

			case 3:
				spawnPosition = new Vector2(Gdx.graphics.getWidth(), 0); //bottom-right
				break;
		}
		
		Gdx.app.log("spawnPosition", "spawning at location: (" + spawnPosition.x + ", " + spawnPosition.y + "). [" + "location: " + randomSpawnLocation + ", type: " + randomEnemyType + "]");

		switch (randomEnemyType)
		{
			case 0:
				return new Enemy(spawnPosition, enemySpriteMapParts.get(0), 4, 1);

			case 1:
				return new Enemy(spawnPosition, enemySpriteMapParts.get(1), 3, 2);

			case 2:
				return new Enemy(spawnPosition, enemySpriteMapParts.get(2), 2, 3);

			case 3:
				return new Enemy(spawnPosition, enemySpriteMapParts.get(3), 1, 4);
		}

		return null;
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

	public void Draw(SpriteBatch spriteBatch, ShapeRenderer sr, boolean debug, BitmapFont smallUI/*, SpriteFont enemyFont*/)
	{
		if (alive)
		{
			spriteBatch.begin();
			spriteBatch.draw(drawRectangle, position.x, position.y);
			smallUI.draw(spriteBatch, "" + health, position.x + 4, position.y + rectangle.height - 4);

			//spriteBatch.DrawString(enemyFont, " " + health, position, Color.Black);
			spriteBatch.end();
		}
		
		if(debug)
		{
			sr.begin(ShapeType.Line);
			sr.setColor(Color.BLUE);
			sr.rect(position.x, position.y, rectangle.width, rectangle.height);
			sr.end();
		}
	}

	public void kill()
	{
		this.alive = false;
	}
	
	public boolean lostHealth(int damage) //returns true if hit kills enemy
	{
		if(health - damage < 1)
		{
			kill();
			return true;
		}
		else
		{
			health -= damage;
			return false;
		}
	}
}