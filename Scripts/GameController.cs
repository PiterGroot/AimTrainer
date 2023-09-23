using static FirstMonoGame.Scripts.GameHelper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace FirstMonoGame.Scripts
{
    public class GameController : Game
    {
        public Action onUpdate;
      
        public GameIdentityManager gameIdentityManager;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private Texture2D mainBackgroundSprite;
        private Texture2D backgroundSprite;
        private Texture2D crosshairSprite;
        private Texture2D targetSprite;

        private Vector2 mousePosition;
        private Vector2 currentTargetPosition;
        private Vector2 mouseOffset = new Vector2(50, 50);
        private Vector2 mainMenuStaticTargetPosition = new Vector2(178, 119);

        private Random random;
        private Song succesSFX;
        private Song failSFX;

        private int screenBoundsMargin = 80;
        private int targetRadius = 40;
        private const int targetOriginOffset = 125;
        private const int mainMenuStaticTargetRadius = 60;

        private int gameDuration = 20;
        private bool gameStarted = false;

        private float timeUntilGameStarted = 0;

        private int hits = 0;
        private int misses = 0;
        
        public GameController()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 640;

            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            gameIdentityManager = new GameIdentityManager();
            GameHelper.GameController = this;
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            targetSprite = Content.Load<Texture2D>("target");
            backgroundSprite = Content.Load<Texture2D>("sky");
            mainBackgroundSprite = Content.Load<Texture2D>("sky_main");
            crosshairSprite = Content.Load<Texture2D>("crosshair");
            spriteFont = Content.Load<SpriteFont>("font");
            
            succesSFX = Content.Load<Song>("succesSFX");
            failSFX = Content.Load<Song>("failSFX");
            MediaPlayer.Volume = .4f;

            GameIdentity identity = new GameIdentity("Background", mainBackgroundSprite, -1);
            GameIdentityManager.Instance.RegisterGameIdentity(identity);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            onUpdate?.Invoke();
            Window.Title = $"Aim Trainer | Hits: {hits} Misses: {misses}";

            //updating crosshair position
            MouseState currentMouseState = Mouse.GetState();
            mousePosition = currentMouseState.Position.ToVector2();

            if (!gameStarted) {
                HandleStartGame(); //simple main menu logic
                timeUntilGameStarted = (float)gameTime.TotalGameTime.TotalSeconds;
            }
            else HandleGameplay(); //run gameplay loop

            base.Update(gameTime);
        }

        private void HandleStartGame() {
            InputHandler.OnMouseDown(() => {
                float mouseTargetDistance = Vector2.Distance(mainMenuStaticTargetPosition, mousePosition);
                if (mouseTargetDistance < mainMenuStaticTargetRadius) {
                    gameStarted = true;
                    MediaPlayer.Play(succesSFX);
                    RandomizeTargetPosition();
                }
                else MediaPlayer.Play(failSFX);
            });
        }

        private void HandleGameplay() {
            InputHandler.OnMouseDown(() => {
                Vector2 adjustedMousePosition = mousePosition + mouseOffset;
                float mouseTargetDistance = Vector2.Distance(currentTargetPosition, adjustedMousePosition) - targetOriginOffset;

                if (mouseTargetDistance < targetRadius && mouseTargetDistance > -targetRadius) {
                    hits++;
                    MediaPlayer.Play(succesSFX);
                    RandomizeTargetPosition();
                }
                else {
                    misses++;
                    MediaPlayer.Play(failSFX);
                    RandomizeTargetPosition();
                }
            });
        }

        private void RandomizeTargetPosition()
        {
            Vector2 minScreenBounds = Vector2.One * screenBoundsMargin;
            int maxX = Window.ClientBounds.Width - (int)minScreenBounds.X;
            int maxY = Window.ClientBounds.Height - (int)minScreenBounds.Y;

            Vector2 randomScreenPosition = new Vector2();
            randomScreenPosition.X = random.Next((int)minScreenBounds.X, maxX);
            randomScreenPosition.Y = random.Next((int)minScreenBounds.Y, maxY);

            currentTargetPosition = randomScreenPosition;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);


            int l = GameIdentityManager.Instance.ActiveGameIdentities.Count;
            for (int i = 0; i < l; i++) {
                GameIdentity identity = GameIdentityManager.Instance.ActiveGameIdentities[i];
                Transform transform = identity.Transform;
                Visual visual = identity.Visual;

                int scaleX = visual.targetTexture.Width * (int)transform.scale.X;
                int scaleY = visual.targetTexture.Height * (int)transform.scale.Y;

                Rectangle identityRectangle = new Rectangle((int)transform.position.X, (int)transform.position.Y, scaleX, scaleY);
                spriteBatch.Draw(visual.targetTexture, identityRectangle, visual.textureColor);
            }

            Rectangle backgroundRectangle = new Rectangle(0, 0, 982, 720);
            //Rectangle croshairRectangle = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 20, 20);
            //Rectangle targetRectangle = new Rectangle((int)currentTargetPosition.X, (int)currentTargetPosition.Y, 96, 94);

            spriteBatch.Draw(gameStarted ? backgroundSprite : mainBackgroundSprite, backgroundRectangle, Color.White);
            //if(gameStarted) spriteBatch.Draw(targetSprite, targetRectangle, Color.White);
            //spriteBatch.Draw(crosshairSprite, croshairRectangle, Color.White);


            if (gameStarted) {
                Vector2 timerPosition = new Vector2((Window.ClientBounds.Width / 2f) - 55, 0);
                
                float gameTimer = (float)gameTime.TotalGameTime.TotalSeconds - timeUntilGameStarted;
                string timerText = gameTimer.ToString("F2");

                if(gameTimer >= gameDuration) {
                    gameStarted = false;
                }

                spriteBatch.DrawString(spriteFont, timerText, timerPosition, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}