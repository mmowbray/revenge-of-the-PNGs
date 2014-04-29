using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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

        Texture2D playerTexture; //our player's texture

        Boolean gameOver = false;

        Texture2D enemyTexture; //our Enemy's texture
  
        Vector2 enemyPositionTL = new Vector2();
        Vector2 enemyPositionTR = new Vector2();
        Vector2 enemyPositionBL = new Vector2();
        Vector2 enemyPositionBR = new Vector2();

        Boolean intermission;

        int randomSpawnLocation;
        int randomEnemyType;

        Vector2 indicatorPos = new Vector2();

        Vector2 currEnemySpawnPos = new Vector2();
       
        Player player;

        Texture2D projectileTexture; //bullet texture
        Vector2 projectilePosition;

        List<Enemy> theEnemies;
        List<Projectile> theProjectiles;
        List<Rectangle> thePlayerSpriteMapParts;
        List<Rectangle> theEnemySpriteMapParts;
        List<Rectangle> theProjectileIndicatorSpriteMapParts;

        TimeSpan timeSinceLastProjectile;
        TimeSpan fireTime;

        TimeSpan timeSinceLastZombie;
        TimeSpan zombieSpawnTimeInterval; //minimum delay between shooting bullets

        TimeSpan intermissionTime;//duration of the interval ie 10 secs
        TimeSpan intermissionBegin;//when intermission last began

        TimeSpan timeSinceReloadStart;
        TimeSpan reloadTime;

        Rectangle currAmmoIndicatorRect;

        SoundEffect gunFire;
        //SoundEffect enemyDeath;

        Texture2D projectileIndicator;

        int waveNumber;
        int waveEnemyCount = 0;

        int enemiesLeftToSpawn;

        Random random; //used for randomly spawning enemies

        SpriteFont enemyHealthSpriteFont;
        SpriteFont UIFont;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	      
			this.Window.Title = "Revenge of the PNGs";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			base.Initialize();

            fireTime = TimeSpan.FromSeconds(.15f);
            zombieSpawnTimeInterval = TimeSpan.FromSeconds(1.5f);
            reloadTime = TimeSpan.FromSeconds(1.0f);
            intermissionTime = TimeSpan.FromSeconds(5);

            enemyPositionTL.X = 0;
            enemyPositionTL.Y = 50;

            enemyPositionTR.X = GraphicsDevice.Viewport.Width - 50;
            enemyPositionTR.Y = 50;

            enemyPositionBL.X = 0;
            enemyPositionBL.Y = GraphicsDevice.Viewport.Height - 50;

            enemyPositionBR.X = GraphicsDevice.Viewport.Width - 50;
            enemyPositionBR.Y = GraphicsDevice.Viewport.Height - 50;

            random = new Random();

            theEnemies = new List<Enemy>();
            theProjectiles = new List<Projectile>();

            thePlayerSpriteMapParts = new List<Rectangle>();

            //Initialize all the player spritemap parts
            for (int i = 0; i < 21; i++) //there are 21 different spritemap parts for the player in playerSpriteMap.png
            {
                Rectangle newSpriteMapPart = new Rectangle(i * 34, 0, 34, 34);
                thePlayerSpriteMapParts.Add(newSpriteMapPart);
            }

            //Initialize all the enemy spritemap part rectangles
            theEnemySpriteMapParts = new List<Rectangle>();

            Rectangle enemySpriteMapPart1 = new Rectangle(0, 0, 25, 25); theEnemySpriteMapParts.Add(enemySpriteMapPart1);
            Rectangle enemySpriteMapPart2 = new Rectangle(25, 0, 21, 21); theEnemySpriteMapParts.Add(enemySpriteMapPart2);
            Rectangle enemySpriteMapPart3 = new Rectangle(46, 0, 17, 17); theEnemySpriteMapParts.Add(enemySpriteMapPart3);
            Rectangle enemySpriteMapPart4 = new Rectangle(63, 0, 13, 13); theEnemySpriteMapParts.Add(enemySpriteMapPart4);

            theProjectileIndicatorSpriteMapParts = new List<Rectangle>();

            for (int i = 0; i < 10; i++) //initialize all the bullet sprite map parts
            {
                Rectangle newProjectileSpriteMapPart = new Rectangle(i * 8, 0, 8, 30);
                theProjectileIndicatorSpriteMapParts.Add(newProjectileSpriteMapPart);
            }

            player = new Player(playerTexture, new Vector2(((GraphicsDevice.Viewport.Width - (playerTexture.Width / 21)) / 2), ((GraphicsDevice.Viewport.Height - playerTexture.Height) / 2)), 6, 6);
            waveNumber = 0;
            intermission = true;
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		/// 
		protected override void LoadContent ()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("playerSpriteMap");
            enemyTexture = Content.Load<Texture2D>("enemySpriteMap");
            projectileTexture = Content.Load<Texture2D>("projectile");
            projectileIndicator = Content.Load<Texture2D>("projectileSpriteMap"); ;

            UIFont = Content.Load<SpriteFont>("UIFont");
            enemyHealthSpriteFont = Content.Load<SpriteFont>("enemyHealthSpriteFont");

            gunFire = Content.Load<SoundEffect>("projectileFire.xnb");
            //enemyDeath = Content.Load<SoundEffect>("enemyDeath");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {      
            UpdateKeyboard(gameTime);
            
            if (gameTime.TotalGameTime - intermissionBegin > intermissionTime) //we are in intermission
            {
                intermission = false;
            }

            if (waveEnemyCount < 1)//killed all enemies
            {
                Intermission(gameTime);
            }

            if (player.health < 1)
            {
                player.kill();
                gameOver = true;
            }

            if (gameTime.TotalGameTime - timeSinceLastZombie > zombieSpawnTimeInterval && enemiesLeftToSpawn > 0 && gameTime.TotalGameTime - intermissionBegin > intermissionTime && waveEnemyCount - enemiesLeftToSpawn < 8 && !gameOver) //max 8 enemies on screen at once
            {
                timeSinceLastZombie = gameTime.TotalGameTime;

                addRandomEnemy(gameTime); //random enemy at random location//upper range is exclusive, so ran can be 0 1 2 or 3

                enemiesLeftToSpawn--;
            }

            player.playerPosition.X = MathHelper.Clamp(player.playerPosition.X, 0, GraphicsDevice.Viewport.Width - (playerTexture.Width / 21)); //prevent player from going off-screen using math clamp
            player.playerPosition.Y = MathHelper.Clamp(player.playerPosition.Y, 0, GraphicsDevice.Viewport.Height - playerTexture.Height);
            
            Rectangle playerRect = new Rectangle((int)player.playerPosition.X, (int)player.playerPosition.Y, (playerTexture.Width / 21), playerTexture.Height);

            for (int i = 0; i < theEnemies.Count; i++)
            {
                if (theEnemies[i].alive)
                {
                    Rectangle enemyRect = new Rectangle((int)theEnemies[i].enemyPosition.X, (int)theEnemies[i].enemyPosition.Y, theEnemies[i].currEnemyRect.Width, theEnemies[i].currEnemyRect.Height);

                    if (enemyRect.Intersects(playerRect)) //Enemy hits player
                    {
                        theEnemies[i].killEnemy();
                        //enemyDeath.Play();
                        waveEnemyCount--;
                        player.score++;

                        player.health = player.health - theEnemies[i].enemyHealth; //subtract enemy's remaining health from player's health

                        if (player.health < 0)
                        {
                            player.health = 0;
                        }
                    }

                    for (int j = 0; j < theProjectiles.Count; j++)
                    {
                        if (theProjectiles[j].active)
                        {
                            Rectangle projectileRect = new Rectangle((int)theProjectiles[j].projectilePosition.X, (int)theProjectiles[j].projectilePosition.Y, projectileTexture.Width, projectileTexture.Height);

                            if (enemyRect.Intersects(projectileRect)) //bullet hits enemy
                            {
                                theEnemies[i].enemyHealth--;
                                theProjectiles[j].killProjectile();

                                if (theEnemies[i].enemyHealth < 1)//the enemy was shot dead
                                {
                                    theEnemies[i].killEnemy();
                                    //enemyDeath.Play();
                                    waveEnemyCount--;
                                    player.score++;

                                    if (player.health < 10) //give the player health if player health is less than half (health can be replenished, but not armor)
                                    {
                                        player.health++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < theEnemies.Count; i++)
            {
                if (!gameOver)
                {
                    theEnemies[i].Update(player.playerPosition, enemyTexture, playerTexture);
                }
            }

            UpdateProjectiles(theProjectiles, theProjectileIndicatorSpriteMapParts, gameTime);

            base.Update(gameTime);
        }

        public void StartGame(GameTime gameTime)
        {
            waveNumber = 0;

            theEnemies.Clear();
            theProjectiles.Clear();

            player.playerPosition.X = (GraphicsDevice.Viewport.Width - (playerTexture.Width / 21)) / 2;
            player.playerPosition.Y = (GraphicsDevice.Viewport.Height - playerTexture.Height) / 2;

            player.reset();
            player.score = 0;
            waveEnemyCount = 0;

            gameOver = false;
        }
 
        public void UpdateKeyboard(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
           
            if (gameOver)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Y)) 
                {
                    StartGame(gameTime);
                }

                if (currentKeyboardState.IsKeyDown(Keys.Q))
                {
                    Exit();
                }
            }

            if (!gameOver)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Up))//Move using Arrow Keys
                {
                    player.playerPosition.Y -= player.dY;
                }

                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    player.playerPosition.X -= player.dX;
                }

                if (currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    player.playerPosition.Y += player.dY;
                }

                if (currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    player.playerPosition.X += player.dX;
                }

                if (currentKeyboardState.IsKeyDown(Keys.W)) //shoot bullets using WASD
                {
                    projectilePosition.X = player.playerPosition.X + ((playerTexture.Width / 21) - projectileTexture.Width) / 2;
                    projectilePosition.Y = player.playerPosition.Y - projectileTexture.Height;

                    if (gameTime.TotalGameTime - timeSinceLastProjectile > fireTime && gameTime.TotalGameTime - timeSinceReloadStart > reloadTime && player.projectileCount > 1 && !intermission)//(timeSinceReloadStart  > reloadTime)
                    {
                        timeSinceLastProjectile = gameTime.TotalGameTime;

                        Projectile newProjectile = new Projectile(projectileTexture, projectilePosition, 0, -35);
                        theProjectiles.Add(newProjectile);

                        gunFire.Play();

                        player.projectileCount--;
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.A))
                {
                    projectilePosition.X = player.playerPosition.X - projectileTexture.Width;
                    projectilePosition.Y = player.playerPosition.Y + (playerTexture.Height - projectileTexture.Height) / 2;

                    if (gameTime.TotalGameTime - timeSinceLastProjectile > fireTime && gameTime.TotalGameTime - timeSinceReloadStart > reloadTime && !intermission)
                    {
                        timeSinceLastProjectile = gameTime.TotalGameTime;

                        Projectile newProjectile = new Projectile(projectileTexture, projectilePosition, -35, 0);
                        theProjectiles.Add(newProjectile);

                        gunFire.Play();

                        player.projectileCount--;
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.S))
                {
                    projectilePosition.X = player.playerPosition.X + ((playerTexture.Width / 21) - projectileTexture.Width) / 2;
                    projectilePosition.Y = player.playerPosition.Y + playerTexture.Height;

                    if (gameTime.TotalGameTime - timeSinceLastProjectile > fireTime && gameTime.TotalGameTime - timeSinceReloadStart > reloadTime && !intermission)
                    {
                        timeSinceLastProjectile = gameTime.TotalGameTime;

                        Projectile newProjectile = new Projectile(projectileTexture, projectilePosition, 0, 35);
                        theProjectiles.Add(newProjectile);

                        gunFire.Play();

                        player.projectileCount--;
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.D))
                {
                    projectilePosition.X = player.playerPosition.X + (playerTexture.Width / 21);
                    projectilePosition.Y = player.playerPosition.Y + (playerTexture.Height - projectileTexture.Height) / 2;

                    if (gameTime.TotalGameTime - timeSinceLastProjectile > fireTime && gameTime.TotalGameTime - timeSinceReloadStart > reloadTime && !intermission)
                    {
                        timeSinceLastProjectile = gameTime.TotalGameTime;

                        Projectile newProjectile = new Projectile(projectileTexture, projectilePosition, 35, 0);
                        theProjectiles.Add(newProjectile);

                        gunFire.Play();

                        player.projectileCount--;
                    }
                }
            }
        }

        public void Intermission(GameTime gameTime) // call when all enemies killed
        {
            intermissionBegin = gameTime.TotalGameTime;
            intermission = true;

            player.reset();

            theEnemies.Clear();
            theProjectiles.Clear();
 
            waveNumber++;

            waveEnemyCount = 5 + waveNumber * 5;

            enemiesLeftToSpawn = waveEnemyCount;
        }

        public void UpdateProjectiles(List<Projectile> theProjectiles,List<Rectangle> theProjectileIndicatorSpriteMapParts, GameTime gameTime)
        {      
            for (int i = 0; i < theProjectiles.Count; i++) //update all the projectiles
            {
                theProjectiles[i].Update();

                if (theProjectiles[i].projectilePosition.X < 0 || theProjectiles[i].projectilePosition.X > (GraphicsDevice.Viewport.Width - projectileTexture.Width)
                    || theProjectiles[i].projectilePosition.Y < 0 || theProjectiles[i].projectilePosition.Y > (GraphicsDevice.Viewport.Height - projectileTexture.Height)) //bullet is off the screen
                {
                    theProjectiles[i].killProjectile();//Deactivate the projectiles that are off-screen
                }
            }

            if (player.projectileCount < 2) //shot all bullets, reload
            {
                player.reloading = true;

                timeSinceReloadStart = gameTime.TotalGameTime;

                player.projectileCount = 10;
            }

            if (gameTime.TotalGameTime - timeSinceReloadStart > reloadTime)
            {
                player.reloading = false;
            }

            currAmmoIndicatorRect = theProjectileIndicatorSpriteMapParts[player.projectileCount - 1];
        }

        public void addRandomEnemy(GameTime gameTime)
        {
            randomSpawnLocation = random.Next(0, 4);
            randomEnemyType = random.Next(0, 4);

            if (randomSpawnLocation == 0)
            {
                currEnemySpawnPos = enemyPositionTL;
            }

            if (randomSpawnLocation == 1)
            {
                currEnemySpawnPos = enemyPositionTR;
            }

            if (randomSpawnLocation == 2)
            {
                currEnemySpawnPos = enemyPositionBL;
            }

            if (randomSpawnLocation == 3)
            {
                currEnemySpawnPos = enemyPositionBR;
            }

            if (!intermission) //if not in intermission
            {
                if (randomEnemyType == 0)
                {
                    Enemy newEnemy0 = new Enemy(enemyTexture, currEnemySpawnPos, 1, theEnemySpriteMapParts[0], 4); //4 health for small fat monsters, move slow but need more damaging
                    theEnemies.Add(newEnemy0);
                }

                if (randomEnemyType == 1)
                {
                    Enemy newEnemy1 = new Enemy(enemyTexture, currEnemySpawnPos, 2, theEnemySpriteMapParts[1], 3);
                    theEnemies.Add(newEnemy1);
                }

                if (randomEnemyType == 2)
                {
                    Enemy newEnemy2 = new Enemy(enemyTexture, currEnemySpawnPos, 3, theEnemySpriteMapParts[2], 2);
                    theEnemies.Add(newEnemy2);
                }

                if (randomEnemyType == 3)
                {
                    Enemy newEnemy3 = new Enemy(enemyTexture, currEnemySpawnPos, 4, theEnemySpriteMapParts[3], 1);
                    theEnemies.Add(newEnemy3);
                }
            }
        }

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.DimGray);

            player.Draw(spriteBatch, thePlayerSpriteMapParts);

            for (int i = 0; i < theProjectiles.Count; i++)
            {
                theProjectiles[i].Draw(spriteBatch);
            }
            for (int i = 0; i < theEnemies.Count; i++)
            {
                theEnemies[i].Draw(spriteBatch, enemyHealthSpriteFont);
            }

            spriteBatch.Begin();

            indicatorPos.X = player.playerPosition.X + 24;
            indicatorPos.Y = player.playerPosition.Y + 2;

            spriteBatch.Draw(projectileIndicator, indicatorPos, currAmmoIndicatorRect, Color.White);

            spriteBatch.DrawString(UIFont, "Wave: " + waveNumber, new Vector2(0, GraphicsDevice.Viewport.Height - 27), Color.Black);
            spriteBatch.DrawString(UIFont, "Enemies Remaining: " + waveEnemyCount, new Vector2(130, GraphicsDevice.Viewport.Height - 27), Color.Black);
            spriteBatch.DrawString(UIFont, "Score: " + player.score, new Vector2(0, 0), Color.Black);

            if (intermission && !gameOver)
            {
                spriteBatch.DrawString(UIFont, "WAVE INCOMING!!!", new Vector2(GraphicsDevice.Viewport.Width / 2 - 80, 0), Color.DarkRed);
            }

            if (player.reloading && !intermission)
            {
                spriteBatch.DrawString(UIFont, "RELOADING!", new Vector2(GraphicsDevice.Viewport.Width / 2 - 80, 0), Color.Crimson);
            }

            if (gameOver)
            {
                spriteBatch.DrawString(enemyHealthSpriteFont, "GAME OVER! Press Y to restart or Q to quit", new Vector2(player.playerPosition.X, player.playerPosition.Y - 20), Color.FloralWhite);
            }

            spriteBatch.End();

            base.Draw(gameTime);
		}
	}
}

