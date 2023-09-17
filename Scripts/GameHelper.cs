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

        private static void OnUpdate() { }

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
