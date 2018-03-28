using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace asteriods
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D shipTexture;
        Texture2D oceanTile;
        Texture2D enemyship;
        Texture2D bulletTexture;
        Texture2D deadship;
        Texture2D pirateBackground;
        Texture2D pirateEnding;
        Texture2D pirateVictory;

        //player variables 
        float playerSpeed = 150f;
        float playerTurnSpeed = 6;
        Vector2 playerPosition = new Vector2(0, 0);
        Vector2 playerOffset = new Vector2(0, 0);
        float playerRotation = 0;
        float playerRadius = 40;
        bool playerIsAlive = true;

        //enemy variables
        const int numberOfEnemies = 4;
        Vector2[] enemyPositions = new Vector2[numberOfEnemies];
        Vector2[] enemyOffsets = new Vector2[numberOfEnemies];
        float[] enemyRotations = new float[numberOfEnemies];
        bool[] enemyLifeStates = new bool[numberOfEnemies];

        float enemyRadius = 40;
        float enemySpeed = 100f;
        float enemyTurnSpeed = 6;

        //bullet variables
      //  Vector2 bulletPosition = new Vector2(0, 0);
        Vector2 bulletOffset = new Vector2(0, 0);
        float bulletSpeed = 300;
        float bulletRadius = 5;
        //Vector2 bulletVelocity = new Vector2(0, 0);
       // bool bulletIsDead = true;
        const int numberOfBullets = 50;
        Vector2[] bulletPositions = new Vector2[numberOfBullets];
        Vector2[] bulletVelocity = new Vector2[numberOfBullets];
        bool[] bulletLifeStates = new bool[numberOfBullets];

        float bulletShootTimer = 0;
        float shootDelay = 0.3f;

        SpriteFont FreestyleFont;
        SpriteFont ArialFont;

        int score = 0;
        int currentFPS = 0;
        int fpsCounter = 0;
        float fpsTimer = 0;

        const int STATE_SPLASH = 0;
        const int STATE_GAME = 1;
        const int STATE_GAMEOVER = 2;
        const int STATE_WIN = 3;

        int gameState = STATE_SPLASH;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
            this.graphics.SynchronizeWithVerticalRetrace = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ResetGame();

            base.Initialize();
        }

        protected void ResetGame()
        {
            int halfWidth = graphics.GraphicsDevice.Viewport.Width / 2;
            int halfHeight = graphics.GraphicsDevice.Viewport.Height / 2;

            playerPosition = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 3,
                graphics.GraphicsDevice.Viewport.Height / 3);


            enemyPositions[0] = new Vector2(80, halfHeight);
            enemyRotations[0] = 5f;
            enemyLifeStates[0] = true;

            enemyPositions[1] = new Vector2(graphics.GraphicsDevice.Viewport.Width - 80, halfHeight);
            enemyRotations[1] = 1.5f;
            enemyLifeStates[1] = true;

            enemyPositions[2] = new Vector2(halfWidth, 80);
            enemyRotations[2] = 0.5f;
            enemyLifeStates[2] = true;

            enemyPositions[3] = new Vector2(halfWidth, graphics.GraphicsDevice.Viewport.Height - 80);
            enemyRotations[3] = 3.5f;
            enemyLifeStates[3] = true;


            score = 0;
            playerRotation = 0;
            playerIsAlive = true;

            
            for(int i=0; i < numberOfBullets; i++)
            {
                bulletLifeStates[i] = false;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            shipTexture = Content.Load<Texture2D>("pirate ship");
            oceanTile = Content.Load<Texture2D>("ocean");
            enemyship = Content.Load<Texture2D>("enemyship");
            bulletTexture = Content.Load<Texture2D>("cannonball");
            deadship = Content.Load<Texture2D>("dead ship");
            pirateBackground = Content.Load<Texture2D>("pirate background");
            pirateEnding = Content.Load<Texture2D>("pirate ending 2");
            pirateVictory = Content.Load<Texture2D>("pirate victory");

            FreestyleFont = Content.Load<SpriteFont>("freestyle script regular");
            ArialFont = Content.Load<SpriteFont>("arial");

            playerOffset = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);
            bulletOffset = new Vector2(bulletTexture.Width / 2, bulletTexture.Height / 2);

            playerRadius = shipTexture.Height / 2f;
            enemyRadius = enemyship.Height / 2f;

            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemyOffsets[i] = new Vector2(enemyship.Width / 2, enemyship.Height / 2);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        void ShootBullet(Vector2 position, float rotation)
        {

            int indexOfDeadBullet = -1;

            for(int i=0; i < numberOfBullets; i++)
            {
                if(bulletLifeStates[i] == false)
                {
                    indexOfDeadBullet = i;
                    break;
                }
            }

            if (indexOfDeadBullet == -1)
                return;

            Vector2 direction = new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));
            direction.Normalize();
            bulletVelocity[indexOfDeadBullet] = direction * bulletSpeed;
            bulletPositions[indexOfDeadBullet] = position;
            bulletLifeStates[indexOfDeadBullet] = true;
        }
        void UpdateBullet(int bulletidx, float deltaTime)
        {
            
                bulletPositions[bulletidx] += bulletVelocity[bulletidx] * deltaTime;

                if (bulletPositions[bulletidx].X < 0 ||
                   bulletPositions[bulletidx].X > graphics.GraphicsDevice.Viewport.Width ||
                   bulletPositions[bulletidx].Y < 0 ||
                   bulletPositions[bulletidx].Y > graphics.GraphicsDevice.Viewport.Height)
                {
                    bulletLifeStates[bulletidx] = true;
                }
          
        }

        protected void Updateplayer(float deltaTime)
        {
            if (playerIsAlive == false)
                return;
            KeyboardState state = Keyboard.GetState();

            float xSpeed = 0;
            float ySpeed = 0;

            bulletShootTimer += deltaTime;

            if (state.IsKeyDown(Keys.Up) == true)
            {
                ySpeed += playerSpeed * deltaTime;
            }
            if (state.IsKeyDown(Keys.Down) == true)
            {
                ySpeed -= playerSpeed * deltaTime;
            }
            if (state.IsKeyDown(Keys.Left) == true)
            {
                playerRotation -= playerTurnSpeed * deltaTime;
            }
            if (state.IsKeyDown(Keys.Right) == true)
            {
                playerRotation += playerTurnSpeed * deltaTime;
            }
            if (state.IsKeyDown(Keys.Space) == true)
            {
                if (bulletShootTimer >= shootDelay)
                {
                    ShootBullet(playerPosition, playerRotation);
                    bulletShootTimer = 0;
                }
            }

            double x = (xSpeed * Math.Cos(playerRotation)) - (ySpeed * Math.Sin(playerRotation));
            double y = (xSpeed * Math.Sin(playerRotation)) + (ySpeed * Math.Cos(playerRotation));

            playerPosition.X += (float)x;
            playerPosition.Y += (float)y;

            if (playerPosition.X < -playerOffset.Y)
            {
                playerPosition.X = graphics.GraphicsDevice.Viewport.Width - playerOffset.Y;
            }
            if (playerPosition.Y < -playerOffset.Y)
            {
                playerPosition.Y = graphics.GraphicsDevice.Viewport.Height - playerOffset.Y;
            }
            if (playerPosition.X > graphics.GraphicsDevice.Viewport.Width + playerOffset.Y)
            {
                playerPosition.X = playerOffset.Y;
            }
            if (playerPosition.Y > graphics.GraphicsDevice.Viewport.Height + playerOffset.Y)
            {
                playerPosition.Y = playerOffset.Y;
            }

            Vector2 playerDirection = new Vector2(-(float)Math.Sin(playerRotation),
                                                    (float)Math.Cos(playerRotation));
            playerDirection.Normalize();

            Vector2 direction = new Vector2(30, 30);
            direction.Normalize();

            Vector2 playerVelocity = playerDirection * ySpeed;
            playerPosition += playerVelocity;
        }



        protected void UpdateEnemies(float deltaTime)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (enemyLifeStates[i] == true)
                {
                    Vector2 velocity = new Vector2(
                        (float)(-enemySpeed * Math.Sin(enemyRotations[i])),
                        (float)(enemySpeed * Math.Cos(enemyRotations[i])));



                    Vector2 direction = playerPosition - enemyPositions[i];
                    float distance = direction.Length();

                    if (enemyPositions[i].X < 0)
                    {
                        enemyPositions[i].X = 0;
                        velocity.X = -velocity.X;
                        enemyRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }
                    if (enemyPositions[i].Y < 0)
                    {
                        enemyPositions[i].Y = 0;
                        velocity.Y = -velocity.Y;
                        enemyRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }
                    if (enemyPositions[i].X > graphics.GraphicsDevice.Viewport.Width)
                    {
                        enemyPositions[i].X = graphics.GraphicsDevice.Viewport.Width;
                        velocity.X = -velocity.X;
                        enemyRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }
                    if (enemyPositions[i].Y > graphics.GraphicsDevice.Viewport.Height)
                    {
                        enemyPositions[i].X = graphics.GraphicsDevice.Viewport.Height;
                        velocity.Y = -velocity.Y;
                        enemyRotations[i] = (float)Math.Atan2(velocity.Y, velocity.X) - 1.5708f;
                    }


                    enemyPositions[i] += velocity * deltaTime;
                }
            }
        }

        protected void UpdateEnemyCollision()
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (enemyLifeStates[i] == false)
                    continue;

                for (int j = i; j < numberOfEnemies; j++)
                {
                    if (enemyLifeStates[j] == false)
                        continue;

                    if (i == j)
                        continue;

                    if (IsColliding(enemyPositions[i], enemyRadius, enemyPositions[j], enemyRadius) == true)
                    {
                        if (enemyLifeStates[i] == true)
                            enemyRotations[i] += 3.14159f;

                        if (enemyLifeStates[j] == true)
                            enemyRotations[j] += 3.14159f;
                        return;
                    }
                }
            }
        }

        protected void UpdateSplashState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_GAME;
            }
        }

        protected void UpdateGameState(float deltaTime)
        {
            Updateplayer(deltaTime);
            UpdateEnemies(deltaTime);
            UpdateEnemyCollision();


            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (enemyLifeStates[i] == false)
                    continue;

                if (IsColliding(enemyPositions[i], enemyRadius, playerPosition, playerRadius) == true)
                {
                    playerIsAlive = false;
                    gameState = STATE_GAMEOVER;
                    break;
                }
            }

            for (int bulletidx = 0; bulletidx < numberOfBullets; bulletidx++)
            {
                if (bulletLifeStates[bulletidx] == true)
                {
                    UpdateBullet(bulletidx, deltaTime);

                    for (int i = 0; i < numberOfEnemies; i++)
                    {

                        if (enemyLifeStates[i] == true)
                        {
                            bool isColliding = IsColliding(bulletPositions[bulletidx],
                                                    bulletRadius, enemyPositions[i], enemyRadius);

                            if (isColliding == true)
                            {
                                score ++; 
                                bulletLifeStates[bulletidx] = false;
                                enemyLifeStates[i] = false;
                                break;
                            }
                        }
                    }
                }
            }

         



            int aliveCount = 0;
            foreach (bool lifeState in enemyLifeStates)
            {
                if (lifeState == true)
                    aliveCount++;
            }
            if (aliveCount == 0)
            {
                gameState = STATE_WIN; 
            }
        }
        protected void UpdateGameOverState(float deltaTime)
        {
                       
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_SPLASH;
                ResetGame();
            }
        }

        protected void UpdateWinState(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                gameState = STATE_SPLASH;
                ResetGame();
            }
            if (score == 4)
            {
                gameState = STATE_WIN;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fpsTimer += deltaTime;
            fpsCounter++;
            if(fpsTimer >= 1.0f)
            {
                currentFPS = fpsCounter;
                fpsCounter = 0;
                fpsTimer -= 1.0f;
            }
           
            switch (gameState)
            {
                case STATE_SPLASH:
                    UpdateSplashState(deltaTime);
                    break;
                case STATE_GAME:
                    UpdateGameState(deltaTime);
                    break;
                case STATE_GAMEOVER:
                    UpdateGameOverState(deltaTime);
                    break;
                case STATE_WIN:
                    UpdateWinState(deltaTime);
                    break;
            }

            
            base.Update(gameTime);
        }

        protected void DrawSplashState( SpriteBatch spriteBatch)
        {
            float Xscale = (float) graphics.GraphicsDevice.Viewport.Width / pirateBackground.Width ;
            float Yscale = (float) graphics.GraphicsDevice.Viewport.Height / pirateBackground.Height ; 
            spriteBatch.Draw(pirateBackground, new Vector2(0,0), null, Color.White, 0, new Vector2(0,0), new Vector2 (Xscale, Yscale), SpriteEffects.None, 0);
            
            spriteBatch.DrawString(ArialFont, "Press Enter to Play!", new Vector2(320, 200), Color.Red);
            
            
        }

        protected void DrawGameState(SpriteBatch spriteBatch)
        {

            int tileWidth = (graphics.GraphicsDevice.Viewport.Width / oceanTile.Width) + 1;
            int tileHeight = (graphics.GraphicsDevice.Viewport.Height / oceanTile.Height) + 1;

            for (int column = 0; column < tileWidth; column += 1)
            {
                for (int row = 0; row < tileHeight; row += 1)
                {
                    Vector2 position = new Vector2(column * oceanTile.Width, row * oceanTile.Height);

                    spriteBatch.Draw(oceanTile, position, Color.White);
                }
            }

            for(int i=0; i < numberOfBullets; i++)
            if (bulletLifeStates[i] == true)
            {
                spriteBatch.Draw(bulletTexture, bulletPositions[i], null,
                        Color.White, 0, bulletOffset, 1, SpriteEffects.None, 0);
            }

            
            if (playerIsAlive == true)
            {
                spriteBatch.Draw(shipTexture, playerPosition, null,
                    Color.White, playerRotation, playerOffset, 1, SpriteEffects.None, 0);
            }
            if (playerPosition.X < playerOffset.Y)
            {
                Vector2 wrapPos = new Vector2(graphics.GraphicsDevice.Viewport.Width + playerPosition.X, playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation, playerOffset, 1, SpriteEffects.None, 0);
            }
            if (playerPosition.Y < playerOffset.Y)
            {
                Vector2 wrapPos = new Vector2(playerPosition.X, graphics.GraphicsDevice.Viewport.Height + playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation, playerOffset, 1, SpriteEffects.None, 0);
            }
            if (playerPosition.X > graphics.GraphicsDevice.Viewport.Width - playerOffset.Y)
            {
                Vector2 wrapPos = new Vector2(-(graphics.GraphicsDevice.Viewport.Width - playerPosition.X), playerPosition.Y);
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation, playerOffset, 1, SpriteEffects.None, 0);
            }
            if (playerPosition.Y > graphics.GraphicsDevice.Viewport.Height - playerOffset.Y)
            {
                Vector2 wrapPos = new Vector2(playerPosition.X, -(graphics.GraphicsDevice.Viewport.Height - playerPosition.Y));
                spriteBatch.Draw(shipTexture, wrapPos, null, Color.White, playerRotation, playerOffset, 1, SpriteEffects.None, 0);
            }

            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (enemyLifeStates[i] == true)
                {
                    spriteBatch.Draw(enemyship,
                        enemyPositions[i], null, Color.White,
                        enemyRotations[i],
                        enemyOffsets[i], 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(deadship,
                        enemyPositions[i], null, Color.White,
                        enemyRotations[i],
                        enemyOffsets[i], 1, SpriteEffects.None, 0);
                }
            }
                        
            spriteBatch.DrawString(ArialFont, "FPS" + fpsCounter, new Vector2(graphics.GraphicsDevice.Viewport.Width - 150, 30), Color.Black,
                0, new Vector2(0, 0), 2, SpriteEffects.None, 0);

            spriteBatch.DrawString(ArialFont, "Score = " + score, new Vector2(80, 30), Color.Black,
                0, new Vector2(0, 0), 2, SpriteEffects.None, 0);
        }

        protected void DrawWinState(SpriteBatch spriteBatch)
        {
            float Xscale = (float)graphics.GraphicsDevice.Viewport.Width / pirateVictory.Width;
            float Yscale = (float)graphics.GraphicsDevice.Viewport.Height / pirateVictory.Height;
            spriteBatch.Draw(pirateVictory, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), new Vector2(Xscale, Yscale), SpriteEffects.None, 0);

            spriteBatch.DrawString(ArialFont, "YOU WIN!! \n\nPress ENTER to Play Again!", new Vector2(330, 170), Color.Red);
        }
                

        protected void DrawGameOverState(SpriteBatch spriteBatch)
        {
            float Xscale = (float)graphics.GraphicsDevice.Viewport.Width / pirateEnding.Width;
            float Yscale = (float)graphics.GraphicsDevice.Viewport.Height / pirateEnding.Height;
            spriteBatch.Draw(pirateEnding, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), new Vector2(Xscale, Yscale), SpriteEffects.None, 0);

            spriteBatch.DrawString(ArialFont, "GAME OVER! \n\nPress ENTER to Play Again!", new Vector2(360, 170), Color.Red);
        }

      

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            switch(gameState)
            {
                case STATE_SPLASH:
                    DrawSplashState(spriteBatch);
                    break;
                case STATE_GAME:
                    DrawGameState(spriteBatch);
                    break;
                case STATE_GAMEOVER:
                    DrawGameOverState(spriteBatch);
                    break;
                case STATE_WIN:
                    DrawWinState(spriteBatch);
                    break;
            }

            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected bool IsColliding(Vector2 position1, float radius1, Vector2 position2, float radius2)
        {
            Vector2 distance = position2 - position1;

            if (distance.Length()< radius1 + radius2)
            {
                return true;
            }
            return false;
        }        
    }
}
