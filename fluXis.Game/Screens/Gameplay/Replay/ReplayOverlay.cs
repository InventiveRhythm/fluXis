using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Replay;

public partial class ReplayOverlay : FillFlowContainer
{
    private Replays.Replay replay { get; }

    public ReplayOverlay(Replays.Replay replay)
    {
        this.replay = replay;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        Direction = FillDirection.Vertical;
        Y = 80;
        Alpha = .8f;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                FontSize = 32,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Replay Mode (Experimental)"
            },
            new FluXisSpriteText
            {
                FontSize = 24,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = $"Watching {replay.Player?.NameWithApostrophe} replay",
                Alpha = .8f
            }
        };
    }
}
