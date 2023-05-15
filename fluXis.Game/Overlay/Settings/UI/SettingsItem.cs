using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsItem : Container
{
    public string Label { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

        InternalChildren = new Drawable[]
        {
            new SpriteText
            {
                Text = Label,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            }
        };
    }
}
