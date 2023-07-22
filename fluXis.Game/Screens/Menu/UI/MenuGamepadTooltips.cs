using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuGamepadTooltips : Container
{
    public Container ButtonContainer { get; init; }

    private const int button_size = 40;

    private GamepadIcon play;
    private GamepadIcon multiplayer;
    private GamepadIcon browse;
    private GamepadIcon rankings;
    private GamepadIcon options;
    private GamepadIcon exit;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new[]
        {
            play = new GamepadIcon
            {
                ButtonName = "A",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            },
            multiplayer = new GamepadIcon
            {
                ButtonName = "B",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            },
            browse = new GamepadIcon
            {
                ButtonName = "Y",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            },
            rankings = new GamepadIcon
            {
                ButtonName = "X",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            },
            options = new GamepadIcon
            {
                ButtonName = "Menu",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            },
            exit = new GamepadIcon
            {
                ButtonName = "View",
                Origin = Anchor.Centre,
                Size = new Vector2(button_size)
            }
        };
    }

    protected override void Update()
    {
        if (ButtonContainer == null || ButtonContainer.Count < 6) return;

        updatePosition(play, ButtonContainer[0]);
        updatePosition(options, ButtonContainer[1]);
        updatePosition(multiplayer, ButtonContainer[2]);
        updatePosition(rankings, ButtonContainer[3]);
        updatePosition(exit, ButtonContainer[4]);
        updatePosition(browse, ButtonContainer[5]);
    }

    private void updatePosition(GamepadIcon icon, Drawable button)
    {
        var delta = button.ScreenSpaceDrawQuad.TopLeft - icon.ScreenSpaceDrawQuad.Centre;
        delta += new Vector2(5);

        icon.Position += delta;
        icon.Alpha = GamepadHandler.GamepadConnected ? ButtonContainer.Alpha : 0;
    }
}
