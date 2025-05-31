using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Account;

public partial class DashboardAccountTextBox : Container
{
    public string Title { get; init; }
    public string Default { get; init; }
    public string Placeholder { get; init; }
    public Action OnChange { get; init; }
    public bool ReadOnly { get; init; }

    public string Value
    {
        get => textBox.Text;
        set => textBox.Text = value;
    }

    private TextBox textBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(500, 50);
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 10, Vertical = 8 },
                Spacing = new Vector2(-3),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = Title,
                                FontSize = 18,
                                Alpha = .8f
                            }
                        }
                    },
                    textBox = new TextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 24,
                        Alpha = ReadOnly ? .5f : 1,
                        Text = Default,
                        PlaceholderText = Placeholder,
                        ReadOnly = ReadOnly,
                        OnTextChanged = OnChange
                    }
                }
            }
        };
    }

    private partial class TextBox : FluXisTextBox
    {
        protected override float LeftRightPadding => 0;

        [BackgroundDependencyLoader]
        private void load()
        {
            TextContainer.Height = 1;
            BackgroundActive = FluXisColors.Background3;
            BackgroundFocused = FluXisColors.Background3;
            BackgroundInactive = FluXisColors.Background3;
            BackgroundUnfocused = FluXisColors.Background3;
            BackgroundCommit = FluXisColors.Background3;
        }
    }
}
