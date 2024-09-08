using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.Center;

public partial class ResultsCenterScore : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load(Results results, ScoreInfo score)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Margin = new MarginPadding { Top = 36 };

        var difference = "";

        if (results.ComparisonScore != null)
            difference = $"{score.Score - results.ComparisonScore.Score:+#0;-#0}";

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
                    Text = $"{score.Score:000000}",
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
