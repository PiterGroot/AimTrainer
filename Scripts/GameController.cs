using static FirstMonoGame.Scripts.GameHelper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;

namespace FirstMonoGame.Scripts
{
    public class GameController : Game
    {
        public GameIdentityManager gameIdentityManager;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private GameTime GameTime { get; set; }

        private Vector2 mousePosition;
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

        private string timerText;
        private Vector2 timerPositon;

        private int hits = 0;
        private int misses = 0;

        private GameIdentity crosshair;
        private GameIdentity target;
        private GameIdentity gameBackground;

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
            timerPositon = new Vector2((Window.ClientBounds.Width / 2f), 0);
            gameIdentityManager = new GameIdentityManager(Content);
            GameHelper.GameController = this;
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            succesSFX = Content.Load<Song>("succesSFX");
            failSFX = Content.Load<Song>("failSFX");
            spriteFont = Content.Load<SpriteFont>("font");

            MediaPlayer.Volume = .4f;

            GameIdentity mainBackground = new GameIdentity("Background", "sky_main", -1);
            mainBackground.Transform.scale = Vector2.One * 2;
            GameIdentityManager.Instance.InstantiateIdentity(mainBackground);

            crosshair = new GameIdentity("Crosshair", "crosshair", 99);
            crosshair.Transform.scale = new Vector2(2f, 2f);
            GameIdentityManager.Instance.InstantiateIdentity(crosshair);

            GameIdentity debugObject = new GameIdentity("Debug", "debug", 99);
            debugObject.Transform.scale = new Vector2(2f, 2f);
            GameIdentityManager.Instance.InstantiateIdentity(debugObject);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            GameTime = gameTime;
            Window.Title = $"Aim Trainer | Hits: {hits} Misses: {misses}";

            //updating crosshair position
            MouseState currentMouseState = Mouse.GetState();
            mousePosition = currentMouseState.Position.ToVector2();
            crosshair.Transform.position = mousePosition;

            if (!gameStarted) {
                HandleStartGame(); //simple main menu logic
                timeUntilGameStarted = (float)gameTime.TotalGameTime.TotalSeconds;
            }
            else HandleGameplay(); //run gameplay loop

            gameIdentityManager.DrawGameIdentities(spriteBatch, GraphicsDevice, gameTime);
            base.Update(gameTime);
        }

        private void HandleStartGame() {
            InputHandler.OnMouseDown(() => {
                float mouseTargetDistance = Vector2.Distance(mainMenuStaticTargetPosition, mousePosition);
                if (mouseTargetDistance < mainMenuStaticTargetRadius) {
                    gameStarted = true;
                    UpdateGameTimer();

                    target = new GameIdentity("Target", "target", 2);
                    target.Transform.scale = Vector2.One * .5f;
                    GameIdentityManager.Instance.InstantiateIdentity(target);

                    gameBackground = new GameIdentity("GameBackground", "sky", 1);
                    gameBackground.Transform.scale = Vector2.One * 2;
                    GameIdentityManager.Instance.InstantiateIdentity(gameBackground);

                    MediaPlayer.Play(succesSFX);
                    RandomizeTargetPosition();
                }
                else MediaPlayer.Play(failSFX);
            });
        }

        private void HandleGameplay() {
            UpdateGameTimer();

            InputHandler.OnMouseDown(() => {
                Vector2 adjustedMousePosition = mousePosition + mouseOffset;
                float mouseTargetDistance = Vector2.Distance(target.Transform.position, adjustedMousePosition) - targetOriginOffset;

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

        private void UpdateGameTimer() {
            float gameTimer = (float)GameTime.TotalGameTime.TotalSeconds - timeUntilGameStarted;
            timerText = gameTimer.ToString("F2");

            if (gameTimer >= gameDuration) {
                GameIdentityManager.Instance.DestroyIdentity(gameBackground);
                GameIdentityManager.Instance.DestroyIdentity(target);
                gameStarted = false;
            }
        }

        private void RandomizeTargetPosition()
        {
            Vector2 minScreenBounds = Vector2.One * screenBoundsMargin;
            int maxX = Window.ClientBounds.Width - (int)minScreenBounds.X;
            int maxY = Window.ClientBounds.Height - (int)minScreenBounds.Y;

            Vector2 randomScreenPosition = new Vector2();
            randomScreenPosition.X = random.Next((int)minScreenBounds.X, maxX);
            randomScreenPosition.Y = random.Next((int)minScreenBounds.Y, maxY);

            target.Transform.position = randomScreenPosition;
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if(gameStarted) DrawText(spriteBatch, spriteFont, timerText, timerPositon);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}