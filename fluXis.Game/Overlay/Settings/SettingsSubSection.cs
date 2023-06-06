using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsSubSection : FillFlowContainer
{
    [Resolved]
    protected FluXisConfig Config { get; private set; }

    public virtual string Title => "Subsection";

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);

        InternalChild = new FluXisSpriteText
        {
            Text = Title,
            FontSize = 38
        };
    }
}
