using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;

namespace Revenge_of_the_PNGs
{
	/// <summary>
	///
	/// Revenge of the PNGs v1.0
	/// XNA Game by Maxwell Mowbray
	///
	///About:
	///
	///the .pngs have revolted and you must now survive wave after wave of their onslaught! Enemies will try to approach and kill you.
	///Your only defense is your rifle, and armor.
	///Each wave, the number of enemies will increase.
	///Between each wave, health and armor will be replenished. During the wave, an enemy kill will increase your health by one, but you cannot repair your
	///armor during the wave.
	///You must survive each wave to increase your score. Enemy type and spawn location is randomized. If you die, you will be asked to
	///restart or quit. Good luck!
	///
	///Controls:
	///
	///Player movement is controlled using the arrow keys, and firing is done with the WASD keys.
	///
	/// </summary>
	///

	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;

		SpriteBatch spriteBatch;

		Texture2D playerTexture;
		Texture2D enemyTexture;
		Texture2D projectileTexture;
		Texture2D projectileIndicator;

		SoundEffect gunFire;

		SpriteFont font;
		SpriteFont enemyFont;

		List<Enemy> enemies;
		List<Projectile> projectiles;
		Player player1;

		enum GameState { Normal, Intermission, GameOver };
		GameState currentGameState;

		int wave, toSpawn, toKill;

		TimeSpan lastShot;
		TimeSpan fireDelay;

		TimeSpan lastMonster;
		TimeSpan monsterDelay;

		TimeSpan intermissionStart;
		TimeSpan intermissionLength;

		TimeSpan reloadStart;
		TimeSpan reloadTime;

		public Game1 ()
		{
			Content.RootDirectory = "Content";
			this.Window.Title = "Revenge of the PNGs";
			graphics = new GraphicsDeviceManager (this);
			//set screen resolution to 800 x 480 in the future
		}

		protected override void Initialize ()
		{
			base.Initialize();
		}

		protected override void LoadContent ()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			playerTexture = Content.Load<Texture2D>("playerSpriteMap");
			enemyTexture = Content.Load<Texture2D>("enemySpriteMap");
			projectileTexture = Content.Load<Texture2D>("projectile");
			projectileIndicator = Content.Load<Texture2D>("projectileSpriteMap"); ;

			gunFire = Content.Load<SoundEffect>("projectileFire.xnb");

			font = Content.Load<SpriteFont>("UIFont.xnb");
			enemyFont = Content.Load<SpriteFont>("enemyHealthSpriteFont.xnb");

			Enemy.Initialize(GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, enemyTexture);
			Player.Initialize(playerTexture, projectileIndicator);
			Projectile.Initialize(projectileTexture, gunFire);

			//enemyDeath = Content.Load<SoundEffect>("enemyDeath");

			enemies = new List<Enemy>();
			projectiles = new List<Projectile>();

			intermissionLength = TimeSpan.FromSeconds(5);
			monsterDelay = TimeSpan.FromSeconds(1);
			reloadTime = TimeSpan.FromSeconds(2);
			fireDelay = TimeSpan.FromMilliseconds(200);

			StartNewGame();
		}

		protected override void Update (GameTime gameTime)
		{
			//read input

			KeyboardState keyboard = Keyboard.GetState ();

			//check+update game state

			if (!player1.alive) { //player was killed
				currentGameState = GameState.GameOver;
			}

			if (toKill == 0) { //player killed all monsters
				currentGameState = GameState.Intermission;
				intermissionStart = gameTime.TotalGameTime;
				player1.health = 20;

				wave++;
				toSpawn = 6 + wave * 3;
				toKill = toSpawn;

				lastMonster = gameTime.TotalGameTime;

				enemies.Clear ();
				projectiles.Clear ();
			}

			if (currentGameState == GameState.Intermission && gameTime.TotalGameTime - intermissionStart > intermissionLength) { //intermission is over
				currentGameState = GameState.Normal;
			}

			if (currentGameState == GameState.GameOver)
			{

				if (keyboard.IsKeyDown (Keys.R)) {
					StartNewGame ();
					intermissionStart = gameTime.TotalGameTime;
				}

				if (keyboard.IsKeyDown (Keys.Q)) {
					Exit ();
				}

			}

			//move+manipulate objects

			if (currentGameState == GameState.Normal && toSpawn > 0 && gameTime.TotalGameTime - lastMonster > monsterDelay) { //add enemy

				enemies.Add (Enemy.newRandom ());
				lastMonster = gameTime.TotalGameTime;
				toSpawn--;
			}

			if (!player1.reloading && player1.projectileCount < 2)
			{

				player1.reloading = true;
				reloadStart = gameTime.TotalGameTime;

			}

			if (player1.reloading && gameTime.TotalGameTime - reloadStart > reloadTime)
			{

				player1.reloading = false;
				player1.projectileCount = 10;

			}

			if (!player1.reloading && gameTime.TotalGameTime - lastShot > fireDelay && currentGameState == GameState.Normal)
			{
				if (keyboard.IsKeyDown (Keys.W)) {
					projectiles.Add (Projectile.fire (0, player1));
					player1.projectileCount--;
					lastShot = gameTime.TotalGameTime;
				}

				else if (keyboard.IsKeyDown (Keys.A)) {
					projectiles.Add (Projectile.fire (3, player1));
					player1.projectileCount--;
					lastShot = gameTime.TotalGameTime;
				}

				else if (keyboard.IsKeyDown (Keys.S)) {
					projectiles.Add (Projectile.fire (2, player1));
					player1.projectileCount--;
					lastShot = gameTime.TotalGameTime;
				}

				else if (keyboard.IsKeyDown (Keys.D)) {
					projectiles.Add (Projectile.fire (1, player1));
					player1.projectileCount--;
					lastShot = gameTime.TotalGameTime;
				}
			}

			for (int i = 0; i < projectiles.Count; i++) {
				projectiles [i].Update ();
			}

			player1.Update (keyboard, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);

			for (int i = 0; i < enemies.Count; i++) {
				enemies [i].Update (player1);
			}

			//collision detection

			for (int i = 0; i < enemies.Count; i++)
			{
				for (int j = 0; j < projectiles.Count; j++)
				{
					if (enemies [i].rectangle.Intersects (projectiles [j].rectangle) && enemies [i].alive && projectiles [j].active) {
						enemies [i].kill ();
						toKill--;
						projectiles [j].kill ();
					}
				}

				if (enemies [i].rectangle.Intersects (player1.rectangle) && enemies [i].alive)
				{
					enemies [i].kill ();
					toKill--;
					player1.health -= enemies [i].health;
				}
			}

			//remove dead elements

			for (int i = 0; i < projectiles.Count; i++) { //worry about off-screen projectiles in future
				if (!projectiles [i].active) {
					projectiles.RemoveAt (i);
				}
			}

			for (int i = 0; i < enemies.Count; i++) {
				if (!enemies [i].alive) {
					enemies.RemoveAt (i);
				}
			}

			if (player1.health < 1) {
				player1.kill ();
			}

			base.Update(gameTime);
		}

		public void StartNewGame ()
		{
			currentGameState = GameState.Intermission;
			player1 = new Player(new Vector2(((GraphicsDevice.Viewport.Width - (playerTexture.Width / 21)) / 2), ((GraphicsDevice.Viewport.Height - playerTexture.Height) / 2)), 6);

			enemies.Clear();
			projectiles.Clear();

			wave = 1;
			toSpawn = 6 + wave * 3;
			toKill = toSpawn;
		}

		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (new Color (225, 225, 225)); //light grey

			player1.Draw (spriteBatch, font);

			for (int i = 0; i < enemies.Count; i++) {
				enemies [i].Draw (spriteBatch, enemyFont);
			}

			for (int i = 0; i < projectiles.Count; i++) {
				projectiles [i].Draw (spriteBatch);
			}

			switch (currentGameState)
			{
				case GameState.Normal:
				{
					//spriteBatch.DrawString (font, "tokill:" + toKill, new Vector2 (GraphicsDevice.Viewport.Width / 2 - 80, 50), Color.DarkRed); //used previously for debugging
					//spriteBatch.DrawString (font, "tospawn:" + toSpawn, new Vector2 (GraphicsDevice.Viewport.Width / 2 - 80, 90), Color.DarkRed);
					break;
				}

				case GameState.Intermission:
				{
					spriteBatch.DrawString (font, "WAVE INCOMING!", new Vector2 (GraphicsDevice.Viewport.Width / 2 - 80, 0), Color.DarkRed);
					break;
				}

				case GameState.GameOver:
				{
					spriteBatch.DrawString (font, "GAME OVER! Press R to restart or Q to quit", new Vector2 (player1.position.X, player1.position.Y - 20), Color.FloralWhite);
					break;
				}
			}

			spriteBatch.DrawString (font, "Wave: " + wave, new Vector2 (5, 5), Color.Black);

			base.Draw(gameTime);
		}
	}
}