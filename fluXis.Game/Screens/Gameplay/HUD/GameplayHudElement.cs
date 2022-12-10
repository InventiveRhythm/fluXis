using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class GameplayHUDElement : Container
    {
        public GameplayScreen Screen { get; }

        public GameplayHUDElement(GameplayScreen screen)
        {
            Screen = screen;
        }
    }
}
