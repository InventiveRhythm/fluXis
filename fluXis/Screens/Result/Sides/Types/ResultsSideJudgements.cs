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

    public ResultsSideJudgements(ISkin skin, ScoreInfo score)
    {
        this.score = score;
        this.skin = skin;
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(16),
        Children = new Drawable[]
        {
            new ResultsSideDoubleText("Flawless", $"{score.Flawless}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Flawless) },
            new ResultsSideDoubleText("Perfect", $"{score.Perfect}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Perfect) },
            new ResultsSideDoubleText("Great", $"{score.Great}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Great) },
            new ResultsSideDoubleText("Alright", $"{score.Alright}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Alright) },
            new ResultsSideDoubleText("Okay", $"{score.Okay}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Okay) },
            new ResultsSideDoubleText("Miss", $"{score.Miss}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Miss) },
        }
    };
}
