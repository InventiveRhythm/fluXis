using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Gameplay.Input
{
    public class GameplayInput
    {
        private static readonly Key[] keys = { Key.A, Key.S, Key.K, Key.L };
        private readonly bool[] pressedStatus;
        public bool[] JustPressed { get; }
        public bool[] Pressed { get; }
        public bool[] JustReleased { get; }

        public GameplayInput()
        {
            pressedStatus = new bool[keys.Length];
            JustPressed = new bool[keys.Length];
            Pressed = new bool[keys.Length];
            JustReleased = new bool[keys.Length];
        }

        public void Update()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (pressedStatus[i] && !Pressed[i])
                {
                    JustPressed[i] = true;
                    Pressed[i] = true;
                }
                else if (!pressedStatus[i] && Pressed[i])
                {
                    JustReleased[i] = true;
                    Pressed[i] = false;
                }
                else
                {
                    JustPressed[i] = false;
                    JustReleased[i] = false;
                }
            }
        }

        public void OnKeyDown(KeyDownEvent e)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (e.Key == keys[i])
                {
                    pressedStatus[i] = true;
                }
            }
        }

        public void OnKeyUp(KeyUpEvent e)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (e.Key == keys[i])
                {
                    pressedStatus[i] = false;
                }
            }
        }
    }
}
