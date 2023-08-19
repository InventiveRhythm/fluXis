using System.Linq;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Panel;

public partial class ButtonPanel : Panel
{
    public string Text { get; set; }
    public ButtonData[] Buttons { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 600;
        Height = 400;

        Content.Children = new Drawable[]
        {
            new FluXisTextFlow
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Text = Text,
                FontSize = 30,
                TextAnchor = Anchor.TopCentre,
                Shadow = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
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
                    RelativeSizeAxes = Axes.Y,
                    Width = 150,
                    Text = b.Text,
                    Color = b.Color,
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
