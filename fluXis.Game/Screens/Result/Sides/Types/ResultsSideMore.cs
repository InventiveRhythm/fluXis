using fluXis.Game.Screens.Result.Sides.Presets;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Result.Sides.Types;

public partial class ResultsSideMore : ResultsSideContainer
{
    protected override LocalisableString Title => "More Info";

    private ScoreInfo score { get; }

    public ResultsSideMore(ScoreInfo score)
    {
        this.score = score;
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(16),
        Children = new Drawable[]
        {
            new ResultsSideDoubleText("FL:PF Ratio", score.Flawless switch
            {
                0 => "0",
                > 0 when score.Perfect == 0 => "Full Flawless",
                _ => $"{(score.Flawless / (float)score.Perfect).ToStringInvariant("0.0")}:1"
            }),
            new ResultsSideDoubleText("Scroll Speed", $"{score.ScrollSpeed.ToStringInvariant()}")
        }
    };
}
