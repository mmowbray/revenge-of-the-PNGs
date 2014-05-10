package com.mmowbray.RotP;

import java.util.ArrayList;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input.Keys;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.utils.TimeUtils;

public class Game extends ApplicationAdapter
{
	SpriteBatch spriteBatch;

	Texture playerTexture;
	Texture enemyTexture;
	Texture projectileTexture;
	Texture projectileIndicator;

	//SoundEffect gunFire;

	ArrayList<Enemy> enemies;
	ArrayList<Projectile> projectiles;
	Player player1;

	enum GameState { Normal, Intermission, GameOver };
	GameState currentGameState;

	int wave, toSpawn, toKill;

	long lastShot;
	long fireDelay;

	long lastMonster;
	long monsterDelay;

	long intermissionStart;
	long intermissionLength;

	long reloadStart;
	long reloadTime;
	
	long gameStartTime;
	long gameTime;
	
	BitmapFont font;

	@Override
	public void create ()
	{
		spriteBatch = new SpriteBatch();
		
		playerTexture = new Texture("playerSpriteMap.png");
		enemyTexture = new Texture("enemySpriteMap.png");
		projectileTexture = new Texture("projectile.png");
		projectileIndicator = new Texture("projectileSpriteMap.png");		

		//gunFire = Content.Load<SoundEffect>("projectileFire.xnb");

		Enemy.Initialize(Gdx.graphics.getHeight(), Gdx.graphics.getWidth(), enemyTexture);
		Player.Initialize(playerTexture, projectileIndicator);
		Projectile.Initialize(projectileTexture/*, gunFire*/);

		//enemyDeath = Content.Load<SoundEffect>("enemyDeath");

		enemies = new ArrayList<Enemy>();
		projectiles = new ArrayList<Projectile>();

		intermissionLength = 5000; //5 seconds
		monsterDelay = 1000; //1 second
		reloadTime = 2000; //2 seconds
		fireDelay = 160; //160 ms			
		
		gameStartTime = TimeUtils.millis();
		gameTime = 0;
		
		font = new BitmapFont(Gdx.files.internal("uiFont.fnt"), false);
		
		StartNewGame();
	}

	@Override
	public void render ()
	{
		//update gameTime
		gameTime = TimeUtils.timeSinceMillis(gameStartTime);

		//check+update game state

		if (!player1.alive) { //player was killed
			currentGameState = GameState.GameOver;
		}

		if (toKill == 0) { //player killed all monsters
			intermissionStart = gameTime;
			currentGameState = GameState.Intermission;
			player1.refill();//replenishes health and ammo

			wave++;
			toSpawn = 6 + wave * 3;
			toKill = toSpawn;

			lastMonster = gameTime;

			enemies.clear ();
			projectiles.clear ();
		}

		if (currentGameState == GameState.Intermission && gameTime - intermissionStart > intermissionLength) { //intermission is over
			currentGameState = GameState.Normal;
		}

		if (currentGameState == GameState.GameOver)
		{

			if (Gdx.input.isKeyPressed(Keys.R)) {
				StartNewGame ();
				intermissionStart = gameTime;
			}

			if (Gdx.input.isKeyPressed(Keys.Q)) {
				System.exit(0);
			}
		}

		//move+manipulate objects

		if (currentGameState == GameState.Normal && toSpawn > 0 && gameTime - lastMonster > monsterDelay) { //add enemy

			enemies.add (Enemy.newRandom ());
			lastMonster = gameTime;
			toSpawn--;
		}

		if (!player1.reloading && player1.projectileCount < 2)
		{

			player1.reloading = true;
			reloadStart = gameTime;

		}

		if (player1.reloading && gameTime - reloadStart > reloadTime)
		{

			player1.reloading = false;
			player1.projectileCount = 10;

		}

		if (!player1.reloading && gameTime - lastShot > fireDelay && currentGameState == GameState.Normal)
		{
			if (Gdx.input.isKeyPressed(Keys.W)) {
				projectiles.add (Projectile.fire (0, player1));
				player1.projectileCount--;
				lastShot = gameTime;
			}

			else if (Gdx.input.isKeyPressed(Keys.A)) {
				projectiles.add (Projectile.fire (3, player1));
				player1.projectileCount--;
				lastShot = gameTime;
			}

			else if (Gdx.input.isKeyPressed(Keys.S)) {
				projectiles.add (Projectile.fire (2, player1));
				player1.projectileCount--;
				lastShot = gameTime;
			}

			else if (Gdx.input.isKeyPressed(Keys.D)) {
				projectiles.add (Projectile.fire (1, player1));
				player1.projectileCount--;
				lastShot = gameTime;
			}
		}

		for (int i = 0; i < projectiles.size(); i++) {
			projectiles.get(i).Update ();
		}

		player1.Update (Gdx.graphics.getHeight(), Gdx.graphics.getWidth());

		for (int i = 0; i < enemies.size(); i++) {
			enemies.get(i).Update (player1);
		}

		//collision detection

		for (int i = 0; i < enemies.size(); i++)
		{
			for (int j = 0; j < projectiles.size(); j++)
			{
				if (enemies.get(i).rectangle.overlaps (projectiles.get(j).rectangle) && enemies.get(i).alive && projectiles.get(j).active)
				{
					enemies.get(i).kill ();
					toKill--;
					projectiles.get(j).kill ();
				}
			}

			if (enemies.get(i).rectangle.overlaps (player1.rectangle) && enemies.get(i).alive)
			{
				enemies.get(i).kill ();
				toKill--;
				player1.health -= enemies.get(i).health;
			}
		}

		//remove dead elements

		for (int i = 0; i < projectiles.size(); i++) { //worry about off-screen projectiles in future
			if (!projectiles.get(i).active) {
				projectiles.remove(i);
			}
		}

		for (int i = 0; i < enemies.size(); i++) {
			if (!enemies.get(i).alive) {
				enemies.remove (i);
			}
		}

		if (player1.health < 1) {
			player1.kill ();
		}

		//draw 
		
		Gdx.gl.glClearColor(0,0,0,0); //black, would prefer grey
		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

		player1.Draw (spriteBatch/* font*/);

		for (int i = 0; i < enemies.size(); i++) {
			enemies.get(i).Draw (spriteBatch/*, enemyFont*/);
		}

		for (int i = 0; i < projectiles.size(); i++) {
			projectiles.get(i).Draw (spriteBatch);
		}

		spriteBatch.begin();
		
		switch (currentGameState)
		{
			case Normal:
			{				
				font.draw(spriteBatch, "Enemies:" + toKill, Gdx.graphics.getWidth() / 2 - 80, Gdx.graphics.getHeight() - 5);
				break;
			}

			case Intermission:
			{
				font.draw(spriteBatch, "WAVE INCOMING!", Gdx.graphics.getWidth() / 2 - 120, Gdx.graphics.getHeight() - 5);
				break;
			}

			case GameOver:
			{
				font.draw(spriteBatch, "GAME OVER! Press R to restart or Q to quit", player1.position.x, player1.position.y);
				break;
			}
		}
		
		font.draw(spriteBatch, "Wave: " + wave, 5, Gdx.graphics.getHeight() - 5);
		
		spriteBatch.end();

	}
	
	public void StartNewGame ()
	{
		currentGameState = GameState.Intermission;
		player1 = new Player(new Vector2(((Gdx.graphics.getWidth() - (playerTexture.getWidth() / 21)) / 2), ((Gdx.graphics.getHeight() - playerTexture.getHeight()) / 2)), 7);

		enemies.clear();
		projectiles.clear();

		wave = 1;
		toSpawn = 6 + wave * 3;
		toKill = toSpawn;
	}
}
