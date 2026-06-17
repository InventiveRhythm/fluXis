using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Replays;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Replays;

#nullable enable

public partial class ReplayOverlay : FillFlowContainer
{
    [Resolved]
    private UserCache users { get; set; } = null!;

    public string Title { get; set; } = "Replay Mode";
    public Func<APIUser?, string> SubTitle { get; set; } = u => $"Watching {u?.NameWithApostrophe} replay";

    private Replay replay { get; }

    public ReplayOverlay(Replay replay)
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
        createContent();
    }

    public void Recreate() => createContent();

    private void createContent()
    {
        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                FontSize = 32,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = Title
            },
            new FluXisSpriteText
            {
                FontSize = 24,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = SubTitle.Invoke(replay.GetPlayer(users)),
                Alpha = .8f
            }
        };
    }
}
