using fluXis.Game.Graphics;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;
using fluXis.Game.Screens.Multiplayer.SubScreens.Ranked;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens;

public partial class MultiModeSelect : MultiSubScreen
{
    public override string Title => "Multiplayer";
    public override string SubTitle => "Mode Select";

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 20),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new ClickableContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Action = () => this.Push(new MultiRankedMain()),
                    Child = new FluXisSpriteText
                    {
                        Text = "Ranked",
                        FontSize = 40
                    }
                },
                new ClickableContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Action = () => this.Push(new MultiLobbyList()),
                    Child = new FluXisSpriteText
                    {
                        Text = "Open Match",
                        FontSize = 40
                    }
                }
            }
        };
    }
}
