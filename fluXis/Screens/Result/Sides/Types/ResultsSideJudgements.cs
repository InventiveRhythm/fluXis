using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Result.Sides.Presets;
using fluXis.Skinning;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideJudgements : ResultsSideContainer
{
    protected override LocalisableString Title => "Judgements";

    private ISkin skin { get; }
    private Bindable<ScoreInfo> score { get; }

    public ResultsSideJudgements(ISkin skin, Bindable<ScoreInfo> score)
    {
        this.score = score;
        this.skin = skin;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        score.BindValueChanged(_ => RebuildContent());
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(16),
        Children = new Drawable[]
        {
            new ResultsSideDoubleText("Flawless", $"{score.Value.Flawless}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Flawless) },
            new ResultsSideDoubleText("Perfect", $"{score.Value.Perfect}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Perfect) },
            new ResultsSideDoubleText("Great", $"{score.Value.Great}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Great) },
            new ResultsSideDoubleText("Alright", $"{score.Value.Alright}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Alright) },
            new ResultsSideDoubleText("Okay", $"{score.Value.Okay}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Okay) },
            new ResultsSideDoubleText("Miss", $"{score.Value.Miss}") { Colour = skin.SkinJson.GetColorForJudgement(Judgement.Miss) },
        }
    };
}
