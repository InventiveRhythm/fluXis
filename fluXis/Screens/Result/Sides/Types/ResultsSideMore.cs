using System.Linq;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Result.Sides.Presets;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideMore : ResultsSideContainer
{
    protected override LocalisableString Title => "More Info";

    private Bindable<ScoreInfo> score { get; }

    public ResultsSideMore(Bindable<ScoreInfo> score)
    {
        this.score = score;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        score.BindValueChanged(_ => RebuildContent());
    }

    protected override Drawable CreateContent()
    {
        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(16)
        };

        flow.Add(new ResultsSideDoubleText("FL:PF Ratio", score.Value.Flawless switch
        {
            0 => "0",
            > 0 when score.Value.Perfect == 0 => "Full Flawless",
            _ => $"{(score.Value.Flawless / (float)score.Value.Perfect).ToStringInvariant("0.0")}:1"
        }));

        if (score.Value.HitResults is { Count: > 0 })
        {
            var nonMiss = score.Value.HitResults.Where(r => r.Judgement > Judgement.Miss).ToList();
            var avg = nonMiss.Count > 1 ? nonMiss.Average(x => x.Difference) : 0;
            flow.Add(new ResultsSideDoubleText("Mean", $"{(int)-avg}ms"));
        }

        flow.Add(new ResultsSideDoubleText("Scroll Speed", $"{score.Value.ScrollSpeed.ToStringInvariant()}"));
        return flow;
    }
}
