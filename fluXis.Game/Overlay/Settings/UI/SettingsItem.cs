using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsItem : Container
{
    public string Label { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;
        Alpha = Enabled ? 1 : 0.5f;

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = Label,
                        FontSize = 24,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    },
                    new FluXisSpriteText
                    {
                        Text = Description,
                        Colour = FluXisColors.Text2,
                        FontSize = 16,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    }
                }
            }
        };
    }
}
