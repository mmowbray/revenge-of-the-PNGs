package com.mmowbray.RotP;

import java.util.ArrayList;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input.Keys;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.glutils.ShapeRenderer;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.utils.TimeUtils;

public class Game extends ApplicationAdapter
{
	SpriteBatch spriteBatch;

	//SoundEffect gunFire;

	ArrayList<Enemy> enemies;
	ArrayList<Projectile> projectiles;
	Player player1;

	enum GameState { Normal, Intermission, GameOver };
	GameState currentGameState;

	int wave, toSpawn, toKill;

	long lastShot, fireDelay;

	long lastMonster; //last time a monster was spawned
	long monsterDelay; //the delay between spawning a new monster

	long intermissionStart; //last time an intermission was started
	long intermissionLength; //the length of an intermission

	long reloadStart; //last time the player reloaded
	long reloadTime; //the reloading delay
	
	long gameStartTime; //the time the game started
	long gameTime; //the elapsed time since the game was started
	
	long lastPress; //when debug button was last pressed
	long pressDelay; //delay for toggling debug mode
	
	boolean debug;
	
	BitmapFont font, smallUI;
	Texture instructions;
	ShapeRenderer sr;

	@Override
	public void create ()
	{
		spriteBatch = new SpriteBatch();		

		//gunFire = Content.Load<SoundEffect>("projectileFire.xnb");

		Enemy.Initialize();
		Player.Initialize();
		Projectile.Initialize(/*gunFire*/);

		//enemyDeath = Content.Load<SoundEffect>("enemyDeath");

		enemies = new ArrayList<Enemy>();
		projectiles = new ArrayList<Projectile>();

		intermissionLength = 5500; //5.5 seconds
		monsterDelay = 1500; //1.5 seconds
		reloadTime = 1400; //1.4 seconds
		fireDelay = 160; //160 ms
		pressDelay = 250; //250 ms
		
		debug = false;
		
		font = new BitmapFont(Gdx.files.internal("uiFont.fnt"), false);
		smallUI = new BitmapFont(Gdx.files.internal("smallUI.fnt"), false);
		
		instructions = new Texture("instructions.png");
		
		sr = new ShapeRenderer();
		sr.setColor(Color.BLUE);
		
		StartNewGame();
	}

	@Override
	public void render ()
	{
		//update gameTime + debug mode
		
		gameTime = TimeUtils.timeSinceMillis(gameStartTime);
		
		if (gameTime - lastPress > pressDelay && Gdx.input.isKeyPressed(Keys.SPACE)){
			debug = !debug; //toggle debug
			lastPress = gameTime;
		}

		//check+update game state
		
		if (!player1.alive) { //player was killed
			player1.kill();
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
				System.exit(0); //must be a more generic method (GDX)
			}
		}
		
		//collision detection
				
		for (int i = 0; i < enemies.size(); i++)
		{
			
			if (enemies.get(i).rectangle.overlaps (player1.rectangle)) //enemy collides with player
			{
				enemies.get(i).kill();
				toKill--;
				player1.lostHealth(enemies.get(i).health);
			}		

			if (enemies.get(i).alive) //wasn't previously killed by colliding with player
			{
				for (int j = 0; j < projectiles.size(); j++) //check against all projectiles
				{
					if (enemies.get(i).rectangle.overlaps (projectiles.get(j).rectangle))
					{
						enemies.get(i).lostHealth(1);
						projectiles.get(j).kill ();
					}
				}
				
				if(!enemies.get(i).alive)
				{
					toKill--;
				}
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

		//draw 
		
		Gdx.gl.glClearColor(0.75f,0.75f,0.75f,1f); // grey
		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

		player1.Draw (spriteBatch, sr, debug/* font*/);

		for (int i = 0; i < enemies.size(); i++) {
			enemies.get(i).Draw (spriteBatch, sr, debug, smallUI/*, enemyFont*/);
		}

		for (int i = 0; i < projectiles.size(); i++) {
			projectiles.get(i).Draw (spriteBatch, sr, debug);
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
				
				if(wave == 1)
				{
					spriteBatch.draw(instructions, (Gdx.graphics.getWidth() - instructions.getWidth()) / 2, (Gdx.graphics.getHeight() - instructions.getHeight()) / 2);
				}
				
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
		player1 = new Player(new Vector2(((Gdx.graphics.getWidth() - (Player.texture.getWidth() / 21)) / 2), ((Gdx.graphics.getHeight() - Player.texture.getHeight()) / 2)), 7);

		enemies.clear();
		projectiles.clear();

		wave = 1;
		toSpawn = 2 * wave + 6;
		toKill = toSpawn;
		lastMonster = 0;
		
		gameStartTime = TimeUtils.millis();
		gameTime = 0;
		lastPress = gameTime - pressDelay;
	}
}
