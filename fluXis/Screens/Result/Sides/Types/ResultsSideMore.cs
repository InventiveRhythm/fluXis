using System.Linq;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Result.Sides.Presets;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideMore : ResultsSideContainer
{
    protected override LocalisableString Title => "More Info";

    private ScoreInfo score { get; }
    private int selectedPlayer { get; }

    public ResultsSideMore(ScoreInfo score, int selectedPlayer)
    {
        this.score = score;
        this.selectedPlayer = (selectedPlayer >= score.Players.Count) ? selectedPlayer - 1 : selectedPlayer;
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

        flow.Add(new ResultsSideDoubleText("FL:PF Ratio", score.Players[selectedPlayer].Flawless switch
        {
            0 => "0",
            > 0 when score.Players[selectedPlayer].Perfect == 0 => "Full Flawless",
            _ => $"{(score.Players[selectedPlayer].Flawless / (float)score.Players[selectedPlayer].Perfect).ToStringInvariant("0.0")}:1"
        }));

        if (score.Players[selectedPlayer].HitResults is { Count: > 0 })
        {
            var nonMiss = score.Players[selectedPlayer].HitResults.Where(r => r.Judgement > Judgement.Miss).ToList();
            var avg = nonMiss.Count > 1 ? nonMiss.Average(x => x.Difference) : 0;
            flow.Add(new ResultsSideDoubleText("Mean", $"{(int)-avg}ms"));
        }

        flow.Add(new ResultsSideDoubleText("Scroll Speed", $"{score.Players[selectedPlayer].ScrollSpeed.ToStringInvariant()}"));
        return flow;
    }
}
