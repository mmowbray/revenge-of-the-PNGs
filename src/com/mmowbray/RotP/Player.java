package com.mmowbray.RotP;

import java.util.ArrayList;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input.Keys;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.math.MathUtils;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.math.Rectangle;

public class Player
{
	
	static ArrayList<TextureRegion> playerSpriteMapParts;
	static ArrayList<TextureRegion> projectileIndicatorSpriteMapParts;
	public static Texture texture;
	public static Texture overlay;
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
		this.rectangle = new Rectangle(position.x, position.x, texture.getWidth() / 21, texture.getHeight());
		this.overlayPos = new Vector2(position.x + 24, position.y + 2);
		this.alive = true;
		this.reloading = false;
		this.health = 20;
		this.projectileCount = 10;
		this.score = 0;
		this.speed = speed;
	}

	public static void Initialize ()
	{

		texture = new Texture("playerSpriteMap.png");		
		overlay = new Texture("projectileSpriteMap.png");	

		//singleBulletReloadTime = new TimeSpan(100);

		playerSpriteMapParts = new ArrayList<TextureRegion>();

		for (int i = 0; i < 21; i++) //there are 21 different spritemap parts for the player in playerSpriteMap.png
		{
			
			TextureRegion newSpriteMapPart = new TextureRegion(texture, i * 34, 0, 34, 34);
			
			playerSpriteMapParts.add(newSpriteMapPart);
		}

		projectileIndicatorSpriteMapParts = new ArrayList<TextureRegion>();

		for (int i = 0; i < 10; i++) //initialize all the bullet sprite map parts
		{
			TextureRegion newProjectileSpriteMapPart = new TextureRegion(overlay, i * 8, 0, 8, 30);
			projectileIndicatorSpriteMapParts.add(newProjectileSpriteMapPart);
		}

	}

	public void Update(int screenHeight, int screenWidth)
	{

		if (Gdx.input.isKeyPressed(Keys.UP))
		{
			position.y += speed;
		}

		if (Gdx.input.isKeyPressed(Keys.LEFT))
		{
			position.x -= speed;
		}

		if (Gdx.input.isKeyPressed(Keys.DOWN))
		{
			position.y -= speed;
		}

		if (Gdx.input.isKeyPressed(Keys.RIGHT))
		{
			position.x += speed;
		}

		position.x = MathUtils.clamp(position.x, 0, screenWidth - (texture.getWidth() / 21));
		position.y = MathUtils.clamp(position.y, 0, screenHeight - texture.getHeight());	

		rectangle = new Rectangle(position.x, position.y, texture.getWidth() / 21, texture.getHeight());

		overlayPos.x = position.x + 24;
		overlayPos.y = position.y + 2;

	}

	public void Draw(SpriteBatch spriteBatch /*,SpriteFont smallFont*/)
	{
		spriteBatch.begin();

		if (alive)
		{
			spriteBatch.draw(playerSpriteMapParts.get(health), position.x, position.y);
			spriteBatch.draw(projectileIndicatorSpriteMapParts.get(projectileCount - 1), overlayPos.x, overlayPos.y);

			if(reloading)
			{
				//spriteBatch.DrawString (smallFont, "Reloading!", new Vector2 (position.x - 30, position.y - 30), new Color (89,89,171, 1));
			}

		}
		else
		{
			spriteBatch.draw(playerSpriteMapParts.get(0), position.x, position.y);
		}

		spriteBatch.end();
	}

	public void reset()
	{
		this.alive = true;
		this.health = 20;
		this.projectileCount = 10;
	}
	public void refill()
	{
		health = 20;
		reloading = false;
		projectileCount = 10;
	}
	public void kill()
	{
		alive = false;
	}

}
