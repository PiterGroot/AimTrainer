using static FirstMonoGame.Scripts.GameHelper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tweening;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace FirstMonoGame.Scripts
{
#pragma warning disable IDE0090 
    public class GameController : Game
    {
        public Action<GameTime> OnUpdate { get; set; }
        private GameTime GameTime { get; set; }
        public ContentManager ContentManager => Content;

        private readonly Tweener tweener = new Tweener();

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Vector2 mousePosition;
        private Vector2 mainMenuStaticTargetPosition = new Vector2(188, 129);

        private Random random;
        private Song succesSFX;
        private Song failSFX;

        private GameIdentity crosshair;
        private GameIdentity target;
        private GameIdentity gameBackground;
        private GameIdentity endScreen;

        private SpriteFont spriteFont;

        private string timerText;
        private Vector2 timerPositon;
        
        private const int mainMenuStaticTargetRadius = 60;
        private readonly int screenBoundsMargin = 80;
        private readonly int targetRadius = 45;

        private float timeUntilGameStarted = 0;
        private readonly int gameDuration = 20;

        private int hits = 0;
        private int misses = 0;

        private bool gameStarted = false;
        private bool gameEnded = false;
        
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

            spriteBatch = new SpriteBatch(GraphicsDevice);
            new GameIdentityManager(Content);

            GameHelper.GameController = this;
            GameHelper.GraphicsDevice = GraphicsDevice;
            GameHelper.spriteBatch = spriteBatch;
            random = new Random();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
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
            if (keyboardState.IsKeyDown(Keys.Back))
                Exit();

            if (keyboardState.IsKeyDown(Keys.Escape) && gameEnded) {
                HandleResetGame();
            }

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

            //updating game logic
            GameIdentityManager.Instance.DrawGameIdentities(spriteBatch, GraphicsDevice);
            tweener.Update(gameTime.GetElapsedSeconds());
            OnUpdate?.Invoke(gameTime);

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

                PlaySFX(hitTarget ? succesSFX : failSFX);
                RandomizeTargetPosition();
            });
        }

        private void HandleGameEnd() {
            gameStarted = false;
            gameEnded = true;

            GameIdentityManager.Instance.DestroyIdentity(gameBackground);
            GameIdentityManager.Instance.DestroyIdentity(target);

            endScreen = new GameIdentity("EndScreen", "sky_clean", 1, false);
            endScreen.Transform.scale = Vector2.One * 2;
            GameIdentityManager.Instance.InstantiateIdentity(endScreen);

            Vector2 topCenterPosition = new Vector2((Window.ClientBounds.Width / 2f), 0);
            Vector2 botCenterPosition = new Vector2((Window.ClientBounds.Width / 2f), Window.ClientBounds.Height);
            
            Vector2 endLabelPosition = topCenterPosition - new Vector2(75, 0);
            Vector2 endLabelHitsPosition = topCenterPosition - new Vector2(100, -100);
            Vector2 endLabelMissesPosition = topCenterPosition - new Vector2(130, -150);
            Vector2 endLabelAccuracyPosition = topCenterPosition - new Vector2(270, -200);
            Vector2 endLabelResetPosition = botCenterPosition - new Vector2(400, 50);

            TextDrawer.InstantiateTextLabel(spriteFont, "end", "END", endLabelPosition, Color.White, Vector2.One * 2);
            TextDrawer.InstantiateTextLabel(spriteFont, "hits", $"HITS:{hits}", endLabelHitsPosition, Color.White, Vector2.One * 1.3f);
            TextDrawer.InstantiateTextLabel(spriteFont, "misses", $"MISSES:{misses}", endLabelMissesPosition, Color.White, Vector2.One * 1.3f);
            TextDrawer.InstantiateTextLabel(spriteFont, "accuracy", $"ACCURACY:{CalculateAccuracy().ToString("F2")}%", endLabelAccuracyPosition, Color.White, Vector2.One * 1.3f);

            TextDrawer.InstantiateTextLabel(spriteFont, "reset", $"Hit esc to go back to main menu", endLabelResetPosition, Color.Black, Vector2.One * 1f);
        }

        private void HandleResetGame() {
            gameEnded = false;

            hits = 0;
            misses = 0;

            TextDrawer.DestroyText("end");
            TextDrawer.DestroyText("hits");
            TextDrawer.DestroyText("misses");
            TextDrawer.DestroyText("accuracy");
            TextDrawer.DestroyText("reset");

            GameIdentityManager.Instance.DestroyIdentity(endScreen);
        }

        private float CalculateAccuracy() {
            return ((float)hits / ((float)hits + (float)misses)) * 100;
        }
        
        private void UpdateGameTimer() {
            float gameTimer = (float)GameTime.TotalGameTime.TotalSeconds - timeUntilGameStarted;
            timerText = gameTimer.ToString("F2");

            if (gameTimer >= gameDuration) HandleGameEnd();
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

            tweener.CancelAndCompleteAll();
            tweener.TweenTo(target.Transform, target => target.scale, new Vector2(.6f, .6f), .1f, 0)
                .AutoReverse()
                .Easing(EasingFunctions.CubicIn);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (gameStarted) TextDrawer.DrawTextLabelOnce(spriteFont, timerText, timerPositon);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
#pragma warning restore IDE0090 
}