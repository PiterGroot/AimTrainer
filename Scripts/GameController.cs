using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FirstMonoGame.Scripts
{
    public class GameController : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;
        public Action onUpdate;

        private Texture2D backgroundSprite;
        private Texture2D crosshairSprite;
        private Texture2D targetSprite;

        private Vector2 mousePosition;
        private Vector2 currentTargetPosition;
        private Random random;

        private int currentScore = 0;

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
            random = new Random();
            GameHelper.GameController = this;

            MoveTargetPosition();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            targetSprite = Content.Load<Texture2D>("target");
            backgroundSprite = Content.Load<Texture2D>("sky");
            crosshairSprite = Content.Load<Texture2D>("crosshair");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            onUpdate?.Invoke();
            Window.Title = $"Very cool geam. Current score: {currentScore}";

            MouseState currentMouseState = Mouse.GetState();
            mousePosition = currentMouseState.Position.ToVector2();

            GameHelper.InputHandler.OnMouseDown(() =>
            {
                Vector2 adjustedMousePosition = mousePosition + new Vector2(50, 50);
                float mouseTargetDistane = Vector2.Distance(currentTargetPosition, adjustedMousePosition) - 125;

                if (mouseTargetDistane < 40 && mouseTargetDistane > -40)
                {
                    currentScore++;
                    MoveTargetPosition();
                }
            });

            base.Update(gameTime);
        }
        private void MoveTargetPosition()
        {
            Vector2 minScreenBounds = Vector2.One * 80;
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

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            Rectangle backgroundRectangle = new Rectangle(0, 0, 982, 720);
            Rectangle croshairRectangle = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 20, 20);
            Rectangle targetRectangle = new Rectangle((int)currentTargetPosition.X, (int)currentTargetPosition.Y, 96, 94);

            _spriteBatch.Draw(backgroundSprite, backgroundRectangle, Color.White);
            _spriteBatch.Draw(targetSprite, targetRectangle, Color.White);
            _spriteBatch.Draw(crosshairSprite, croshairRectangle, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}