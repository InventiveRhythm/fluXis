using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsTextBox : Container
{
    public string Text { get; init; }
    public string DefaultText { get; init; }
    public string ExtraText { get; init; }
    public int TextBoxWidth { get; init; } = 210;
    public Action<FluXisTextBox> OnTextChanged { get; set; }

    public FluXisTextBox TextBox { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Text,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 16
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    TextBox = new FluXisTextBox
                    {
                        Width = TextBoxWidth,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Text = DefaultText,
                        SidePadding = 10,
                        TextContainerHeight = .7f,
                        BackgroundInactive = FluXisColors.Background3,
                        BackgroundActive = FluXisColors.Background4,
                        OnTextChanged = () => OnTextChanged?.Invoke(TextBox),
                        OnCommitAction = () => OnTextChanged?.Invoke(TextBox)
                    },
                    new FluXisSpriteText
                    {
                        Text = ExtraText,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16,
                        Alpha = string.IsNullOrEmpty(ExtraText) ? 0 : 1
                    }
                }
            }
        };
    }
}
