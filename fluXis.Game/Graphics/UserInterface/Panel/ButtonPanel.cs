using System.Linq;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ButtonPanel : Panel
{
    public string Text { get; set; }
    public string SubText { get; set; }
    public ButtonData[] Buttons { get; set; }
    public float ButtonWidth { get; init; } = 150;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 600;
        Height = 400;

        Content.Children = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Text = Text,
                        FontSize = 30,
                        TextAnchor = Anchor.TopCentre,
                        Shadow = true
                    },
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Text = SubText,
                        FontSize = 22,
                        Colour = FluXisColors.Text2,
                        TextAnchor = Anchor.TopCentre,
                        Shadow = true
                    }
                }
            },
            new FillFlowContainer
            {
                Height = 50,
                AutoSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(20),
                Children = Buttons.Select(b => new FluXisButton
                {
                    Width = ButtonWidth,
                    RelativeSizeAxes = Axes.Y,
                    Data = b,
                    Action = () =>
                    {
                        b.Action?.Invoke();
                        Hide();
                    }
                }).ToArray()
            }
        };
    }
}
