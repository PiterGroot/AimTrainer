using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Timers;

namespace FirstMonoGame.Scripts
{
#pragma warning disable IDE1006 
    public static class GameHelper
    {
        private static GameController gameController;
        public static GameController GameController
        {
            get => gameController;
            set
            {
                if (value == null) return;
                gameController = value;

                gameController.onUpdate += OnUpdate;
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

        /// <summary>
        /// Starts a while loop for some amount of time and calls back every update tick during the loop
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="updateCallBack"></param>
        public static void WhileFor(float duration, Action updateCallBack) {
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(duration)) updateCallBack?.Invoke();
        }

        public static void print(object message) {
            Debug.WriteLine(message.ToString());    
        }

        private static void OnUpdate() { }

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
