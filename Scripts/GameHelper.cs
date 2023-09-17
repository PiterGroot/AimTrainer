using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Timers;

namespace FirstMonoGame.Scripts
{
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

        private static void OnUpdate() { }

        public static class RandomHandler {
            public static Random random;

            public static float GetRandomFloatingNumber(float minimum, float maximum) {
                return (float)random.NextDouble() * (maximum - minimum) + minimum;
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
}
