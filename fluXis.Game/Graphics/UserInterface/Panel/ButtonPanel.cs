using System;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ButtonPanel : Panel, ICloseable
{
    public IconUsage Icon { get; init; } = FontAwesome.Solid.QuestionCircle;
    public LocalisableString Text { get; init; }
    public LocalisableString SubText { get; init; }
    public ButtonData[] Buttons { get; init; } = Array.Empty<ButtonData>();

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 490;
        AutoSizeAxes = Axes.Y;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;
        Content.Padding = new MarginPadding(20) { Top = 30 };
        Content.Children = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Icon = Icon,
                        Size = new Vector2(64),
                        Margin = new MarginPadding { Bottom = 10 }
                    },
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        Text = Text,
                        FontSize = FluXisSpriteText.GetWebFontSize(20),
                        Shadow = false
                    },
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        Text = SubText,
                        FontSize = FluXisSpriteText.GetWebFontSize(14),
                        Alpha = string.IsNullOrEmpty(SubText.ToString()) ? 0 : .8f,
                        Shadow = false
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(10),
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding { Top = 20 },
                        ChildrenEnumerable = Buttons.Select(b => new FluXisButton
                        {
                            Width = 450,
                            Height = 50,
                            Data = b,
                            FontSize = FluXisSpriteText.GetWebFontSize(16),
                            Action = () =>
                            {
                                b.Action?.Invoke();
                                Hide();
                            }
                        })
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e) => true;

    public void Close()
    {
        var last = Buttons.Last();
        last.Action?.Invoke();
        Hide();
    }
}
