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

    private SkinManager skins { get; }
    private ScoreInfo score { get; }

    public ResultsSideJudgements(SkinManager skins, ScoreInfo score)
    {
        this.score = score;
        this.skins = skins;
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(16),
        Children = new Drawable[]
        {
            new ResultsSideDoubleText("Flawless", $"{score.Flawless}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Flawless) },
            new ResultsSideDoubleText("Perfect", $"{score.Perfect}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Perfect) },
            new ResultsSideDoubleText("Great", $"{score.Great}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Great) },
            new ResultsSideDoubleText("Alright", $"{score.Alright}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Alright) },
            new ResultsSideDoubleText("Okay", $"{score.Okay}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Okay) },
            new ResultsSideDoubleText("Miss", $"{score.Miss}") { Colour = skins.SkinJson.GetColorForJudgement(Judgement.Miss) },
        }
    };
}
