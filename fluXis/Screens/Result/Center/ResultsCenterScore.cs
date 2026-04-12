using fluXis.Graphics.Sprites.Text;
using fluXis.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Center;

public partial class ResultsCenterScore : CompositeDrawable
{
    [Resolved]
    private Bindable<ScoreInfo> score { get; set; }

    [Resolved]
    private Results results { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Margin = new MarginPadding { Top = 36 };

        setContent();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        score.BindValueChanged(_ => setContent());
    }

    private void setContent()
    {
        var difference = "";

        if (results.ComparisonScore != null)
            difference = $"{score.Value.Score - results.ComparisonScore.Score:+#0;-#0}";

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(14),
            Children = new Drawable[]
            {
                new ForcedHeightText
                {
                    Text = "Score",
                    WebFontSize = 24,
                    Height = 18,
                    Shadow = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .8f
                },
                new ForcedHeightText
                {
                    Text = $"{score.Value.Score:000000}",
                    WebFontSize = 64,
                    Height = 48,
                    Shadow = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new ForcedHeightText
                {
                    Text = difference,
                    WebFontSize = 24,
                    Height = 18,
                    Shadow = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = string.IsNullOrEmpty(difference) ? 0 : .6f,
                }
            }
        };
    }
}
