using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;

public partial class MultiRankedMain : MultiSubScreen
{
    public override string Title => "Ranked";
    public override string SubTitle => "Main Menu";

    [Resolved]
    private MultiplayerMenuMusic menuMusic { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Nothing here yet!",
                FontSize = 40,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Shadow = true
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Children = new Drawable[]
                {
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () => menuMusic.GoToLayer(0, 0),
                        Child = new FluXisSpriteText { Text = "Play Main Menu Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () => menuMusic.GoToLayer(1, 0),
                        Child = new FluXisSpriteText { Text = "Play Prepare Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () => menuMusic.GoToLayer(2, 0),
                        Child = new FluXisSpriteText { Text = "Play Win Music" }
                    },
                    new ClickableContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Action = () => menuMusic.GoToLayer(2, 0, 1),
                        Child = new FluXisSpriteText { Text = "Play Lose Music" }
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.MoveToY(100).MoveToY(0, 400, Easing.OutQuint);
        menuMusic.GoToLayer(0, 0);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.MoveToY(100, 400, Easing.OutQuint);
        return base.OnExiting(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        menuMusic.GoToLayer(0, 0);
        base.OnResuming(e);
    }
}
