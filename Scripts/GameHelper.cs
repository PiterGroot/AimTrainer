using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Timers;
using System;
using Microsoft.Xna.Framework.Media;

namespace FirstMonoGame.Scripts
{
#pragma warning disable IDE1006 
    public static class GameHelper
    {
        private static GameController gameController;
        public static GraphicsDevice graphicsDevice;
        public static GameController GameController
        {
            get => gameController;
            set
            {
                if (value == null) return;
                gameController = value;
                RandomHandler.random = new Random();
            }
        }

        /// <summary>
        /// Waits for some amount of time (seconds) and invokes onComplete callback
        /// </summary>
        public static void WaitFor(float duration, Action onWaitComplete)
        {
            Timer timer = new Timer(duration * 1000);
            timer.Start();

            timer.Elapsed += (sender, e) =>
            {
                timer.Dispose();
                onWaitComplete?.Invoke();
            };
        }

        public static void print(object message) {
            Debug.WriteLine(message.ToString());
        }

        public static void DrawText(SpriteBatch spriteBatch, SpriteFont font, object message, Vector2 position) {
            spriteBatch.DrawString(font, message.ToString(), position, Color.White);
        }

        public static void DrawText(SpriteBatch spriteBatch, SpriteFont font, object message, Vector2 position, Color textColor) {
            spriteBatch.DrawString(font, message.ToString(), position, textColor);
        }

        public static void DrawStaticTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, float rotation, Vector2 scale) {
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public static bool IsMouseInsideWindow() {
            MouseState mouseState = Mouse.GetState();
            return graphicsDevice.Viewport.Bounds.Contains(mouseState.Position);
        }

        public static void PlaySFX(Song sfx, bool doSafetyChecks = true) {
            if (doSafetyChecks) {
                if (IsMouseInsideWindow() && GameController.IsActive) {
                    MediaPlayer.Play(sfx);
                }
            }
            else MediaPlayer.Play(sfx);
        }

        public static void ExitWithDebugMessage(object message) {
            Debug.WriteLine("");
            Debug.WriteLine($"{message}");
            Debug.WriteLine("");
            Environment.Exit(0);
        }

        public static class RandomHandler {
            public static Random random;
            
            public static Color RandomColor() {
                int r = random.Next(0, 255);
                int g = random.Next(0, 255);
                int b = random.Next(0, 255);

                return new Color(r, g, b, 255);
            }

            public static float GetRandomFloatingNumber(float minimum, float maximum) {
                return (float)random.NextDouble() * (maximum - minimum) + minimum;
            }

            public static int GetRandomIntNumber(int minimum, int maximum) {
                return random.Next(minimum, maximum + 1);
            }
        }

        public static class InputHandler
        {
            private static bool mousePressed = false;
            private static bool mouseReleased = false;

            /// <summary>
            /// Invokes callback when left mouse is pressed
            /// </summary>
            /// <param name="onMouseDown">Invoked callback</param>
            public static void OnMouseDown(Action onMouseDown)
            {
                MouseState currentMouseState = Mouse.GetState();
                if (currentMouseState.LeftButton == ButtonState.Pressed && mousePressed)
                {
                    onMouseDown?.Invoke();
                    mousePressed = false;
                }

                if (currentMouseState.LeftButton == ButtonState.Released)
                {
                    mousePressed = true;
                }
            }

            /// <summary>
            /// Invokes callback when left mouse is released
            /// </summary>
            /// <param name="onMouseUp">Invoked callback</param>
            public static void OnMouseUp(Action onMouseUp)
            {
                MouseState currentMouseState = Mouse.GetState();
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseReleased = true;
                }
                if (currentMouseState.LeftButton == ButtonState.Released && mouseReleased)
                {
                    onMouseUp?.Invoke();
                    mouseReleased = false;
                }
            }
        }
    }
#pragma warning restore IDE1006s
}
