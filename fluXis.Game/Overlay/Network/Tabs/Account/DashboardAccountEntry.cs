using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs.Account;

public partial class DashboardAccountEntry : Container
{
    public string Title { get; set; }
    public string Default { get; set; }
    public string Placeholder { get; set; }
    public Action OnChange { get; set; }
    public bool ReadOnly { get; set; }

    public string Value => textBox.Text;
    private TextBox textBox;

    public Drawable LoadingIcon { get; set; }
    public Drawable CompletedIcon { get; set; }

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
                            },
                            new Container
                            {
                                Size = new Vector2(12),
                                Margin = new MarginPadding(3),
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Children = new[]
                                {
                                    LoadingIcon = new SpriteIcon
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Icon = FontAwesome6.Solid.Rotate,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Alpha = 0
                                    },
                                    CompletedIcon = new SpriteIcon
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Icon = FontAwesome6.Solid.Check,
                                        Alpha = 0
                                    }
                                }
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

    protected override void LoadComplete()
    {
        base.LoadComplete();
        LoadingIcon.Spin(1000, RotationDirection.Clockwise);
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
