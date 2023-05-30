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
    public int LabelSize { get; init; } = 24;
    public int DescriptionSize { get; init; } = 18;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

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
                    new SpriteText
                    {
                        Text = Label,
                        Font = FluXisFont.Default(LabelSize),
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    },
                    new SpriteText
                    {
                        Text = Description,
                        Colour = FluXisColors.Text2,
                        Font = FluXisFont.Default(DescriptionSize),
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft
                    },
                }
            }
        };
    }
}
