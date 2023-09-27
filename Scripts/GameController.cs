using static FirstMonoGame.Scripts.GameHelper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tweening;
using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Content;

namespace FirstMonoGame.Scripts
{
#pragma warning disable IDE0090 
    public class GameController : Game
    {
        private GameIdentityManager gameIdentityManager;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public ContentManager ContentManager => Content;
        private GameTime GameTime { get; set; }

        private Vector2 mousePosition;
        private Vector2 mainMenuStaticTargetPosition = new Vector2(188, 129);

        private Random random;
        private Song succesSFX;
        private Song failSFX;

        private int screenBoundsMargin = 80;
        private int targetRadius = 45;
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
        private GameIdentity debugObject;

        private readonly Tweener tweener = new Tweener();

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
            GameHelper.graphicsDevice = GraphicsDevice;
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

            GameIdentity mainBackground = new GameIdentity("Background", "sky_main", -1, false);
            mainBackground.Transform.scale = Vector2.One * 2;
            GameIdentityManager.Instance.InstantiateIdentity(mainBackground);

            crosshair = new GameIdentity("Crosshair", "crosshair", 99);
            crosshair.Transform.scale = new Vector2(2f, 2f);
            
            GameIdentityManager.Instance.InstantiateIdentity(crosshair);

            
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

            gameIdentityManager.DrawGameIdentities(spriteBatch, GraphicsDevice);
            tweener.Update(gameTime.GetElapsedSeconds());
            
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

                    gameBackground = new GameIdentity("GameBackground", "sky", 1, false);
                    gameBackground.Transform.scale = Vector2.One * 2;
                    GameIdentityManager.Instance.InstantiateIdentity(gameBackground);

                    PlaySFX(succesSFX);
                    RandomizeTargetPosition();
                }
                else PlaySFX(failSFX);
            });
        }

        private void HandleGameplay() {
            UpdateGameTimer();

            InputHandler.OnMouseDown(() => {
                float mouseTargetDistance = Vector2.Distance(target.Transform.position, mousePosition);
                bool hitTarget = mouseTargetDistance < targetRadius;
                
                if (hitTarget) hits++;
                else misses++;
                
                MediaPlayer.Play(hitTarget ? succesSFX : failSFX);
                RandomizeTargetPosition();
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
            //target.Transform.scale = (Vector2.One * .5f) * RandomHandler.GetRandomFloatingNumber(.85f, 1.15f);

            tweener.CancelAndCompleteAll();
            tweener.TweenTo(target.Transform, target => target.scale, new Vector2(.6f, .6f), .1f, 0)
                .AutoReverse()
                .Easing(EasingFunctions.CubicIn);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (gameStarted) DrawText(spriteBatch, spriteFont, timerText, timerPositon);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
#pragma warning restore IDE0090 
}