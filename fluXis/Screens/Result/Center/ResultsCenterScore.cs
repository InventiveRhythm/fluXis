using fluXis.Graphics.Sprites.Text;
using fluXis.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Result.Center;

public partial class ResultsCenterScore : CompositeDrawable
{
    [Resolved]
    private Results results { get; set; }

    [Resolved]
    private ScoreInfo score { get; set; }

    private ForcedHeightText scoreText;
    private ForcedHeightText differenceText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Margin = new MarginPadding { Top = 36 };

        int playerIndex = results.SelectedPlayer.Value;

        var difference = "";

        if (results.ComparisonScore != null)
            difference = $"{score.Players[playerIndex].Score - results.ComparisonScore.Players[0].Score:+#0;-#0}"; //TODO: better logic for dual maps

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
                scoreText = new ForcedHeightText
                {
                    Text = $"{score.Players[playerIndex].Score:000000}",
                    WebFontSize = 64,
                    Height = 48,
                    Shadow = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                differenceText = new ForcedHeightText
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

        results.SelectedPlayer.BindValueChanged(v => updateContentForPlayer(v.NewValue));
    }

    private void updateContentForPlayer(int playerIndex)
    {
        var difference = "";

        if (results.ComparisonScore != null)
            difference = $"{score.Players[playerIndex].Score - results.ComparisonScore.Players[0].Score:+#0;-#0}"; //TODO: better logic for dual maps

        scoreText.Text = $"{score.Players[playerIndex].Score:000000}";
        differenceText.Text = difference;
    }
}
