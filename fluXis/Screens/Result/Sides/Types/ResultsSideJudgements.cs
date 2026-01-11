using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Result.Sides.Presets;
using fluXis.Skinning;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideJudgements : ResultsSideContainer
{
    protected override LocalisableString Title => "Judgements";

    private ISkin skin { get; }
    private ScoreInfo score { get; }
    private int selectedPlayer { get; set; }

    public ResultsSideJudgements(ISkin skin, ScoreInfo score, int selectedPlayer)
    {
        this.score = score;
        this.skin = skin;
        this.selectedPlayer = (selectedPlayer >= score.Players.Count) ? 0 : selectedPlayer;
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(16),
        Children = new Drawable[]
        {
            new ResultsSideDoubleText("Flawless", $"{score.Players[selectedPlayer].Flawless}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Flawless) },
            new ResultsSideDoubleText("Perfect", $"{score.Players[selectedPlayer].Perfect}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Perfect) },
            new ResultsSideDoubleText("Great", $"{score.Players[selectedPlayer].Great}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Great) },
            new ResultsSideDoubleText("Alright", $"{score.Players[selectedPlayer].Alright}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Alright) },
            new ResultsSideDoubleText("Okay", $"{score.Players[selectedPlayer].Okay}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Okay) },
            new ResultsSideDoubleText("Miss", $"{score.Players[selectedPlayer].Miss}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Miss) },
        }
    };
}
