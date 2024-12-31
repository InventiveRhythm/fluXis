using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Browse.Info;

public partial class BrowseInfoTagsChip : Container
{
    private TruncatingText noTagsText;
    private FillFlowContainer tagsFlow;

    public string[] Tags
    {
        set
        {
            tagsFlow.Clear();

            if (value == null || value.Length == 0 || (value.Length == 1 && string.IsNullOrEmpty(value[0])))
            {
                noTagsText.FadeTo(.5f);
                return;
            }

            noTagsText.Hide();

            foreach (var tag in value)
            {
                tagsFlow.Add(new TagText
                {
                    Text = tag
                });
            }
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 760;
        AutoSizeAxes = Axes.Y;
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
                Padding = new MarginPadding(15) { Vertical = 12 },
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = "Tags",
                        FontSize = 16,
                        Colour = FluXisColors.Text2
                    },
                    noTagsText = new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = "No Tags",
                        Alpha = 0.5f,
                        FontSize = 20
                    },
                    tagsFlow = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Full,
                        Spacing = new Vector2(4, 0),
                    }
                }
            }
        };
    }

    private partial class TagText : FluXisSpriteText
    {
        protected override bool OnHover(HoverEvent e)
        {
            this.FadeColour(FluXisColors.Highlight, 50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeColour(FluXisColors.Text, 200);
        }
    }
}
