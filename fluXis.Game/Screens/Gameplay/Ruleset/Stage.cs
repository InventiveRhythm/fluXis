using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Stage : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        AddRangeInternal(new[]
        {
            skinManager.GetStageBackground(),
            skinManager.GetStageBorder(false),
            skinManager.GetStageBorder(true)
        });
    }
}
